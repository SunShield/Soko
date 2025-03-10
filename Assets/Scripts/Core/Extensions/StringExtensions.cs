using System.Collections.Generic;

namespace Soko.Core.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns list of substrings enclosed in <openSeparator> and <closeSeparator>.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="openSeparator"></param>
        /// <param name="closeSeparator"></param>
        /// <returns></returns>
        public static List<string> SplitByTwoSeparators(this string text, char openSeparator, char closeSeparator)
        {
            var openSeparatorFound = false;
            var result = new List<string>();
            var currentResult = "";
            for (int i = 0; i < text.Length; i++)
            {
                var currentSymbol = text[i];
                if (!openSeparatorFound)
                {
                    if (currentSymbol != openSeparator) continue;
                    openSeparatorFound = true;
                    continue;
                }

                if (openSeparatorFound)
                {
                    if (currentSymbol != closeSeparator)
                        currentResult += currentSymbol;
                    else
                    {
                        openSeparatorFound = false;
                        result.Add(currentResult);
                        currentResult = "";
                    }
                }
            }

            return result;
        }
    }
}