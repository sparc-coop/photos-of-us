using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.Services
{
    public static class StringHelpers
    {
        public static string Replace(this string s, char[] separators, string newVal)
        {
            var temp = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(newVal, temp);
        }
    }
}
