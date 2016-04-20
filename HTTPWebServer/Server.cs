using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Threading;
using HTTPWebServer.Infrastructure;

namespace HTTPWebServer
{
    public class Server
    {
        private static readonly string Directory = LoadWorkDirectory();
        private HttpListener _listener;
        private static Dictionary<string, string> _pluginsSection;

        public Server()
        {
            LoadConfigSection();
        }

        public void Start(string[] prefixes)
        {
            _listener = new HttpListener();
            foreach (var item in prefixes)
            {
                _listener.Prefixes.Add(item);

                Console.WriteLine("Listening \t" + item);
            }
            Console.WriteLine();

            _listener.Start();
            do
            {
                // Note: The GetContext method blocks while waiting for a request. 
                var context = _listener.GetContext();
                ThreadPool.QueueUserWorkItem(Respond, context);
            }
            while (true);
        }

        private static void LoadConfigSection()
        {
            _pluginsSection = new Dictionary<string, string>();
            var config = System.Configuration.ConfigurationManager.GetSection("Plugins") as PluginsSection;
            if (config == null) return;
           
            foreach (Plugin item in config.Plugins)
            {
                _pluginsSection.Add(item.Extention, item.ClassType);
            }
        }

        private static bool UrlExist(string url)
        {
            return File.Exists(url);
        }

        /// <summary>
        /// Replase all '/' with '\' in URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns>String, which could be used as file path</returns>
        private string UrlDecode(string url)
        {
            return (Directory + HttpUtility.UrlDecode(url)).Replace('/', '\\');
        }

        private void Respond(object listenerContext)
        {
            var context = (HttpListenerContext)listenerContext;
            var requestAdress = UrlDecode(context.Request.RawUrl);
            Console.WriteLine("Processing {0}", HttpUtility.UrlDecode(context.Request.RawUrl));
            //Process custom section from config
            var extention = Path.GetExtension(requestAdress);

            if (extention != null && _pluginsSection.ContainsKey(extention))
            {
                //creates an instance of class from config to process custom logic
                var classInstance = Type.GetType(_pluginsSection[extention]);
                if (classInstance != null)
                {
                    ((IRequestHandler)Activator.CreateInstance(classInstance)).HandleRequest(context);
                    return;
                }
            }

            // Not found
            if (!UrlExist(requestAdress))
            {
                Respond404(context.Response);
                return;
            }

            // OK
            Respond200(context.Response, requestAdress);
            context.Response.Close();
        }

        private static string LoadWorkDirectory()
        {
            var appSettings = System.Configuration.ConfigurationManager.AppSettings;
            if (!(appSettings.Count == 0))
            {
                return appSettings["Directory"];
            }
            else
            {
                throw new ArgumentException("Cannot load work directory, check configuration file");
            }
        }


        private string GetMimeType(string extention)
        {
            if (MimeType.MimeTypes.ContainsKey(extention))
                return MimeType.MimeTypes[extention];
            else
                return "application/octet-stream";
        }

        private void Respond404(HttpListenerResponse response)
        {
            response.StatusCode = 404;
            var htmlResponseBody = Encoding.UTF8.GetBytes(String.Format("<HTML><BODY><h1>{0}</h1><h2>{1}</h2></BODY><HEAD>",
                response.StatusCode, response.StatusDescription));

            response.ContentEncoding = new UTF8Encoding();
            response.ContentType = GetMimeType(".html");
            response.ContentLength64 = htmlResponseBody.Length;
            response.OutputStream.Write(htmlResponseBody, 0, htmlResponseBody.Length);
            response.OutputStream.Close();
        }

        private void Respond500(HttpListenerResponse response)
        {
            response.StatusCode = 500;
            var htmlResponseBody = Encoding.UTF8.GetBytes(String.Format("<HTML><BODY><h1>{0}</h1><h2>{1}</h2></BODY><HEAD>",
                response.StatusCode, response.StatusDescription));
            //in case if you already have not submit this data
            if (response.ContentLength64 == 0)
            {
                response.ContentLength64 = htmlResponseBody.Length;
                response.OutputStream.Write(htmlResponseBody, 0, htmlResponseBody.Length);
            }
        }

        private void Respond200(HttpListenerResponse response, string filePath)
        {
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    response.StatusCode = 200;
                    response.ContentEncoding = new UTF8Encoding();
                    response.ContentType = GetMimeType(Path.GetExtension(filePath));
                    response.ContentLength64 = (new FileInfo(filePath)).Length;
                    fileStream.CopyTo(response.OutputStream);
                    var lastModified = File.GetLastWriteTimeUtc(filePath).ToString(CultureInfo.InvariantCulture);
                    response.Headers.Add("Last-Modified", lastModified);
                    response.Headers.Add("ETag", String.Format("\"{0}\"", lastModified.GetHashCode()));
                    response.Headers.Add("Server", "trainee test server");
                }
            }
            catch
            {
                Respond500(response);
            }
        }
    }
}
