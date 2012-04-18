using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TemplatingExtensions.Utilities
{
    public static class TransliterationUtils
    {
        private static readonly Dictionary<char, string> TransliterationMap = 
            new Dictionary<char, string>
            {
                {'а',"a"}, {'А',"A"},
                {'б',"b"}, {'Б',"B"},
                {'в',"v"}, {'В',"V"},
                {'г',"g"}, {'Г',"G"},
                {'д',"d"}, {'Д',"D"},
                {'е',"e"}, {'Е',"E"},
                {'ё',"yo"}, {'Ё',"Yo"},
                {'ж',"zh"}, {'Ж',"Zh"},
                {'з',"z"}, {'З',"Z"},
                {'и',"i"}, {'И',"I"},
                {'й',"y"}, {'Й',"Y"},
                {'к',"k"}, {'К',"K"},
                {'л',"l"}, {'Л',"L"},
                {'м',"m"}, {'М',"M"},
                {'н',"n"}, {'Н',"N"},
                {'о',"o"}, {'О',"O"},
                {'п',"p"}, {'П',"P"},
                {'р',"r"}, {'Р',"R"},
                {'с',"s"}, {'С',"S"},
                {'т',"t"}, {'Т',"T"},
                {'у',"u"}, {'У',"U"},
                {'ф',"f"}, {'Ф',"F"},
                {'х',"kh"}, {'Х',"Kh"},
                {'ц',"ts"}, {'Ц',"Ts"},
                {'ч',"ch"}, {'Ч',"Ch"},
                {'ш',"sh"}, {'Ш',"Sh"},
                {'щ',"shch"}, {'Щ',"Shch"},
                {'ъ',""}, {'Ъ',""},
                {'ы',"y"}, {'Ы',"Y"},
                {'ь',""}, {'Ь',""},
                {'э',"e"}, {'Э',"E"},
                {'ю',"yu"}, {'Ю',"Yu"},
                {'я',"ya"}, {'Я',"Ya"}
            };

        public static string TransliterateCyrillicToLatin(string str)
        {
            char[] source = str.ToCharArray();

            return source.Aggregate("", (current, t) => current + Transliterate(t));
        }

        private static string Transliterate(char c)
        {
            try
            {
                return TransliterationMap[c];
            }
            catch
            {
                //we don't replace anything
                return c.ToString();
            }
        }

        public static string RemoveDiacritics(string stIn)
        {
            string stFormD = stIn.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char t in stFormD)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(t);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(t);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }
    }
}
