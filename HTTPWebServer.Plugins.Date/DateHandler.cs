using System;
using System.Text;
using System.Net;
using HTTPWebServer.Infrastructure;

namespace HTTPWebServer.Plugins.Date
{
    public class DateHandler : IRequestHandler
    {
        public void HandleRequest(HttpListenerContext context)
        {
            string result;
            var response = context.Response;
            switch (context.Request.RawUrl.Replace("/", ""))
            {
                case "now.date": 
                    result = DateTime.Now.ToLongDateString();
                    break;
                case "tomorrow.date":
                    result = DateTime.Now.AddDays(1).ToLongDateString();
                    break;
                case "yesterday.date":
                    result = DateTime.Now.AddDays(-1).ToLongDateString();
                    break;
                default:
                    result = DateTime.Now.ToLongDateString();
                    break;
            }
            response.StatusCode = 200;
            response.ContentEncoding = new UTF8Encoding();
            response.ContentType = "text/html";

            var htmlResponseBody = Encoding.UTF8.GetBytes(String.Format("<HTML><BODY><h1>{0}\t{1}</h1><h2>{2}</h2></BODY><HEAD>",
                response.StatusCode, response.StatusDescription, result));

            response.ContentLength64 = htmlResponseBody.Length;
            response.OutputStream.Write(htmlResponseBody, 0, htmlResponseBody.Length);
            response.OutputStream.Close();            
        }
    }
}
