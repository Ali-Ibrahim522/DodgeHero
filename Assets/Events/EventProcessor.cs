using System;

namespace Events {
    internal interface IEventProcessor<T> {
        public Action<T> OnEvent { get; set; }
        public Action OnEventNoArgs { get; set; }
    }

    public class EventProcessor<T> : IEventProcessor<T> where T : IEvent {
        Action<T> onEvent = _ => { };
        Action onEventNoArgs = () => { };

        Action<T> IEventProcessor<T>.OnEvent {
            get => onEvent;
            set => onEvent = value;
        }
        
        Action IEventProcessor<T>.OnEventNoArgs {
            get => onEventNoArgs;
            set => onEventNoArgs = value;
        }

        public EventProcessor(Action<T> onEvent) => this.onEvent = onEvent;
        public EventProcessor(Action onEventNoArgs) => this.onEventNoArgs = onEventNoArgs;
        
        public void Add(Action action) => onEventNoArgs += action;
        public void Add(Action<T> action) => onEvent += action;
        
        public void Remove(Action action) => onEventNoArgs -= action;
        public void Remove(Action<T> action) => onEvent -= action;
    }
}
