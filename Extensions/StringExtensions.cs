using System.Collections.Generic;

namespace CBLib
{
    public static class StringExtensions
    {
        public static void ParsingStringSplit(this List<string> list, string input, char separator)
        {
            if (string.IsNullOrEmpty(input)) return;
            
            string[] parts = input.Split(separator);
            foreach (string part in parts)
            {
                string trimmed = part.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                {
                    list.Add(trimmed);
                }
            }
        }
    }
}
