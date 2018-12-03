using System;
using System.Text;
using System.Web;

namespace Abenity.Api
{
    internal static class Utility
    {
        internal static byte[] GetBytes(string str) => Encoding.UTF8.GetBytes(str);

        internal static string Base64String(byte[] b) => Convert.ToBase64String(b);

        internal static string UrlEncode(string str) => HttpUtility.UrlEncode(str);
    }
}
