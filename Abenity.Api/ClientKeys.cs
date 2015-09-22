using System.IO;

namespace Abenity.Api
{
    public class ClientKeys
    {
        internal StreamReader PrivateKeyFileStream { get; private set; }
        internal StreamReader AbenityPublicKeyFileStream { get; private set; }

        /// <summary>
        /// Your own generated private key.
        /// </summary>
        /// <param name="privateKeyPath">File system location to your private key file</param>
        /// <param name="abenityPublicKeyPath">Location of Abenity's public key file</param>
        public ClientKeys(string privateKeyPath, string abenityPublicKeyPath)
        {
            PrivateKeyFileStream = File.OpenText(privateKeyPath);
            AbenityPublicKeyFileStream = File.OpenText(abenityPublicKeyPath);
        }
    }
}
