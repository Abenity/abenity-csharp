using System.IO;

namespace Abenity.Api
{
    public class ClientKeys
    {
        internal StreamReader PrivateKeyFileStream { get; private set; }

        /// <summary>
        /// Your own generated private key.
        /// </summary>
        /// <param name="privateKeyPath">File system location to your private key file</param>
        public ClientKeys(string privateKeyPath)
        {
            PrivateKeyFileStream = File.OpenText(privateKeyPath);
        }
    }
}
