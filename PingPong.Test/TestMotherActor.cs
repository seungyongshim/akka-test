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

            ReceiveAny(x =>
                _childActor.Tell(x)
            );
        }
    }

    //https://getakka.net/articles/actors/testing-actor-systems.html#externalize-child-making-from-the-parent
    public class TestMotherActor : TestKit
    {
        [Fact]
        public void CreateChild()
        {
            var child = CreateTestProbe();
            var mother = ActorOfAsTestActorRef<MotherActor>(Props.Create(() => new MotherActor( x=>child.Ref  )));

            mother.Tell(false);

            ExpectMsgAnyOf(true);
        }
    }
}
