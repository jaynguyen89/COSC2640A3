using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Helper.Shared;
using Newtonsoft.Json;

namespace Helper {

    public static class Helpers {

        public static byte[] EncodeDataUtf8([NotNull] this object data) {
            var serializedData = JsonConvert.SerializeObject(data);
            return Encoding.UTF8.GetBytes(serializedData);
        }

        public static T DecodeUtf8<T>([NotNull] this byte[] data) {
            var plainData = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<T>(plainData);
        }
        
        public static bool IsProperString([AllowNull] string any) {
            return !string.IsNullOrEmpty(any) && !string.IsNullOrWhiteSpace(any);
        }
        
        public static string RemoveAllSpaces(this string any) {
            return Regex.Replace(any, SharedConstants.MultiSpace, string.Empty);
        }

        // Splits a Camel-Case string at capital letters and returns spaced string.
        public static string ToHumanStyled(this string any) {
            return string.Join(
                SharedConstants.MonoSpace,
                Regex.Split(any, @"(?<!^)(?=[A-Z])")
            );
        }

        public static int GetRandomNumberInRangeInclusive([NotNull] int max,[NotNull] int min = 0) {
            var random = new Random();
            return random.Next(min, max + 1);
        }
        
        public static string GenerateRandomString([NotNull] int length = 8,[NotNull] bool caseSensitive = false,[NotNull] bool includeSpecialChars = false) {
            const string sChars = "QWERTYUIOPASDFGHJKKLZXCVBNMqwertyuiopasdfghjklzxcvbnmn1234567890!@#$%&_+.";
            const string nChars = "QWERTYUIOPASDFGHJKKLZXCVBNMqwertyuiopasdfghjklzxcvbnmn1234567890";
            const string scChars = "QWERTYUIOPASDFGHJKKLZXCVBNM1234567890!@#$%&_+.";
            const string ncChars = "QWERTYUIOPASDFGHJKKLZXCVBNM1234567890";
            
            var charSetToUse = caseSensitive
                ? (includeSpecialChars ? sChars : nChars)
                : (includeSpecialChars ? scChars : ncChars);
            
            var randomString = new string(
                Enumerable.Repeat(charSetToUse, length)
                          .Select(p => p[(new Random()).Next(p.Length)])
                          .ToArray()
            );

            return randomString;
        }

        public static string HideEmailPartial(this string email) {
            var emailTokens = email.Split("@");
            return $"{ emailTokens[0][..2] }***{ emailTokens[0][^2..] }@{ emailTokens[^1] }";
        }
        
        public static DateTime GetRandomDateTime(DateTime from, DateTime to) {
            var range = to - from;
            var randTimeSpan = new TimeSpan((long)((new Random()).NextDouble() * range.Ticks)); 

            return from + randTimeSpan;
        }
    }
}
