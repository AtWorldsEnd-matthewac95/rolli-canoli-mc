using System;

namespace RolliCanoli {
    public interface ITimer {
        bool IsStarted { get; }
        bool IsDone { get; }

        TimeSpan FindElapsedTime();
        TimeSpan FindCurrentTime();

        bool StartTimer();
        bool StartTimerAscending(double max = 0.0);
        bool StartTimerDescending(double max);

        void ResetTimer();
        void ResetTimer(out double elapsedTime);

        string ToString(string format);
    }
}
