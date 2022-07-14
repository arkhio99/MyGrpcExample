using Grpc.Core;

namespace CarServer
{
    class Program
    {
        public static void Main(string[] args)
        {
            var server = new Server
            {
                Services = { CarBuilder.BindService(new CarServer()) },
                Ports = { new ServerPort("localhost", 30051, ServerCredentials.Insecure) },
            };

            server.Start();

            Console.WriteLine("CarServer listening on port " + 30051);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}