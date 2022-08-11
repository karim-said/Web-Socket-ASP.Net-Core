using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("press enter to const......") ;
            Console.ReadLine();

            using (ClientWebSocket client = new ClientWebSocket())
            {
                Uri serviceUri = new Uri("ws://localhost:5000/send");
                var cTs = new CancellationTokenSource();
                cTs.CancelAfter(TimeSpan.FromSeconds(120));

                try
                {
                    await client.ConnectAsync(serviceUri, cTs.Token);
                     
                    while (client.State == WebSocketState.Open)
                    {
                        Console.WriteLine("Enter message to send");
                        string message = Console.ReadLine();

                        if(!string.IsNullOrEmpty(message))
                        {
                            ArraySegment<byte> byteToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
                            await client.SendAsync(byteToSend, WebSocketMessageType.Text, true, cTs.Token);
                            var responceBuffer = new byte[1024];
                            var offset = 0;
                            var packeg = 1024;

                            while (true)
                            {
                                ArraySegment<byte> byteRecieved = new ArraySegment<byte>(responceBuffer, offset, packeg);
                                WebSocketReceiveResult response = await client.ReceiveAsync(byteRecieved, cTs.Token);
                                var responseMessage = Encoding.UTF8.GetString(responceBuffer, offset, response.Count);

                                if (response.EndOfMessage)
                                    break;
                            }
                        }

                    }
                }
                catch (WebSocketException e)
                {

                    Console.WriteLine(e.Message);
                }
            }

            Console.ReadLine();
        }
    }
}
