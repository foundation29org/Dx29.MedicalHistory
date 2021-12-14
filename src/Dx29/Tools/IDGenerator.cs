using System;

namespace Dx29.Tools
{
    static public class IDGenerator
    {
        static private Random _random = new Random();

        static public string GenerateID(char prefix = 'u')
        {
            var date = DateTimeOffset.UtcNow;
            return $"{prefix}{date:yyMMddHHmmssffff}{_random.Next(0, 9999999):0000}";
        }

        static public string GenerateToken()
        {
            DateTimeOffset date = DateTimeOffset.UtcNow;
            return $"{date:yyyy-MM-dd-HHmmssff}-{Guid.NewGuid().ToString().Substring(0, 8)}";
        }

        static public string GenerateFilename(string extension = null)
        {
            extension = extension == null ? "" : $".{extension.Trim('.')}";
            return $"{GenerateID('f')}{extension}";
        }

        static public string GenerateGuid()
        {
            return $"{Guid.NewGuid():d}";
        }
    }
}
