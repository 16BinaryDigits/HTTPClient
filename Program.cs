using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;


namespace HTTPClient
{
    class Program
    {
        // Sites to test the decompression
        // https://www.microsoft.com/ -gzip
        // https://www.brotli.pro/ -br
        // https://www.Roblox.com -deflate

        static async Task Main(string[] args)
        {
            // Initialize client instance
            var client = new Client();

            // Setting up the request params
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://www.microsoft.com/");
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            // Get the Http response
            var response = await client.SendAsync(request);

            // Print the response -plus null check ... refer back to the HttpResponse model 
            if (response.HttpResponseMessage != null) Console.WriteLine(response.HttpResponseMessage);
            if (response.ResponseMessage != null) Console.WriteLine(response.ResponseMessage);
            if (response.Exception != null) Console.WriteLine(response.Exception.Message);

            Loop();
        }

        private static void Loop()
        {
            Console.ReadLine();
            Loop();
        }
    }
}
