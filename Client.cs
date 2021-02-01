using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HTTPClient
{
    public class Client
    {
        private static HttpClient httpClient;

        public Client() { }

        public HttpResponse Send(HttpRequestMessage httpRequest)
        {
            return SendAsyncInternal(httpRequest).Result;
        }

        public async Task<HttpResponse> SendAsync(HttpRequestMessage httpRequest)
        {
            return await SendAsyncInternal(httpRequest);
        }

        async Task<HttpResponse> SendAsyncInternal(HttpRequestMessage httpRequest)
        {
            try
            {
                if (httpClient is null) httpClient = new HttpClient();
                HttpResponse httpResponse = new HttpResponse();

                using (httpRequest)
                using (HttpResponseMessage response = await httpClient.SendAsync(httpRequest))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        byte[] responseContentBytes = await response.Content.ReadAsByteArrayAsync();
                        
                        responseContentBytes = DecompressResponse(responseContentBytes, response.Content.Headers.ContentEncoding.FirstOrDefault());
                        
                        httpResponse.ResponseMessage = Encoding.GetEncoding(response.Content.Headers.ContentType.CharSet).GetString(responseContentBytes, 0, responseContentBytes.Length);
                        httpResponse.HttpResponseMessage = response;
                    }
                }
                return httpResponse;
            }
            catch (Exception exception) { return new HttpResponse { Exception = exception }; }
        }

        private byte[] DecompressResponse(byte[] contentBytes, string contentEncoding)
        {
            try
            {
                byte[] result = null;

                if (contentBytes is null) return result;
                if (contentEncoding is null || contentEncoding == string.Empty) return contentBytes;

                using (MemoryStream compressedStream = new MemoryStream(contentBytes))
                using (MemoryStream resultStream = new MemoryStream())
                {
                    if (contentEncoding == "deflate")
                    {
                        using DeflateStream decompressedStream = new DeflateStream(compressedStream, CompressionMode.Decompress);
                        decompressedStream.CopyTo(resultStream);
                        goto Skip;
                    }

                    if (contentEncoding == "gzip")
                    {
                        using GZipStream decompressedStream = new GZipStream(compressedStream, CompressionMode.Decompress);
                        decompressedStream.CopyTo(resultStream);
                        goto Skip;
                    }

                    if (contentEncoding == "br")
                    {
                        using BrotliStream decompressedStream = new BrotliStream(compressedStream, CompressionMode.Decompress);
                        decompressedStream.CopyTo(resultStream);
                        goto Skip;
                    }

                Skip:
                    result = resultStream.ToArray();
                }

                return result;
            }
            catch { return contentBytes; }
        }
    }
}
