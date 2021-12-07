using System;
using System.IO;

namespace Sample
{
    static public class FConsole
    {
        static FConsole()
        {
            Filename = $"..\\..\\..\\_logs\\{DateTime.Now:yyMMdd-HHmmss}.log";
        }

        public static string Filename { get; }

        static public void WriteLine(string str)
        {
            using (var writer = new StreamWriter(Filename, true))
            {
                writer.WriteLine(str);
                Console.WriteLine(str);
            }
        }
    }
}
