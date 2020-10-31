using System;

namespace az_lazy.Extensions
{
    public static class StringExtensions
    {
        public static bool IsBase64(this string base64String)
        {
            if (string.IsNullOrEmpty(base64String) || base64String.Length % 4 != 0
                || base64String.Contains(" ") || base64String.Contains("\t") || base64String.Contains("\r") || base64String.Contains("\n"))
            {
                return false;
            }

            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}