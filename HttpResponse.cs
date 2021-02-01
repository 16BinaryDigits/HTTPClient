using System;
using System.Net.Http;

namespace HTTPClient
{
    public class HttpResponse
    {
        public HttpResponseMessage HttpResponseMessage { get; set; }

        public Exception Exception { get; set; }

        public string ResponseMessage { get; set; }
    }
}
