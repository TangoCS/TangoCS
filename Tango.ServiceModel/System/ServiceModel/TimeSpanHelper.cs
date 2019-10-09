//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
namespace System.ServiceModel
{
	using System;

	static class TimeSpanHelper
    {
        static public TimeSpan FromMinutes(int minutes, string text)
        {
            TimeSpan value = TimeSpan.FromTicks(TimeSpan.TicksPerMinute * minutes);
            return value;
        }
        static public TimeSpan FromSeconds(int seconds, string text)
        {
            TimeSpan value = TimeSpan.FromTicks(TimeSpan.TicksPerSecond * seconds);
            return value;
        }
        static public TimeSpan FromMilliseconds(int ms, string text)
        {
            TimeSpan value = TimeSpan.FromTicks(TimeSpan.TicksPerMillisecond * ms);
            return value;
        }
        static public TimeSpan FromDays(int days, string text)
        {
            TimeSpan value = TimeSpan.FromTicks(TimeSpan.TicksPerDay * days);
            return value;
        }
    }
}
