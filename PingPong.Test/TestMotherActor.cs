using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.Xunit2;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PingPong.Test
{
    public class MotherActor : ReceiveActor
    {
        IActorRef _childActor;

        public MotherActor(Func<IUntypedActorContext, IActorRef> childMaker)
        {
            _childActor = childMaker(Context);

            Receive<string>(x =>
                _childActor.Tell(x)
            );
        }
    }

    //https://getakka.net/articles/actors/testing-actor-systems.html#externalize-child-making-from-the-parent
    public class TestMotherActor : TestKit
    {
        [Fact]
        public void CreateChildUsingTestActor()
        {
            var mother = ActorOfAsTestActorRef<MotherActor>(Props.Create(
                () => new MotherActor( x=> TestActor )));

            mother.Tell("705243F7-2C1B-4CEE-9B5F-80AE88649180");

            ExpectMsg("705243F7-2C1B-4CEE-9B5F-80AE88649180");
        }

        [Fact]
        public void CreateChildUsingProbe()
        {
            var childProbe = CreateTestProbe();
            var mother = ActorOfAsTestActorRef<MotherActor>(Props.Create(
                () => new MotherActor(x => childProbe.Ref)));

            mother.Tell("705243F7-2C1B-4CEE-9B5F-80AE88649180");
            childProbe.ExpectMsg("705243F7-2C1B-4CEE-9B5F-80AE88649180");
        }
    }
}
