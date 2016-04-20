using System.Net;

namespace HTTPWebServer.Infrastructure
{
    public interface IRequestHandler
    {
        void HandleRequest(HttpListenerContext context);
    }
}
