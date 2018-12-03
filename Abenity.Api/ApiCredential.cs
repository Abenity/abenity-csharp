namespace Abenity.Api
{
    public class ApiCredential
    {
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Key { get; private set; }

        /// <summary>
        /// User/Client credentials to communicate with Abenity's Api
        /// </summary>
        /// <param name="username">Your Abenity Api username</param>
        /// <param name="password">Your Abenity Api password</param>
        /// <param name="key">Your Abenity Api key</param>
        public ApiCredential(string username, string password, string key)
        {
            Username = username;
            Password = password;
            Key = key;
        }
    }
}
