namespace Events {
    public interface IEvent { }

    public readonly struct HitEvent : IEvent {
        public int Gained { get; }
        public HitEvent(int gained) {
            Gained = gained;
        }
    }
    public struct DeathEvent : IEvent {}
    public struct MissEventHealthUpdate : IEvent {}
    public struct MissEventStatsUpdate : IEvent {}
}
