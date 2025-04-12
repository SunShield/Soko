namespace Soko.Core.Events.Impl.Args
{
    public class MovementFinishedArgs : IGameEventArgs
    {
        public bool AnyObjectMoved { get; set; }
    }
}