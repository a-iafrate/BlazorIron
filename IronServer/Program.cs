



namespace IronServer // Note: actual namespace depends on the project name.
{
    public class Program
    {
      

        private static async Task Main(string[] args)
        {
           IronServer server = new IronServer();
           await server.Start();
        }

       
    }
}