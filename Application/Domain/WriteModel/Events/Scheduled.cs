using System;
using Application.EventSourcing;

namespace Application.Domain.WriteModel.Events;

public record ScheduledWithDuration(
    string SlotId,
    DateTime StartTime,
    TimeSpan Duration
): IEvent;

public record Scheduled(
    string SlotId,
    DateTime StartTime
): IEvent;