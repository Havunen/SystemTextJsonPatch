using System.Text;

namespace SystemTextJsonPatch.Internal
{
    internal class StringUtil
    {
        internal static class Consts
        {
            // Constant for carriage return + line feed
            // System.Environment.NewLine is not constant and cannot be used in constant expression
            // also it returns \n in Mac and Unix environments
            public const string CrLf = "\r\n";

            public const char Cr = '\r';
            public const char Lf = '\n';
            public const char Tab = '\t';
            public const char Space = ' ';
            public const char VerticalTab = '\v';
            public const char Back = '\b';
            public const char NullChar = '\0';
            public const char FormFeed = '\f';
            public const char ByteOrderMark = '\uFEFF';
        }

        internal static string TrimWeirdCharacters(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            var resultBuilder = new StringBuilder(str.Length);
            int charCount = 0;

            foreach (char c in str)
            {
                switch (c)
                {
                    case Consts.Cr:
                    case Consts.Lf:
                    case Consts.Tab:
                    case Consts.Space:
                    case Consts.VerticalTab:
                    case Consts.Back:
                    case Consts.NullChar:
                    case Consts.FormFeed:
                    case Consts.ByteOrderMark:
                        continue;
                    default:
                        resultBuilder.Append(c);
                        charCount++;
                        continue;
                }
            }

            return charCount == 0 ? string.Empty : resultBuilder.ToString(0, charCount);
        }
    }
}
