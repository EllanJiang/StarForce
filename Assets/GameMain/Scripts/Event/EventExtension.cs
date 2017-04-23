using GameFramework.Event;
using System;
using UnityGameFramework.Runtime;

namespace StarForce
{
    public static class EventExtension
    {
        public static bool Check(this EventComponent eventComponent, EventId eventId, EventHandler<GameEventArgs> handler)
        {
            return eventComponent.Check((UnityGameFramework.Runtime.EventId)eventId, handler);
        }

        public static void Subscribe(this EventComponent eventComponent, EventId eventId, EventHandler<GameEventArgs> handler)
        {
            eventComponent.Subscribe((UnityGameFramework.Runtime.EventId)eventId, handler);
        }

        public static void Unsubscribe(this EventComponent eventComponent, EventId eventId, EventHandler<GameEventArgs> handler)
        {
            eventComponent.Unsubscribe((UnityGameFramework.Runtime.EventId)eventId, handler);
        }
    }
}
