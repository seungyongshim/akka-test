using Akka.Actor;
using Akka.TestKit.Xunit2;
using System.ComponentModel;
using Xunit;
using FluentAssertions;
using FluentAssertions.Extensions;

namespace PingPong.Test
{
    public class AkkaViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string LabelText { get; set; }
        public IActorRef Actor { get; private set; }

        public AkkaViewModel(IActorRefFactory actorSystem) =>
            Actor = actorSystem.ActorOf(AkkaModelViewActor.Props(this));
    }
    
    public class AkkaModelViewActor : ReceiveActor
    {
        public AkkaViewModel ViewModel { get; private set; }

        public static Props Props(AkkaViewModel vm) =>
            Akka.Actor.Props.Create(() => new AkkaModelViewActor(vm));

        private AkkaModelViewActor() =>
            Receive<string>(msg => ViewModel.LabelText = msg);

        public AkkaModelViewActor(AkkaViewModel vm) : this() => ViewModel = vm;
    }

    public class TestMvvmActor : TestKit
    {
        [Fact]
        public void ChangePropertyMvvmUsingActor()
        {
            var vm = new AkkaViewModel(Sys);
            vm.Actor.Tell("Hello world");
            ExpectNoMsg();
            vm.LabelText.Should().Be("Hello world");
        }
    }
}
