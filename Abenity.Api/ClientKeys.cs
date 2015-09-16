using System.IO;

namespace Abenity.Api
{
    public class ClientKeys
    {
        internal StreamReader PrivateKeyFileStream { get; private set; }
        internal StreamReader PublicKeyFileStream { get; private set; }

        /// <summary>
        /// Your own generated private and public key files.
        /// </summary>
        /// <param name="privateKeyPath">File system location to your private key file</param>
        /// <param name="publicKeyPath">File system location to your public key file</param>
        public ClientKeys(string privateKeyPath, string publicKeyPath)
        {
            PrivateKeyFileStream = File.OpenText(privateKeyPath);
            PublicKeyFileStream = File.OpenText(publicKeyPath);
        }
    }
}
