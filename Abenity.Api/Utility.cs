using System;
using System.Text;
using System.Web;

namespace Abenity.Api
{
    internal static class Utility
    {
        internal static byte[] GetBytes(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        internal static string Base64String(byte[] b)
        {
            return Convert.ToBase64String(b);
        }

        internal static string UrlEncode(string str)
        {
            return HttpUtility.UrlEncode(str);
        }

    }
}
