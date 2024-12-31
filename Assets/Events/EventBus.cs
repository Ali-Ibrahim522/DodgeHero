using System.Collections.Generic;

namespace Events {
    public static class EventBus<T> where T : IEvent {
        static readonly HashSet<IEventProcessor<T>> Subscribers = new HashSet<IEventProcessor<T>>();
        
        public static void Subscribe(EventProcessor<T> processor) => Subscribers.Add(processor);
        public static void Unsubscribe(EventProcessor<T> processor) => Subscribers.Remove(processor);

        public static void Publish(T @event) {
            foreach (var processor in Subscribers) {
                processor.OnEvent.Invoke(@event);
                processor.OnEventNoArgs.Invoke();
            }
        }

        static void Clear() => Subscribers.Clear();
    }
}
