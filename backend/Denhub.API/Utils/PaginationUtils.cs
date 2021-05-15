using System;
using System.Text;

namespace Denhub.API.Utils {
    public class PaginationUtils {
        /// <summary>
        /// Encodes an input string to Base64
        /// </summary>
        /// <param name="input">An input value</param>
        /// <returns>A Base64 string</returns>
        public static string ConvertToBase64(string input) {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }
        
        /// <summary>
        /// Decodes an input string from Base64
        /// </summary>
        /// <param name="input">An input value</param>
        /// <returns>A decoded Base64 string</returns>
        public static string ConvertFromBase64(string input) {
            return Encoding.UTF8.GetString(Convert.FromBase64String(input));
        }
    }
}