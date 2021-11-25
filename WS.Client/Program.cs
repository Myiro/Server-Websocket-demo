using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WS.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("entrez votre nom d'utilisateur");
            string name = Console.ReadLine();

            using (ClientWebSocket client = new ClientWebSocket())
            {
                Uri serviceUri = new Uri("ws://185.180.21.127:5000/send");
                var cTs = new CancellationTokenSource();
                cTs.CancelAfter(TimeSpan.FromSeconds(120));
                try
                {
                    await client.ConnectAsync(serviceUri, cTs.Token);
                    var n = 0;
                    Console.WriteLine("Connected <3");
                    while (client.State == WebSocketState.Open)
                    {
                        string message = Console.ReadLine();

                        message = message.Replace(";", "");

                        string messageAndName = message + ";" + name;  
                        if (!string.IsNullOrEmpty(message))
                        {
                            ArraySegment<byte> byteToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(messageAndName));
                            await client.SendAsync(byteToSend, WebSocketMessageType.Text, true, cTs.Token);
                            var reponseBuffer = new byte[1024];
                            var offset = 0;
                            var packet = 1024;
                            while (true)
                            {
                                ArraySegment<byte> byteRecieved = new ArraySegment<byte>(reponseBuffer, offset, packet);
                                WebSocketReceiveResult reponse = await client.ReceiveAsync(byteRecieved, cTs.Token);
                                var reponseMessage = Encoding.UTF8.GetString(reponseBuffer, offset, reponse.Count);
                                Console.WriteLine(reponseMessage);
                                if (reponse.EndOfMessage)
                                {
                                    break;
                                }
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
