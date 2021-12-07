using System;
using System.Collections.Generic;

namespace Dx29.Tools
{
    static public class Randoms
    {

#if  DEBUG
        static private Random _random = new Random(1234);
#else
        static private Random _random = new Random();
#endif

        static public string Number(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue).ToString();
        }

        static public string Select(params string[] items)
        {
            return items[_random.Next(items.Length)];
        }
    }
}
