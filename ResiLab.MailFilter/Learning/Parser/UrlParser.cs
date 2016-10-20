using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ResiLab.MailFilter.Learning.Parser {
    public class UrlParser {
        private static readonly Regex Pattern =
            new Regex(
                "http(s)?://([\\w+?\\.\\w+])?([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        ///     Parse the text and return the found urls whith the http or https protocol.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IEnumerable<string> Parse(string text) {
            if (text == null) {
                return new string[0];
            }

            var result = new List<string>();
            var matches = Pattern.Matches(text);

            foreach (var match in matches) {
                result.Add(match.ToString());
            }

            return result;
        }
    }
}