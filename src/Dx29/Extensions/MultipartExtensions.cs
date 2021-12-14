using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Dx29
{
    static public class MultipartExtensions
    {
        static public void Add(this MultipartFormDataContent multipart, object obj, string name, string filename, string contentType)
        {
            Add(multipart, obj.Serialize(), name, filename, contentType);
        }
        static public void Add(this MultipartFormDataContent multipart, string body, string name, string filename, string contentType)
        {
            var content = new StringContent(body, Encoding.UTF8);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
            multipart.Add(content, name, filename);
        }
        static public void Add(this MultipartFormDataContent multipart, Stream stream, string name, string filename, string contentType)
        {
            var content = new StreamContent(stream);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
            multipart.Add(content, name, filename);
        }
    }
}
