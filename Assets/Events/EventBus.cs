using System.Collections.Generic;
using System.Linq;

namespace Events {
    public static class EventBus<T> where T : IEvent {
        static readonly Dictionary<int, HashSet<IEventProcessor<T>>> Subscribers = new ();
        public static void Subscribe(EventProcessor<T> processor, int channel = 0) {
            if (!Subscribers.ContainsKey(channel)) Subscribers[channel] = new HashSet<IEventProcessor<T>>();
            Subscribers[channel].Add(processor);
        }
        public static void Unsubscribe(EventProcessor<T> processor, int channel = 0) {
            if (Subscribers.TryGetValue(channel, out var subscriber)) subscriber.Remove(processor);
        }

        public static void Publish(T @event, int channel = 0) {
            if (Subscribers.TryGetValue(channel, out var processors)) {
                foreach (var processor in processors.ToArray()) {
                    processor.OnEvent.Invoke(@event);
                    processor.OnEventNoArgs.Invoke();
                }
            }
        }

        public static int SubscriberCount(int channel = 0) {
            if (Subscribers.TryGetValue(channel, out var subscriber)) return subscriber.Count;
            return 0;
        }

        static void Clear() {
            foreach (var subscriber in Subscribers.Values) subscriber.Clear();
            Subscribers.Clear();
        }
    }
}
