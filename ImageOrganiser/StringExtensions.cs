using System;
using System.Collections.Generic;
using System.Text;

namespace ImageOrganiser
{
    public static class StringExtensions
    {
        public static string GetFileFromKey(this string key)
        {
            if (String.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            var newValue = key.ReplaceSlashes();
            if (newValue.LastIndexOf("/") >= 0)
            {
                return newValue?.Substring(newValue.LastIndexOf("/")+1);
            }
            return newValue;
        }

        public static string ReplaceSlashes(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return value?.Replace(@"\\", "/")?.Replace(@"\", "/");
        }
    }
}
