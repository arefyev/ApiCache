using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Sample8.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Sample8.Proxy
{

    public class NetConnector : IConnector
    {
        #region - Members -
        string _address;
        private const string DateFormat = "yyyy-MM-dd HH:mm:ss";
        private const int WebRequestTimeout = 24 * 60 * 60 * 1000; // 24 часа
        private const int WebResponseTimeout = 24; // 24 часа
        readonly IsoDateTimeConverter _dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = DateFormat };
        #endregion

        public NetConnector() { }

        private MethodType? Type { get; set; }

        public CookieCollection ResponseCookies { get; protected set; }
        public async Task<T> SimpleRequest<T>(MapAuthToken authToken, MapSerializeType serializeType, ContentType requestType, object data, string address, string path, bool needFormat = true, string query = null, CookieCollection cookies = null, string accessToken = null) where T : class
        {
            if (!String.IsNullOrEmpty(address))
                _address = address;

            var uriBuilder = new UriBuilder(_address) { Path = path, Query = query };
            var httpWebRequest = PrepareHttpWebRequest(uriBuilder, null, cookies, accessToken, requestType, authToken, MethodType.POST);

            try
            {
                using (var httpResponse = (HttpWebResponse)(await httpWebRequest.GetResponseAsync()))
                {
                    FixCookies(httpResponse);
                    ResponseCookies = httpResponse.Cookies;

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var resp = ProcessDeserialize<T>(streamReader, serializeType);

                        return resp;
                    }
                }
            }
            catch (WebException ex)
            {
                string resp = "";
                if (ex.Response != null)
                {
                    using (var streamReader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        resp = streamReader.ReadToEnd();
                    }
                }

                // Logger.Error(this, String.Format("Exception {0}", resp));
                //throw;
                return null;
            }
        }

        /// <summary>
        /// Производит поиск и возвращает структурированный набор данных
        /// </summary>
        /// <typeparam name="T">Тип возвращаемых данных</typeparam>
        /// <param name="data">Модель данных</param>
        /// <param name="path">Путь запроса</param>
        /// <returns></returns>
        public T Request<T>(MapAuthToken authToken, MapSerializeType serializeType, ContentType requestType, object data, string address, string path, bool needFormat = true, string query = null, CookieCollection cookies = null, string accessToken = null, MethodType type = MethodType.POST) where T : class
        {
            var request = PrepareData(data, serializeType, needFormat).Result;

            if (!String.IsNullOrEmpty(address))
                _address = address;

            var uriBuilder = new UriBuilder(_address) { Path = path, Query = query };
            var httpWebRequest = PrepareHttpWebRequest(uriBuilder, request, cookies, accessToken, requestType, authToken, type);

            try
            {
                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    FixCookies(httpResponse);
                    ResponseCookies = httpResponse.Cookies;

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var resp = ProcessDeserialize<T>(streamReader, serializeType);

                        return resp;
                    }
                }
            }
            catch (WebException ex)
            {
                string resp;
                if (ex.Response != null)
                {
                    using (var streamReader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        resp = streamReader.ReadToEnd();
                    }
                }

                throw;
            }
        }

        public async Task<T> RequestAsync<T>(MapAuthToken authToken, MapSerializeType serializeType, ContentType requestType, object data, string address, string path, bool needFormat = true, string query = null, CookieCollection cookies = null, string accessToken = null, MethodType type = MethodType.POST) where T : class
        {
            var request = await PrepareData(data, serializeType, needFormat);

            if (!String.IsNullOrEmpty(address))
                _address = address;

            var uriBuilder = new UriBuilder(_address) { Path = path, Query = query };
            var httpWebRequest = PrepareHttpWebRequest(uriBuilder, request, cookies, accessToken, requestType, authToken, type);

            try
            {
                using (var httpResponse = (HttpWebResponse)(await httpWebRequest.GetResponseAsync()))
                {
                    FixCookies(httpResponse);
                    ResponseCookies = httpResponse.Cookies;

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var resp = ProcessDeserialize<T>(streamReader, serializeType);

                        return resp;
                    }
                }
            }
            catch (WebException ex)
            {
                string resp = "";
                if (ex.Response != null)
                {
                    using (var streamReader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        resp = streamReader.ReadToEnd();
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Производит скачивание файла
        /// </summary>
        /// <param name="data">Модель данных для передачи в iaMap</param>
        /// <param name="path">Путь запроса</param>
        /// <returns></returns>
        public T DownloadFile<T>(MapAuthToken authToken, ContentType requestType, object data, string path, string resultFilePath, string query = null, CookieCollection cookies = null) where T : class
        {
            var buffer = new byte[4096];
            var responseResult = new DownloadFileResponse();
            var request = PrepareData(data, MapSerializeType.Json, true).Result;
            var uriBuilder = new UriBuilder(_address) { Path = path, Query = query };
            var httpWebRequest = PrepareHttpWebRequest(uriBuilder, request, cookies, String.Empty, requestType, authToken);

            try
            {
                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    FixCookies(httpResponse);
                    ResponseCookies = httpResponse.Cookies;

                    using (var fileStream = new FileStream(resultFilePath, FileMode.OpenOrCreate))
                    {
                        using (var responseStream = httpResponse.GetResponseStream())
                        {
                            if (responseStream != null)
                            {
                                int bytesRead;
                                while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    fileStream.Write(buffer, 0, bytesRead);
                                }
                                responseResult.IsCreated = true;
                            }
                        }
                    }

                    return responseResult as T;
                }
            }
            catch (Exception ex)
            {
                var message = string.Empty;

                if (ex is WebException wex)
                {
                    if (wex.Response != null)
                    {
                        using (var streamReader = new StreamReader(wex.Response.GetResponseStream()))
                        {
                            message = streamReader.ReadToEnd();
                        }
                    }
                }
                else
                {
                    message = ex.Message;
                }


                if (File.Exists(resultFilePath)) File.Delete(resultFilePath);
                throw;
            }
        }

        public async Task<T> Request<T>(string path,
            HttpMethod method,
            object data,
            bool needFormat = true,
            string query = null,
            CookieCollection cookies = null,
            AuthenticationHeaderValue autorization = null,
            List<KeyValuePair<string, string>> headers = null,
            bool resultFromHeader = false) where T : class
        {
            var request = PrepareData(data, MapSerializeType.Json, needFormat).Result;
            var stringContent = new StringContent(request, Encoding.UTF8, "application/json");

            var uriBuilder = new UriBuilder(_address) { Path = path, Query = query };

            var httpRequest = new HttpRequestMessage(method, uriBuilder.Uri)
            {
                Content = stringContent
            };
            if (autorization != null)
            {
                httpRequest.Headers.Authorization = autorization;
            }
            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    httpRequest.Headers.Add(header.Key, header.Value);
                }
            }

            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (x, c, r, t) => { return true; },
                AutomaticDecompression = DecompressionMethods.GZip,
                Proxy = null //JIRA-7695
            };
            if (cookies != null)
            {
                handler.CookieContainer = new CookieContainer();
                handler.CookieContainer.Add(cookies);
            }

            //var serializer = new JsonSerializer();
            //serializer.Converters.Add(_dateTimeConverter);
            using (var client = new HttpClient(handler))
            {
                client.Timeout = new TimeSpan(WebResponseTimeout, 0, 0);
                try
                {
                    var response = await client.SendAsync(httpRequest);

                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        using (var streamReader = new StreamReader(await response.Content.ReadAsStreamAsync()))
                        {
                            throw new Exception((string)ProcessDeserialize<object>(streamReader, MapSerializeType.Json));
                        }
                    }

                    if (resultFromHeader)
                    {
                        return response.Headers.Location.AbsolutePath as T;
                    }

                    using (var streamReader = new StreamReader(await response.Content.ReadAsStreamAsync()))
                    {
                        return ProcessDeserialize<T>(streamReader, MapSerializeType.Json);
                    }
                }
                catch (HttpRequestException ex)
                {
                    //log error
                    throw;
                }
            }
        }

        public async Task<HttpResponseHeaders> GetResponseHeaderAsync(ConnectorInputModel model)
        {
            var response = await GetHttpResponseAsync(model);
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                return response.Headers;
            }
            return null;
        }

        public async Task<FileModel> DownloadFileAsync(string path,
            string query = null,
            CookieCollection cookies = null,
            AuthenticationHeaderValue autorization = null,
            List<KeyValuePair<string, string>> headers = null)
        {
            var uriBuilder = new UriBuilder(_address) { Path = path, Query = query };
            //_logger.LogInformation($"start connector path: {uriBuilder}");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

            if (autorization != null)
            {
                httpRequest.Headers.Authorization = autorization;
            }

            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    httpRequest.Headers.Add(header.Key, header.Value);
                }
            }

            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (x, c, r, t) => { return true; },
                AutomaticDecompression = DecompressionMethods.GZip,
                Proxy = null //JIRA-7695
            };
            if (cookies != null)
            {
                handler.CookieContainer = new CookieContainer();
                handler.CookieContainer.Add(cookies);
            }

            using (var client = new HttpClient(handler))
            {
                client.Timeout = new TimeSpan(WebResponseTimeout, 0, 0);
                try
                {
                    using (var result = await client.SendAsync(httpRequest).ConfigureAwait(false))
                    {
                        if (result.IsSuccessStatusCode)
                        {
                            var fileBytes = await result.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                            return new FileModel
                            {
                                ContentType = result.Content.Headers.ContentType?.MediaType,
                                FileName = result.Content.Headers.ContentDisposition?.FileName,
                                Data = fileBytes
                            };
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    throw;
                }
            }
            return null;
        }

        private async Task<string> PrepareData(object data, MapSerializeType type, bool needFormat)
        {
            var request = string.Empty;
            if (data == null)
                return request;

            switch (type)
            {
                case MapSerializeType.Form:
                    request = await (new FormUrlEncodedContent(FormSerializer.ToKeyValue(data))).ReadAsStringAsync();
                    break;

                case MapSerializeType.Xml:

                    try
                    {
                        var xmlserializer = new XmlSerializer(data.GetType());
                        var stringWriter = new StringWriter();
                        using (var writer = XmlWriter.Create(stringWriter))
                        {
                            xmlserializer.Serialize(writer, data);
                            request = stringWriter.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        //log
                    }

                    break;

                case MapSerializeType.Json:
                default:
                    request = JsonConvert.SerializeObject(data,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        ContractResolver = needFormat ? new CamelCasePropertyNamesContractResolver() : new DefaultContractResolver()
                    });
                    break;
            }

            return request;
        }

        private HttpWebRequest PrepareHttpWebRequest(UriBuilder uriBuilder, string request, CookieCollection cookies, string accessToken, ContentType requestType, MapAuthToken tokenType, MethodType? mType = null)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Uri.UnescapeDataString(uriBuilder.Uri.AbsoluteUri));
            httpWebRequest.ContentType = requestType.ToValue();
            if (mType != null)
                httpWebRequest.Method = mType.ToString();
            else
                httpWebRequest.Method = Type == null ? "POST" : Type.ToString();

            if (tokenType != MapAuthToken.None)
            {
                switch (tokenType)
                {
                    case MapAuthToken.Bearer:
                        httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + accessToken);
                        break;

                    case MapAuthToken.Token:
                        httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Token " + accessToken);
                        break;

                    default:
                    case MapAuthToken.Basic:
                        httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Basic " + accessToken);
                        break;
                }
            }

            httpWebRequest.Timeout = WebRequestTimeout;
            httpWebRequest.AutomaticDecompression = DecompressionMethods.None;
            httpWebRequest.Proxy = null; // JIRA-7695

            if (cookies != null)
            {
                httpWebRequest.CookieContainer = new CookieContainer();
                httpWebRequest.CookieContainer.Add(cookies);
            }

            if (!string.IsNullOrEmpty(request))
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(request);
                }
            }

            return httpWebRequest;
        }

        private void FixCookies(HttpWebResponse httpResponse)
        {
            var uri = new Uri(_address);
            var domain = uri.Host;
            for (int i = 0; i < httpResponse.Headers.Count; i++)
            {
                string name = httpResponse.Headers.GetKey(i);
                string value = httpResponse.Headers.Get(i);
                if (name == "Set-Cookie")
                {
                    Match match = Regex.Match(value, "(.+?)=(.+?);");
                    if (match.Captures.Count > 0)
                    {
                        httpResponse.Cookies.Add(new Cookie(match.Groups[1].Value, match.Groups[2].Value, "/", domain));
                    }
                }
            }
        }

        private T ProcessDeserialize<T>(StreamReader stream, MapSerializeType serializeType) where T : class
        {
            if (typeof(T) == typeof(String))
                return stream.ReadToEnd() as T;

            switch (serializeType)
            {
                case MapSerializeType.Xml:
                    var xml = stream.ReadToEnd();
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    using (TextReader reader = new StringReader(xml))
                    {
                        return (T)xs.Deserialize(reader);
                    }
                    break;
                default:
                case MapSerializeType.Json:
                    var serializer = new JsonSerializer();
                    serializer.Converters.Add(_dateTimeConverter);
                    using (var reader = new JsonTextReader(stream))
                    {
                        return serializer.Deserialize<T>(reader);
                    }
                    break;
            }
        }

        private HttpRequestMessage CreateHttpRequest(ConnectorInputModel model)
        {
            if (model == null) return null;

            var request = PrepareData(model.Data, MapSerializeType.Json, true).Result;
            var stringContent = new StringContent(request, Encoding.UTF8, "application/json");

            var uriBuilder = new UriBuilder(_address) { Path = model.Path, Query = model.Query };

            var result = new HttpRequestMessage(model.Method ?? HttpMethod.Get, uriBuilder.Uri)
            {
                Content = stringContent
            };

            if (model.Autorization != null)
            {
                result.Headers.Authorization = model.Autorization;
            }
            if (model.Headers != null && model.Headers.Count > 0)
            {
                foreach (var header in model.Headers)
                {
                    result.Headers.Add(header.Key, header.Value);
                }
            }

            //_logger.LogInformation($"start request path: {uriBuilder}, request: {request}");
            return result;
        }

        private async Task<HttpResponseMessage> GetHttpResponseAsync(ConnectorInputModel model)
        {
            var httpRequest = CreateHttpRequest(model);
            if (httpRequest == null) return null;

            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (x, c, r, t) => { return true; },
                AutomaticDecompression = DecompressionMethods.GZip,
                Proxy = null //JIRA-7695
            };
            if (model.Cookies != null)
            {
                handler.CookieContainer = new CookieContainer();
                handler.CookieContainer.Add(model.Cookies);
            }
            using (var client = new HttpClient(handler))
            {
                client.Timeout = new TimeSpan(WebResponseTimeout, 0, 0);
                try
                {
                    var response = await client.SendAsync(httpRequest);
                    //_logger.LogInformation($"end request");
                    return response;
                }
                catch (HttpRequestException ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }
            }
        }
    }

    public enum MapSerializeType
    {
        Json,
        Form,
        Xml
    }

    public enum MapAuthToken
    {
        Token,
        Bearer,
        Basic,
        None
    }

    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding { get { return Encoding.UTF8; } }
    }
}
