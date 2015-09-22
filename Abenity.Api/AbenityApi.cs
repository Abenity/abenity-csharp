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
    public class SsoMemberPayload
    {
        public bool SendWelcomeEmail { get; set; }
        public string ClientUserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
    }

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
            _apiUrl = (useProduction) ? "https://api.abenity.com/v1/client/sso_member.json" : "https://sandbox.abenity.com/v1/client/sso_member.json";
        }

        /// <summary>
        /// POST to the sso_member.json endpoint.
        /// </summary>
        /// <param name="ssoMemberPayload">The SSO member payload to send to Abenity</param>
        public void PostSsoMember(SsoMemberPayload ssoMemberPayload)
        {
            _ssoMemberPayload = ssoMemberPayload;
            Post();
        }

        private void Post()
        {
            var request = (HttpWebRequest)WebRequest.Create(_apiUrl);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(GetApiBody());
            }

            var response = (HttpWebResponse)request.GetResponse();

            var receiveStream = response.GetResponseStream();
            var readStream = new StreamReader(receiveStream, Encoding.UTF8);

            Console.WriteLine(readStream.ReadToEnd());
            response.Close();
            readStream.Close();
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
            string firstName = _ssoMemberPayload.FirstName;
            string lastName = _ssoMemberPayload.LastName;
            string address = _ssoMemberPayload.Address;
            string city = _ssoMemberPayload.City;
            string state = _ssoMemberPayload.State;
            string zip = _ssoMemberPayload.Zip;
            string country = _ssoMemberPayload.Country;

            string message = string.Format("creation_time={0}&salt={1}&send_welcome_email={2}" +
                "&client_user_id={3}&email={4}&firstname={5}&lastname={6}&address={7}&city={8}&state={9}&zip={10}&country={11}",
                Utility.UrlEncode(creationTime),
                salt,
                sendWelcomeEmail,
                clientUserId,
                Utility.UrlEncode(email),
                Utility.UrlEncode(firstName),
                Utility.UrlEncode(lastName),
                Utility.UrlEncode(address),
                Utility.UrlEncode(city),
                Utility.UrlEncode(state),
                Utility.UrlEncode(zip),
                Utility.UrlEncode(country)
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
            var pemReader = new PemReader(File.OpenText("abenity-public.pem"));
            var key = (AsymmetricKeyParameter)pemReader.ReadObject();

            var engine = new Pkcs1Encoding(new RsaEngine());
            engine.Init(true, key);

            var data = engine.ProcessBlock(_desKey, 0, _desKey.Length);

            return Utility.UrlEncode(Utility.Base64String(data)) + "decode";
        }
    }
}
