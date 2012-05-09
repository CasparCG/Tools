using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive;
using System.Reactive.Linq;
using CasparRx;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var c = new Connection("localhost");

            c.OnConnected.Subscribe(x => Console.WriteLine(x));

            while (true)
            {
                c.Send(Console.ReadLine()).ToList().ForEach(Console.WriteLine);
                //c.AsyncSend(Console.ReadLine()).Subscribe(Console.WriteLine);
            }
        }
    }
}
