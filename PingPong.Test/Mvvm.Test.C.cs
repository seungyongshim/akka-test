using Akka.Actor;
using Akka.TestKit.Xunit2;
using System.ComponentModel;
using Xunit;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Extensions;
using System.Windows.Input;
using System;

//https://github.com/AutoFixture/AutoFixture/wiki/Cheat-Sheet
namespace Mvvm.Test.C
{
    public class MessageClick {}
    public class RelayCommand : ICommand
    {
        readonly Action _action;

        public RelayCommand(Action action)
        {
            _action = action;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return !(_action is null);
        }

        public void Execute(object parameter)
        {
            _action();
        }
    }

    public class AkkaViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string LabelText { get; set; }
        public IActorRef Actor { get; private set; }

        public RelayCommand ClickCommand { get; private set; }

        private AkkaViewModel()
        {
            ClickCommand = new RelayCommand(() => Actor.Tell(new MessageClick()));
        }

        public AkkaViewModel(IActorRefFactory actorSystem): this()
        {
            Actor = actorSystem.ActorOf(AkkaModelViewActor.Props(this));
        }

        public AkkaViewModel(IActorRefFactory actorSystem, IActorRef targetActor) : this()
        {
            Actor = actorSystem.ActorOf(AkkaModelViewActor.Props(this, targetActor));
        }
    }
    
    public class AkkaModelViewActor : ReceiveActor
    {
        private IActorRef _targetActor;

        public AkkaViewModel ViewModel { get; private set; }

        public static Props Props(AkkaViewModel vm) =>
            Akka.Actor.Props.Create(() => new AkkaModelViewActor(vm));

        public static Props Props(AkkaViewModel vm, IActorRef targetActor) =>
            Akka.Actor.Props.Create(() => new AkkaModelViewActor(vm, targetActor));

        private AkkaModelViewActor()
        {
            Receive<string>(msg => ViewModel.LabelText = msg);
            Receive<MessageClick>(msg => _targetActor.Tell(msg));
        }

        public AkkaModelViewActor(AkkaViewModel vm) : this() => ViewModel = vm;

        public AkkaModelViewActor(AkkaViewModel vm, IActorRef targetActor) : this(vm)
        {
            _targetActor = targetActor;
        }
    }

    public class TestMvvmActor : TestKit
    {
        [Fact]
        public void ChangePropertyMvvmUsingActor()
        {
            var fixture = new Fixture();
            fixture.Register<IActorRefFactory>(() => Sys);
            var msg = fixture.Create<string>();
            var vm = fixture.Create<AkkaViewModel>();
            vm.Actor.Tell(msg);
            ExpectNoMsg();
            vm.LabelText.Should().Be(msg);
        }

        // AutoFixture는 IoC와 반대로 가장 작은 생성자를 선택한다.
        // https://blog.ploeh.dk/2011/04/19/ConstructorstrategiesforAutoFixture/
        [Fact]
        public void ClickMvvmUsingActor()
        {
            var fixture = new Fixture();
            fixture.Register<IActorRefFactory>(() => Sys);
            fixture.Register(() => TestActor);
            var vm = fixture.Create<AkkaViewModel>();
            vm.ClickCommand.Execute(null);
            ExpectMsg<MessageClick>();
        }
    }
}
