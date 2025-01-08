using System;
using UnityEngine;

namespace RolliCanoli {
    public class Timer : ITimer {
        private const string DEFAULT_FORMAT = "mm\\:ss\\.fff";

        private bool _isDone;
        private bool _isDescending;
        private double _pausedElapsedTime;
        private double _startTime;

        public double CurrentMax { get; private set; }
        public bool IsStarted { get; private set; }
        public bool IsCompletable => CurrentMax > float.Epsilon && !double.IsInfinity(CurrentMax);
        public bool IsPaused => _pausedElapsedTime > float.Epsilon;
        public bool IsDone {
            get {
                _isDone = CheckIfDone();
                return _isDone;
            }
        }

        public Timer() {
            _isDone = false;
            _startTime = 0.0;
            _pausedElapsedTime = 0.0;

            CurrentMax = 0.0;
            IsStarted = false;
        }

        private bool CheckIfDone() => CheckIfDone(FindElapsedTime());
        private bool CheckIfDone(TimeSpan elapsedTime) => _isDone || (IsStarted && IsCompletable && elapsedTime.TotalSeconds >= CurrentMax);
        private bool CheckIfDone(double elapsedTime) => _isDone || (IsStarted && IsCompletable && elapsedTime >= CurrentMax);

        public TimeSpan FindElapsedTime() => IsStarted ? TimeSpan.FromSeconds(IsPaused ? _pausedElapsedTime : (Time.timeAsDouble - _startTime)) : default;

        public TimeSpan FindCurrentTime() {
            var current = new TimeSpan();
            var elapsed = IsPaused ? _pausedElapsedTime : FindElapsedTime().TotalSeconds;

            if (IsStarted) {
                _isDone = CheckIfDone(elapsed);
                if (_isDone) {
                    current = TimeSpan.FromSeconds(_isDescending ? 0.0 : CurrentMax);
                } else {
                    current = TimeSpan.FromSeconds(_isDescending && IsCompletable ? CurrentMax - elapsed : elapsed);
                }
            }

            return current;
        }

        bool ITimer.StartTimer() => StartTimer();
        public bool StartTimer(bool restartIfDone = false, bool unpauseIfPaused = false) {
            if (unpauseIfPaused && IsPaused) {
                Unpause();
            }

            return StartTimerAscending(restartIfDone: restartIfDone);
        }

        bool ITimer.StartTimerAscending(double max) => StartTimerAscending(max);
        public bool StartTimerAscending(double max = 0.0, bool restartIfDone = false) {
            bool newStart = !IsStarted || (restartIfDone && IsDone);

            if (newStart) {
                ForceStartTimerAscending(max);
            }

            return newStart;
        }

        bool ITimer.StartTimerDescending(double max) => StartTimerDescending(max);
        public bool StartTimerDescending(double max, bool restartIfDone = false) {
            bool newStart = !IsStarted || (restartIfDone && IsDone);

            if (newStart) {
                ForceStartTimerDescending(max);
            }

            return newStart;
        }

        public void ForceStartTimer() => ForceStartTimerAscending();
        public void ForceStartTimerAscending(double max = 0.0) {
            _isDescending = false;
            _startTime = Time.timeAsDouble;
            _pausedElapsedTime = 0.0;
            CurrentMax = max;
            IsStarted = true;
        }

        public void ForceStartTimerDescending(double max) {
            _isDescending = true;
            _startTime = Time.timeAsDouble;
            _pausedElapsedTime = 0.0;
            CurrentMax = max;
            IsStarted = true;
        }

        public bool Pause() {
            bool wasUnpaused = !IsPaused;

            if (wasUnpaused) {
                _pausedElapsedTime = FindElapsedTime().TotalSeconds;
            }

            return wasUnpaused;
        }

        public bool Unpause() {
            bool wasPaused = IsPaused;

            if (wasPaused) {
                _startTime = Time.timeAsDouble - _pausedElapsedTime;
                _pausedElapsedTime = 0.0;
            }

            return wasPaused;
        }

        public void ResetTimer() {
            _isDone = false;
            _startTime = 0.0;
            _pausedElapsedTime = 0.0;
            CurrentMax = 0.0;
            IsStarted = false;
        }

        public void ResetTimer(out double elapsedTime) {
            elapsedTime = FindElapsedTime().TotalSeconds;
            ResetTimer();
        }

        public override string ToString() => ToString(DEFAULT_FORMAT);
        public string ToString(string format) {
            var result = string.Empty;

            if (IsStarted) {
                string separator = IsCompletable ? "/" : string.Empty;
                string denominator = IsCompletable ? TimeSpan.FromSeconds(CurrentMax).ToString(format) : string.Empty;
                result = $"{FindCurrentTime().ToString(format)}{separator}{denominator}";
            }

            return result;
        }
    }
}
