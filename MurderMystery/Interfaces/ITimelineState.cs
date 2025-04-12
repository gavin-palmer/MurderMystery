using System;
using MurderMystery.MurderMystery.Generators;

namespace MurderMystery.Interfaces
{
    public interface ITimelineState
    {
        void Process(TimelineContext context);

        string GetCurrentTime();
    }
}
