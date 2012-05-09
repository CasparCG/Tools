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

            while (true)
            {
                c.Send(Console.ReadLine()).Subscribe(Console.WriteLine);
            }
        }
    }
}
