namespace Abenity.Api
{
    public class SsoMemberPayload
    {
        public bool SendWelcomeEmail { get; set; }
        public string ClientUserId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public bool Spotlight { get; set; }
        public string Password { get; set; }
        public string RegistrationCode { get; set; }
    }
}
