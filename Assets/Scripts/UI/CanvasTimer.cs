using System;
using UnityEngine;
using UnityEngine.UI;

namespace RolliCanoli {
    public class CanvasTimer : MonoBehaviour, ITimer {
        private Text _text;
        private Timer _timer;

        public double CurrentMax => _timer.CurrentMax;
        public bool IsStarted => _timer.IsStarted;
        public bool IsCompletable => _timer.IsCompletable;
        public bool IsPaused => _timer.IsPaused;
        public bool IsDone => _timer.IsDone;

        private void Awake() {
            _timer = new();
            gameObject.TryGetComponent(out _text);
            Debug.Assert(_text != null, $"CanvasTimer {gameObject.name} does not have a UI Text element!");
        }

        private void Update() {
            _text.text = _timer.ToString();
        }

        public TimeSpan FindElapsedTime() => _timer.FindElapsedTime();
        public TimeSpan FindCurrentTime() => _timer.FindCurrentTime();

        bool ITimer.StartTimer() => StartTimer();
        public bool StartTimer(bool restartIfDone = false, bool unpauseIfPaused = false) => _timer.StartTimer(restartIfDone: restartIfDone, unpauseIfPaused: unpauseIfPaused);

        bool ITimer.StartTimerAscending(double max) => StartTimerAscending(max);
        public bool StartTimerAscending(double max = 0.0, bool restartIfDone = false) => _timer.StartTimerAscending(max: max, restartIfDone: restartIfDone);

        bool ITimer.StartTimerDescending(double max) => StartTimerDescending(max);
        public bool StartTimerDescending(double max, bool restartIfDone = false) => _timer.StartTimerDescending(max: max, restartIfDone: restartIfDone);

        public void ForceStartTimer() => _timer.ForceStartTimer();
        public void ForceStartTimerAscending(double max = 0.0) => _timer.ForceStartTimerAscending(max: max);
        public void ForceStartTimerDescending(double max) => _timer.ForceStartTimerDescending(max: max);

        public bool Pause() => _timer.Pause();
        public bool Unpause() => _timer.Unpause();
        public void ResetTimer() => _timer.ResetTimer();
        public void ResetTimer(out double elapsedTime) => _timer.ResetTimer(out elapsedTime);

        public override string ToString() => _timer.ToString();
        public string ToString(string format) => _timer.ToString(format);
    }
}
