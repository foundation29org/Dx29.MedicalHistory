using System;
using System.Text;

namespace Dx29.Tools
{
    static public class Lorem
    {
#if  DEBUG
        static private Random _random = new Random(1234);
#else
        static private Random _random = new Random();
#endif

        static public string Generate(int min, int max)
        {
            return Generate(_random.Next(min, max));
        }

        static public string Generate(int count)
        {
            var sb = new StringBuilder();
            bool norepeat = count < _words.Length / 2;

            for (int n = 0; n < count; n++)
            {
                int index = _random.Next(0, _words.Length);
                if (norepeat)
                {
                    while (sb.ToString().Contains(_words[index] + " "))
                    {
                        index = _random.Next(0, _words.Length);
                    }
                }
                sb.Append(_words[index] + " ");
            }

            string str = sb.ToString();
            return str.Substring(0, 1).ToUpper() + str.Substring(1).Trim() + ".";
        }

        static readonly string[] _words = new string[] {
            "lorem",
            "ipsum",
            "dolor",
            "sit",
            "amet",
            "consectetur",
            "adipiscing",
            "elit",
            "phasellus",
            "orci",
            "velit",
            "dignissim",
            "nec",
            "sagittis",
            "a",
            "bibendum",
            "vel",
            "justo",
            "duis",
            "vitae",
            "odio",
            "eu",
            "metus",
            "molestie",
            "placerat",
            "in",
            "quis",
            "nisl",
            "nunc",
            "eget",
            "leo",
            "et",
            "scelerisque",
            "ultricies",
            "ac",
            "dui",
            "faucibus",
            "ullamcorper",
            "ut",
            "libero",
            "non",
            "volutpat",
            "lobortis",
            "mauris",
            "donec",
            "egestas",
            "quam",
            "pharetra",
            "congue",
            "turpis",
            "tempor",
            "morbi",
            "risus",
            "lacinia",
            "sollicitudin",
            "purus",
            "ligula",
            "est",
            "etiam",
            "eros",
            "proin",
            "lectus",
            "suscipit",
            "id",
            "facilisis",
            "laoreet",
            "ultrices",
            "arcu",
            "fermentum",
            "tortor",
            "tempus"
        };
    }
}
