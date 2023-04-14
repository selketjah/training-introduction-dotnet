using System;
using Application.EventSourcing;

namespace Scheduling.Domain.Infrastructure;


public record Scheduled(
    string SlotId,
    DateTime StartTime
): IEvent;

public class EventVersioning {
    public static IEvent upcast(IEvent e) {

        if(e.GetType() != typeof(Scheduled)){
            return e;
        }

        var scheduled = (Scheduled) e;
        return new Application.Domain.WriteModel.Events.Scheduled(scheduled.SlotId, scheduled.StartTime, new TimeSpan(0,15,0));
    }
}