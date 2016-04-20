namespace HTTPWebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server();
            server.Start(new[] { "http://*:29963/" });
        }
    }
}
