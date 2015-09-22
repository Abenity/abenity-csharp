using Abenity;
using Abenity.Api;
using System;

public class Program
{
    private static string API_USER_NAME = "StrategyCorps";
    private static string API_PASSWORD = "KzdVX6";
    private static string API_KEY = "AoxXDwT9yG7Lv97n64iWUgj4NbUnLorm";

    public static void Main(String[] args)
    {
        var apiCredential = new ApiCredential(API_USER_NAME, API_PASSWORD, API_KEY);
        var clientKeys = new ClientKeys(@"c:\work\abenity\private.pem", @"c:\work\abenity\public.pem");

        var abenityApi = new AbenityApi(apiCredential, clientKeys);
        abenityApi.PostSsoMember(new SsoMemberPayload()
        {
            Address = "67 Bianco",
            City = "Irvine",
            ClientUserId = "7",
            Country = "US",
            Email = "ryan@meyer.com",
            FirstName = "Ryan",
            LastName = "Meyer",
            SendWelcomeEmail = true,
            State = "CA",
            Zip = "92618"
        });
    }
}
