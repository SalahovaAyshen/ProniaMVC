using System.Text;

namespace FrontToBack_Pronia.Utilities
{
    public static class StringFormat
    {
        public static string Capitalize(this string word)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Char.ToUpper(word[0]));

            for (int i = 1; i < word.Length; i++)
            {
                sb.Append(Char.ToLower(word[i]));
            }
            return sb.ToString();
        }

        public static bool Check(this string word)
        {
            int count = 0;
            for (int i = 0; i < word.Length; i++)
            {
                if (!Char.IsDigit(word[i])) count++;
            }
            if (count != word.Length - 1)
            {
                return false;
            }
           return true;
            
        }
    }
}
