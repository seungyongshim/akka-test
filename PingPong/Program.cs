using Akka.Actor;
using Akka.Configuration;
using System;
using System.IO;

namespace PingPong
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(File.ReadAllText("Akka.hocon")); ;

            var sys = ActorSystem.Create("PingPong", config);

            var pingActor = sys.ActorOf(Props.Create(() => new PingActor()));

            Console.ReadLine();
        }
    }
}
