using System;
using Application.Domain.WriteModel.Events;
using Application.Domain.WriteModel.Exceptions;
using Application.Infrastructure;

namespace Application.Domain.WriteModel;

public class SlotAggregate : AggregateRoot
{
    private bool _isBooked;
    private bool _isScheduled;
    private DateTime _startTime;
    private TimeSpan _duration;

    public SlotAggregate()
    {
        Register<Booked>(When);
        Register<Cancelled>(When);
        Register<Scheduled>(When);
        Register<ScheduledWithDuration>(When);
    }

    public void Schedule(string id, DateTime startTime, TimeSpan duration)
    {
        if (_isScheduled)
        {
            throw new SlotAlreadyScheduledException();
        }

        Raise(new ScheduledWithDuration(id, startTime, duration));
    }

    public void Cancel(string reason, DateTime cancellationTime)
    {
        if (!_isBooked)
        {
            throw new SlotNotBookedException();
        }

        if (IsStarted(cancellationTime))
        {
            throw new SlotAlreadyStartedException();
        }

        if (_isBooked && !IsStarted(cancellationTime))
        {
            Raise(new Cancelled(Id, reason));
        }
    }

    private bool IsStarted(DateTime cancellationTime)
    {
        return cancellationTime.CompareTo(_startTime) > 0;
    }

    public void Book(string patientId)
    {
        if (!_isScheduled)
        {
            throw new SlotNotScheduledException();
        }

        if (_isBooked)
        {
            throw new SlotAlreadyBookedException();
        }

        Raise(new Booked(Id, patientId));
    }

    private void When(Cancelled _)
    {
        _isBooked = false;
    }

    private void When(Booked booked)
    {
        _isBooked = true;
    }

    private void When(Scheduled scheduled)
    {
        var (slotId, startTime) = scheduled;
        
        _isScheduled = true;
        _startTime = startTime;
        Id = slotId;
        _duration = new TimeSpan(0, 15, 0);
    }

    private void When(ScheduledWithDuration scheduled)
    {
        var (slotId, startTime, duration) = scheduled;
        
        _isScheduled = true;
        _startTime = startTime;
        Id = slotId;
        _duration = duration;
    }
}