using System.Text.RegularExpressions;

namespace TemplatingExtensions.Utilities
{
    /// <summary>
    /// Use these functions to decode JS code blocks
    /// Should keep stuff that needs to be decoded decoded. 
    /// Also note that this is a convenient method to replace unicode codepoints, should you want it.
    /// 
    /// IE, do not use HttpUtility.HtmlDecode or other similar methods
    /// </summary>
    public class JSEncodingUtils
    {
        public static string HtmlEntitiesDecode(string input)
        {
            input = Regex.Replace(input, "&gt;", ">");
            input = Regex.Replace(input, "&lt;", "<");
            input = Regex.Replace(input, "&quot;", "\"");
            input = Regex.Replace(input, "&apos;", "'");
            input = Regex.Replace(input, "&#x22;", "\"");
            return input;
        }

        public static string HtmlEntitiesEncode(string input)
        {
            input = Regex.Replace(input, ">", "&gt;");
            input = Regex.Replace(input, "<", "&lt;");
            input = Regex.Replace(input, "\"", "&quot;");
            input = Regex.Replace(input, "'", "&apos;");
            input = Regex.Replace(input, "\"", "&#x22;");
            return input;
        }

        public static string XmlEntitiesEncode(string input)
        {
            input = Regex.Replace(input, "&", "&amp;");
            return input;
        }
    }
}
