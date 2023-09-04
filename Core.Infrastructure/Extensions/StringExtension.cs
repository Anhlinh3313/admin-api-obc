using System;
namespace Core.Infrastructure.Extensions
{
    public static class StringExtension
    {
        public static bool EqualsIgnoreCase(this string str1, string str2)
        {
            if(!string.IsNullOrWhiteSpace(str2))
                return str1.ToUpper().Equals(str2.ToUpper());
            else
                return str1.Equals(str2);
        }
    }
}
