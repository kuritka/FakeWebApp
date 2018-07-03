using System;
using System.Text;

namespace ExecutiveOffice.EDT.GlobalNotesService.Extensions
{
    public static class StringExtensions
    {

        public static string ToBase64Encode(this string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }



        public static string EncodeTo(this string text, Encoding encoding )
        {
            byte[] bytes = Encoding.Default.GetBytes(text);
            return encoding.GetString(bytes);
        }



        public static string ThrowExceptionIfNullOrEmpty(this string input)
        {
            if(input.IsNullOrEmpty()) throw new ArgumentNullException("empty");
            return input;
        }





    }
}
