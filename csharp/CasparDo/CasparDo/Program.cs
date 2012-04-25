using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;


namespace CasparDo
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = args[0];
            var port = args[1];
            var cmd  = string.Join(" ", args.Skip(2));

            using (var client = new TcpClient(host, int.Parse(port)))
            {
                var reader = new StreamReader(client.GetStream());
                var writer = new StreamWriter(client.GetStream());

                writer.WriteLine(cmd);
                writer.Flush();

                var reply = reader.ReadLine();
               
                Console.WriteLine(reply);

                if (reply.Contains("201"))
                {
                    reply = reader.ReadLine();
                    Console.WriteLine(reply);
                }
                else if (reply.Contains("200"))
                {
                    while (reply.Length > 0)
                    {
                        reply = reader.ReadLine();
                        Console.WriteLine(reply);
                    }
                }                
            }
        }
    }
}
