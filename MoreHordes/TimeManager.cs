﻿using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace MoreHordes
{
    public class TimeManager
    {
        public static readonly TimeManager Instance = new();

        private BehaviorSubject<DateTime> UpdateTime = new(DateTime.Now);

        public void Update()
        {
            UpdateTime.OnNext(DateTime.Now);
        }

        public IObservable<Unit> UpdateTicks() => UpdateTime.Unit();

        public IObservable<Unit> Interval(TimeSpan timeSpan)
        {
            return UpdateTime
                .Scan(
                    new Tuple<DateTime, bool>(DateTime.Now, false),
                    (accum, newItem) =>
                    {
                        if (newItem - accum.Item1 < timeSpan)
                        {
                            return new Tuple<DateTime, bool>(accum.Item1, false);
                        }
                        return new Tuple<DateTime, bool>(newItem, true);
                    })
                .Where(x => x.Item2)
                .Unit();
        }
    }
}