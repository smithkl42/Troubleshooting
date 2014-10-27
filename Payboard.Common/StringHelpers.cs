using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Payboard.Common
{
    public static class StringHelpers
    {
        /// <summary>
        ///     Regex to wrap a blank URL ("www.google.com" or "http://www.google.com") with an anchor tag
        /// </summary>
        private static readonly Regex wrapWithAnchor =
            new Regex(
                @"(?!(?!.*?<a)[^<]*<\/a>)(?:(?:https?|ftp|file)://|www\.|ftp\.)[-A-Z0-9+&#/%=~_|$?!:,.]*[A-Z0-9+&#/%=~_|$]",
                RegexOptions.IgnoreCase);

        /// <summary>
        ///     Regex to take a hostname within an anchor tag hef attribute and append a protocol to it.
        /// </summary>
        private static readonly Regex addProtocol =
            new Regex(@"(?<=href="")(?!http://|https://|ftp://)([A-Za-z0-9_=%&@\?\.\/\-]+)""");

        private static readonly Regex nonAlphaNumeric = new Regex("[^a-zA-Z0-9-]");

        private static readonly PluralizationService _pluralizationService =
            PluralizationService.CreateService(new CultureInfo("en-US"));

        private static readonly Regex _fromPascalCaseRe = new Regex("([A-Z]+[a-z]+)");

        /// <summary>
        ///     Avoids ArgumentOutOfRangeException errors, returning reasonable alternatives instead.
        /// </summary>
        public static string SubstringSafe(this string s, int start, int length)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            if (start > s.Length)
            {
                return string.Empty;
            }
            if (start + length > s.Length)
            {
                length = s.Length - start;
            }
            return s.Substring(start, length);
        }

        public static string ToStringSafe(this object s, string alternate = "")
        {
            if (s == null)
            {
                return alternate;
            }
            var result = s.ToString();
            if (string.IsNullOrWhiteSpace(result))
            {
                return alternate;
            }
            return result;
        }

        public static string TrimSafe(this string s)
        {
            if (s == null) return null;
            return s.Trim();
        }

        public static string ToLowerSafe(this string s)
        {
            if (s == null) return String.Empty;
            return s.ToLowerInvariant();
        }

        public static string StripStartingWith(this string s, string stripAfter)
        {
            if (s == null) return null;
            var indexOf = s.IndexOf(stripAfter, StringComparison.Ordinal);
            if (indexOf > -1)
            {
                return s.Substring(0, indexOf);
            }
            return s;
        }

        public static string AddHtmlParagraphs(this string s)
        {
            if (s == null)
            {
                s = String.Empty;
            }
            var paragraphs = s
                .Replace("\r", "")
                .Split('\n')
                .Select(ps => "<p>" + ps + "</p>\r\n");
            return string.Join("", paragraphs);
        }

        public static string FixUrls(this string s)
        {
            if (s == null)
            {
                return null;
            }
            s = wrapWithAnchor.Replace(s, "<a href=\"$0\">$0</a>");
            s = addProtocol.Replace(s, @"http://$1""");
            return s;
        }

        public static byte[] ToMd5Bytes(this string input)
        {
            // step 1, calculate MD5 hash from input
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);
            return hash;
        }

        public static string ToMd5(this string input)
        {
            // step 1, calculate MD5 hash from input
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new StringBuilder();
            foreach (var b in hash)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        public static string ToGravatarUrl(this string email, int size = 64)
        {
            var protocol = "";
            var ctx = HttpContext.Current;
            if (ctx != null)
            {
                protocol = ctx.Request.IsSecureConnection ? "https:" : "http:";
            }
            email = email ?? "";
            return protocol + "//secure.gravatar.com/avatar/" +
                   email.Trim().ToLowerInvariant().ToMd5() +
                   "?s=" + size +
                   "&d=" + protocol + "//app.payboard.com/Images/User1_64x64.png";
        }

        public static string ReplaceNonAlphaNumerics(this string s, string replacement = "_")
        {
            return nonAlphaNumeric.Replace(s, replacement);
        }

        public static string ReplaceControlChars(this string s, string replacement = "")
        {
            if (s == null) return null;
            return s
                .Replace("\n", String.Empty)
                .Replace("\r", String.Empty)
                .Replace("\t", String.Empty);
        }

        public static string Pluralize(this string s, int number)
        {
            return number == 1
                ? _pluralizationService.Singularize(s)
                : _pluralizationService.Pluralize(s);
        }

        public static string Pluralize(this int i, string s)
        {
            return i + " " + s.Pluralize(i);
        }

        public static bool IsValidEmail(this string email)
        {
            try
            {
                // ReSharper disable once UnusedVariable
                var ignore = new MailAddress(email);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///     Splits a string in pascal case to distinct words, e.g., "CustomerUserEvent" becomes "Customer User Event"
        /// </summary>
        public static string FromPascalCase(this string target)
        {
            return
                _fromPascalCaseRe.Replace(target, m => (m.Value.Length > 3 ? m.Value : m.Value.ToLower()) + " ").Trim();
        }

        public static string OrIfBlank(this string s, string alternate)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return alternate;
            }
            return s;
        }

        /// <summary>
        ///     Splits a string by top-level braces, i.e., "{FirstName {Bob}} {LastName {Smith} {SomethingElse}}"
        ///     will return ["{FirstName {Bob}}", "{LastName {Smith} {SomethingElse}}"]
        /// </summary>
        public static IList<string> SmartBraceSplit(this string s)
        {
            var splits = new List<string>();
            var depth = 0;
            var sb = new StringBuilder();
            foreach (var c in s)
            {
                sb.Append(c);
                if (c == '{')
                {
                    depth++;
                }
                else if (c == '}' && depth > 0)
                {
                    depth--;
                    if (depth == 0)
                    {
                        splits.Add(sb.ToString());
                        sb.Clear();
                    }
                }
            }
            if (sb.Length > 0)
            {
                splits.Add(sb.ToString());
            }
            return splits;
        }

        /// <summary>
        ///     HTMLDecodes everything that is enclosed in top-level braces.
        /// </summary>
        public static string HtmlDecodeInBraces(this string s)
        {
            // Turn "Something &lt; {&lt;Bob&gt;} &gt; Something Else"
            // into "Something &lt; {<Bob>} &gt; Something Else"
            var sb = new StringBuilder();
            var inBraces = new StringBuilder();
            var depth = 0;
            foreach (var c in s)
            {
                if (c == '{')
                {
                    inBraces.Append(c);
                    depth++;
                }
                else if (depth > 0)
                {
                    inBraces.Append(c);
                    if (c == '}')
                    {
                        depth--;
                        if (depth == 0)
                        {
                            var decoded = WebUtility.HtmlDecode(inBraces.ToString());
                            sb.Append(decoded);
                            inBraces.Clear();
                        }
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }
            sb.Append(inBraces);
            return sb.ToString();
        }

        public static bool ContainsCaseInsensitive(this string s, string substring)
        {
            if (s == null) return false;
            return s.IndexOf(substring, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static string[] Lines(this string source)
        {
            if (string.IsNullOrWhiteSpace(source)) return new string[] {};
            return source.Split(new[] {"\r\n", "\n"}, StringSplitOptions.None);
        }

        public static string RemoveFromStart(this string source, string toRemove)
        {
            if (source.StartsWith(toRemove))
            {
                return source.Substring(toRemove.Length, source.Length - toRemove.Length);
            }
            return source;
        }

        public static string RemoveFromEnd(this string source, string toRemove)
        {
            if (source.EndsWith(toRemove))
            {
                return source.Substring(0, source.Length - toRemove.Length);
            }
            return source;
        }

        /// <summary>
        /// Return a Uri if it can be created, or null if not.
        /// </summary>
        public static Uri ToUriSafe(this string url)
        {
            try
            {
                return new Uri(url);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string TrimNonAlphaNumeric(this string source)
        {
            return source.TrimStartNonAlphaNumeric().TrimEndNonAlphaNumeric();
        }

        public static string TrimStartNonAlphaNumeric(this string source)
        {
            for (var i = 0; i < source.Length; i++)
            {
                if (Char.IsLetterOrDigit(source[i]))
                {
                    return source.Substring(i, source.Length - i);
                }
            }
            return string.Empty;
        }

        public static string TrimEndNonAlphaNumeric(this string source)
        {
            for (var i = source.Length - 1; i >= 0; i--)
            {
                if (Char.IsLetterOrDigit(source[i]))
                {
                    return source.Substring(0, i + 1);
                }
            }
            return string.Empty;
        }
    }
}