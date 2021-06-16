using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Sample8.Proxy
{
    public class ConnectorInputModel
    {
        public string Path { get; set; }
        public HttpMethod Method { get; set; } = HttpMethod.Get;
        public CookieCollection Cookies { get; set; }
        public AuthenticationHeaderValue Autorization { get; set; }
        public List<KeyValuePair<string, string>> Headers { get; set; }
        public object Data { get; set; }
        public string Query { get; set; }
    }

    public class FileModel
    {
        public byte[] Data { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }

    public class DownloadFileResponse
    {
        public DownloadFileResponse()
        {
            IsCreated = false;
        }
        public bool IsCreated { get; set; }
    }
}
