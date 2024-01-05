using Abenity;
using Abenity.Api;
using System;

public class Program
{
    private static string API_USER_NAME = "";
    private static string API_PASSWORD = "";
    private static string API_KEY = "";

    public static void Main(String[] args)
    {
        var apiCredential = new ApiCredential(API_USER_NAME, API_PASSWORD, API_KEY);
        var clientKeys = new ClientKeys(@"path-to-client-private-key.pem", @"path-to-abenity-public-key.pem");

        var abenityApi = new AbenityApi(apiCredential, clientKeys);
        abenityApi.PostSsoMember(new SsoMemberPayload()
        {
            Address = "1 Main St.",
            City = "Nashville",
            ClientUserId = "1",
            Country = "US",
            Email = "jane.doe@maildomain.com",
            Username = "jane.doe@maildomain.com",
            FirstName = "Jane",
            LastName = "Doe",
            SendWelcomeEmail = true,
            State = "TN",
            Zip = "37201"
        });
    }
}
