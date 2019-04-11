using Akka.Actor;
using Akka.TestKit.Xunit2;
using System;
using Xunit;

namespace PingPong.Test
{
    public class TestPingActor : TestKit
    {
        [Fact]
        public void TellString()
        {
            var pingActor = Sys.ActorOf(Props.Create(() => new PingActor()));

            pingActor.Tell("string");

            ExpectMsg<string>("string");
        }
    }
}
