using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Abenity.Api
{
    public class AbenityApi
    {
        private ApiCredential _apiCredential;
        private ClientKeys _clientKeys;
        private string _apiUrl;

        private SsoMemberPayload _ssoMemberPayload;

        private byte[] _desKey;
        private byte[] _desIv;
        private string _base64EncodedMessage;

        /// <summary>
        /// Construct a new AbenityApi object to interact with the Abenity Api.
        /// </summary>
        /// <param name="apiCredential">Your Abenity credentials</param>
        /// <param name="clientKeys">Your public and private keys</param>
        /// <param name="useProduction">(optional) Point Api calls at Abenity's production environment?</param>
        public AbenityApi(ApiCredential apiCredential, ClientKeys clientKeys, bool useProduction = false)
        {
            _apiCredential = apiCredential;
            _clientKeys = clientKeys;
            _apiUrl = (useProduction) ? "https://api.abenity.com/v2/client/sso_member.json" : "https://sandbox.abenity.com/v2/client/sso_member.json";
        }

        /// <summary>
        /// POST to the sso_member.json endpoint.
        /// </summary>
        /// <param name="ssoMemberPayload">The SSO member payload to send to Abenity</param>
        public string PostSsoMember(SsoMemberPayload ssoMemberPayload)
        {
            _ssoMemberPayload = ssoMemberPayload;
            return Post();
        }

        private string Post()
        {
            // System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;

            var request = (HttpWebRequest)WebRequest.Create(_apiUrl);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "abenity/abenity-csharp v2";

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(GetApiBody());
            }

            var response = (HttpWebResponse)request.GetResponse();

            var receiveStream = response.GetResponseStream();
            var readStream = new StreamReader(receiveStream, Encoding.UTF8);

            string output = readStream.ReadToEnd();

            response.Close();
            readStream.Close();

            Console.WriteLine(output);

            return output;
        }

        private string GetApiBody()
        {
            var payload = EncryptPayloadWithDES(GetMessage());
            var signature = SignMessage(_base64EncodedMessage);
            var cipher = EncryptDESKey();
            var iv = Utility.UrlEncode(Utility.Base64String(_desIv)) + "decode";

            return string.Format("api_username={0}&api_password={1}&api_key={2}&Payload={3}&Signature={4}&Cipher={5}&Iv={6}",
                _apiCredential.Username,
                _apiCredential.Password,
                _apiCredential.Key,
                payload,
                signature,
                cipher,
                iv
            );
        }

        private string GetMessage()
        {
            string creationTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
            int salt = new Random().Next();
            string sendWelcomeEmail = _ssoMemberPayload.SendWelcomeEmail ? "1" : "0";
            string clientUserId = _ssoMemberPayload.ClientUserId;
            string email = _ssoMemberPayload.Email;
            string username = _ssoMemberPayload.Username;
            string firstName = _ssoMemberPayload.FirstName;
            string lastName = _ssoMemberPayload.LastName;
            string address = _ssoMemberPayload.Address;
            string city = _ssoMemberPayload.City;
            string state = _ssoMemberPayload.State;
            string zip = _ssoMemberPayload.Zip;
            string country = _ssoMemberPayload.Country;
            string spotlight = _ssoMemberPayload.Spotlight ? "1" : "0";
            string password = !String.IsNullOrEmpty(_ssoMemberPayload.Password) ? _ssoMemberPayload.Password : "";
            string registrationCode = !String.IsNullOrEmpty(_ssoMemberPayload.RegistrationCode) ? _ssoMemberPayload.RegistrationCode : "";

            string message = string.Format("creation_time={0}&salt={1}&send_welcome_email={2}" +
                "&client_user_id={3}&email={4}&username={5}&firstname={6}&lastname={7}&address={8}&city={9}&state={10}&zip={11}&country={12}" + 
                "&spotlight={13}&password={14}&registration_code={15}",
                Utility.UrlEncode(creationTime),
                salt,
                sendWelcomeEmail,
                clientUserId,
                Utility.UrlEncode(email),
                Utility.UrlEncode(username),
                Utility.UrlEncode(firstName),
                Utility.UrlEncode(lastName),
                Utility.UrlEncode(address),
                Utility.UrlEncode(city),
                Utility.UrlEncode(state),
                Utility.UrlEncode(zip),
                Utility.UrlEncode(country),
                spotlight,
                Utility.UrlEncode(password),
                Utility.UrlEncode(registrationCode)
            );

            return message;
        }

        private string EncryptPayloadWithDES(string message)
        {
            // Uses CBC by default.
            // https://msdn.microsoft.com/en-us/library/system.security.cryptography.symmetricalgorithm.mode(v=vs.110).aspx
            var des = new TripleDESCryptoServiceProvider();

            _desKey = des.Key;
            _desIv = des.IV;

            var encryptor = des.CreateEncryptor();

            var data = Utility.GetBytes(message);
            var resultArray = encryptor.TransformFinalBlock(data, 0, data.Length);
            des.Clear();

            _base64EncodedMessage = Utility.Base64String(resultArray);
            return Utility.UrlEncode(_base64EncodedMessage) + "decode";
        }

        private string SignMessage(string hash)
        {
            byte[] data = Utility.GetBytes(hash);
            var pemReader = new PemReader(_clientKeys.PrivateKeyFileStream);
            var key = (AsymmetricCipherKeyPair)pemReader.ReadObject();

            ISigner signer = SignerUtilities.GetSigner("MD5WithRSA");
            signer.Init(true, key.Private);

            signer.BlockUpdate(data, 0, data.Length);

            return Utility.UrlEncode(Utility.Base64String(signer.GenerateSignature())) + "decode";
        }

        private string EncryptDESKey()
        {
            var pemReader = new PemReader(_clientKeys.AbenityPublicKeyFileStream);
            var key = (AsymmetricKeyParameter)pemReader.ReadObject();

            var engine = new Pkcs1Encoding(new RsaEngine());
            engine.Init(true, key);

            var data = engine.ProcessBlock(_desKey, 0, _desKey.Length);

            return Utility.UrlEncode(Utility.Base64String(data)) + "decode";
        }
    }
}
