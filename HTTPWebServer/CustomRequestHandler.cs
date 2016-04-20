using System;

namespace HTTPWebServer
{
    public class CustomRequestHandler
    {
        public static string Date(string url)
        {
            string result;

            switch (url)
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
            return result;
        }
    }
}
