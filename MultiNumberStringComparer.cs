using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IndoorCO2App_Android
{
    internal class MultiNumberStringComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == null || y == null) return 0;

            // Tokenize both strings into text and number parts
            var tokensX = Tokenize(x);
            var tokensY = Tokenize(y);

            // Compare token by token
            int tokenCount = Math.Min(tokensX.Count, tokensY.Count);
            for (int i = 0; i < tokenCount; i++)
            {
                int result = CompareTokens(tokensX[i], tokensY[i]);
                if (result != 0)
                    return result; // Return as soon as tokens differ
            }

            // If tokens are equal so far, the longer string is considered "greater"
            return tokensX.Count.CompareTo(tokensY.Count);
        }

        private List<string> Tokenize(string input)
        {
            // Split string into alternating text and number parts
            var tokens = new List<string>();
            foreach (Match match in Regex.Matches(input, @"\D+|\d+"))
            {
                tokens.Add(match.Value);
            }
            return tokens;
        }

        private int CompareTokens(string token1, string token2)
        {
            // Try to parse both tokens as numbers
            bool isNumber1 = int.TryParse(token1, out int num1);
            bool isNumber2 = int.TryParse(token2, out int num2);

            if (isNumber1 && isNumber2)
            {
                // Numeric comparison
                return num1.CompareTo(num2);
            }

            // Textual comparison
            return string.Compare(token1, token2, StringComparison.Ordinal);
        }
    }
}
