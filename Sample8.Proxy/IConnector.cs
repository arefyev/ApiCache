using Sample8.Common;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Sample8.Proxy
{
    public enum MethodType
    {
        POST,
        GET
    }
    public interface IConnector
    {
        Task<T> SimpleRequest<T>(MapAuthToken authToken, MapSerializeType serializeType, ContentType requestType, object data, string address, string path, bool needFormat = true, string query = null, CookieCollection cookies = null, string accessToken = null) where T : class;
        T Request<T>(MapAuthToken authToken, MapSerializeType serializeType, ContentType requestType, object data, string address, string path, bool needFormat = false, string query = null, CookieCollection cookies = null, string accessToken = null, MethodType type = MethodType.POST) where T : class;
        Task<T> RequestAsync<T>(MapAuthToken authToken, MapSerializeType serializeType, ContentType requestType, object data, string address, string path, bool needFormat = true, string query = null, CookieCollection cookies = null, string accessToken = null, MethodType type = MethodType.POST) where T : class;
        T DownloadFile<T>(MapAuthToken authToken, ContentType requestType, object data, string path, string resultFilePath, string query = null, CookieCollection cookies = null) where T : class;
        Task<T> Request<T>(string path,
            HttpMethod method,
            object data,
            bool needFormat = true,
            string query = null,
            CookieCollection cookies = null,
            AuthenticationHeaderValue autorization = null,
            List<KeyValuePair<string, string>> headers = null, bool resultFromHeader = false) where T : class;
        Task<FileModel> DownloadFileAsync(string path,
            string query = null,
            CookieCollection cookies = null,
            AuthenticationHeaderValue autorization = null,
            List<KeyValuePair<string, string>> headers = null);

        Task<HttpResponseHeaders> GetResponseHeaderAsync(ConnectorInputModel model);
    }
}
