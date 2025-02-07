using System.Collections.Generic;

namespace Events {
    public static class EventBus<T> where T : IEvent {
        private static readonly Dictionary<int, HashSet<IEventProcessor<T>>> Subscribers = new() {
            {0, new HashSet<IEventProcessor<T>> ()}
        };
        private static readonly HashSet<int> Invoking = new ();
        private static readonly Queue<IEventProcessor<T>> DeferredRemovals = new();
        private static readonly Queue<IEventProcessor<T>> DeferredAdditions = new();
        
        public static void Subscribe(EventProcessor<T> processor, int channel = 0) {
            if (!Subscribers.ContainsKey(channel)) Subscribers[channel] = new HashSet<IEventProcessor<T>>();
            if (Invoking.Contains(channel)) DeferredAdditions.Enqueue(processor);
            else Subscribers[channel].Add(processor);
        }
        public static void Unsubscribe(EventProcessor<T> processor, int channel = 0) {
            if (Subscribers.TryGetValue(channel, out var subscriber)) {
                if (Invoking.Contains(channel)) DeferredRemovals.Enqueue(processor);
                else subscriber.Remove(processor);
            }
        }

        public static void Publish(T @event, int channel = 0) {
            Invoking.Add(channel);
            if (Subscribers.TryGetValue(channel, out var processors)) {
                foreach (var processor in processors) {
                    processor.OnEvent.Invoke(@event);
                    processor.OnEventNoArgs.Invoke();
                }
                while (DeferredRemovals.Count > 0) Subscribers[channel].Remove(DeferredRemovals.Dequeue());
                while (DeferredAdditions.Count > 0) Subscribers[channel].Add(DeferredAdditions.Dequeue());
            }
            Invoking.Remove(channel);
        }

        public static int SubscriberCount(int channel = 0) {
            return Subscribers.TryGetValue(channel, out var subscriber) ? subscriber.Count : 0;
        }

        public static void Clear() {
            foreach (var subscriber in Subscribers.Values) subscriber.Clear();
            Subscribers.Clear();
        }
    }
}
