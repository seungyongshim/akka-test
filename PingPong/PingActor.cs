using Akka.Actor;

namespace PingPong
{
    public class PingActor : ReceiveActor
    {
        public PingActor()
        {
            Receive<string>(x =>
            {
                Sender.Tell(x);
            });
        }
    }
}