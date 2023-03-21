using System;
using System.Net.Http;

namespace dependency_injection_azure_function
{
    public class MyService : IMyService
    {
        private IHttpClientFactory _httpClientFactory;

        public MyService (IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public void DoWork ()
        {
            Console.WriteLine("I am doing work");
        }
    }
}