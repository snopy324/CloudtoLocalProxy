using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CloudtoLocalProxy
{
    public class ProxyMiddleware
    {
        private readonly RequestDelegate next;

        public ProxyMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, IHubContext<ProxyHub> hubContext)
        {
            var req = context.Request;

            var payload = new Payload
            {
                Query = req.Query,
                Headers = new Dictionary<string, string>(),
                Method = req.Method,
                Path = req.Path
            };

            using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8, true, 1024, true))
            {
                payload.Body = await reader.ReadToEndAsync();
            }

            foreach (var vs in req.Headers)
            {
                payload.Headers.Add(vs.Key, String.Join(";", vs.Value.ToArray()));
            }

            var message = JsonConvert.SerializeObject(payload, settings: new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            await hubContext.Clients.All.SendCoreAsync("Receive", new object[] { message });

            System.Console.WriteLine($"Form : {req.Host.Value} , Method : {req.Method} , Path : {req.Path}.");

            await next(context);
        }
    }

    internal class Payload
    {
        public IQueryCollection Query { get; set; }
        public string Body { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string Method { get; set; }
        public PathString Path { get; set; }
    }
}