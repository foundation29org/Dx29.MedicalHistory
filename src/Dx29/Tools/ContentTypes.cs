using System;
using System.IO;

namespace Dx29.Tools
{
    static public class ContentTypes
    {
        static public string FromFilename(string filename)
        {
            return FromExtension(Path.GetExtension(filename));
        }

        static public string FromExtension(string extension)
        {
            switch (extension?.ToLower())
            {
                case ".json":
                    return "application/json";
                case ".pdf":
                    return "application/pdf";
                case ".doc":
                    return "application/msword";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".txt":
                default:
                    return "text/plain";
            }
        }
    }
}
