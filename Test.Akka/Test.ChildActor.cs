using Akka.Actor;
using Akka.TestKit.Xunit2;
using FluentAssertions;
using System;
using Xunit;

namespace Test.ChildActor
{
    public class MotherActor : ReceiveActor
    {
        private readonly IActorRef _childActor;

        public static Props Props(Func<IUntypedActorContext, IActorRef> makeChild)
        {
            return Akka.Actor.Props.Create(() => new MotherActor(makeChild));
        }

        public MotherActor(Func<IUntypedActorContext, IActorRef> makeChild)
        {
            _childActor = makeChild(Context);

            Receive<string>(x => _childActor.Tell(x, Sender));
        }
    }

    public class ChildActor : ReceiveActor
    {
        public static Props Props()
        {
            return Akka.Actor.Props.Create(()=>new ChildActor());
        }
        public ChildActor()
        {
            Receive<string>(x => Sender.Tell($"{x}-Child"));
        }
    }

    //https://getakka.net/articles/actors/testing-actor-systems.html#externalize-child-making-from-the-parent
    public class TestChildActor : TestKit
    {
        [Fact]
        public void CreateChildUsingTestActor()
        {
            var motherActor = ActorOfAsTestActorRef<MotherActor>(
                MotherActor.Props(context => TestActor ));

            motherActor.Tell("705243F7-2C1B-4CEE-9B5F-80AE88649180");
            ExpectMsg("705243F7-2C1B-4CEE-9B5F-80AE88649180");
        }

        [Fact]
        public void CreateChildUsingProbe()
        {
            var childProbe = CreateTestProbe();
            var motherActor = ActorOfAsTestActorRef<MotherActor>(
                MotherActor.Props(context => childProbe.Ref));

            motherActor.Tell("66A68CB8-AA85-4484-966F-63E64A0C183F");
            childProbe.ExpectMsg("66A68CB8-AA85-4484-966F-63E64A0C183F");
        }

        [Fact]
        public void CreateChildUsingRealChildActor()
        {
            IActorRef childActor = null;
            var motherActor = ActorOfAsTestActorRef<MotherActor>(
                MotherActor.Props(context => childActor = context.ActorOf(ChildActor.Props(),"ChildActor")),"MotherActor");

            childActor.Path.ToString().Should().Be("akka://test/user/MotherActor/ChildActor");
            motherActor.Tell("6076F8AE-164D-4687-9CA7-1D90F33EC6AF");
            ExpectMsg("6076F8AE-164D-4687-9CA7-1D90F33EC6AF-Child");
        }
    }
}
