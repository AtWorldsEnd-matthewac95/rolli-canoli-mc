using System;
using System.Collections;
using System.Collections.Generic;

namespace RolliCanoli {
    public class PowerupDurationCollection : IEnumerable<IPowerupDuration> {
        private readonly Stack<IPowerupDuration> _durations;

        public bool IsEmpty => _durations.Count <= 0;
        public int Count => _durations.Count;

        public PowerupDurationCollection() {
            _durations = new Stack<IPowerupDuration>();
        }
        public PowerupDurationCollection(params IPowerupDuration[] durations) : this((IEnumerable<IPowerupDuration>)durations) {}
        public PowerupDurationCollection(IEnumerable<IPowerupDuration> durations) {
            foreach (var duration in durations) {
                _durations.Push(duration);
            }
        }

        public int FindEffectiveCount() {
            int count = Count;

            foreach (var duration in _durations) {
                if (duration.Parent.PowerupType == PowerupType.WhippedCream) {
                    count += (duration.Parent as WhippedCream).ExtraSize;
                }
            }

            return count;
        }

        public bool IsTopIncomplete() {
            while (_durations.TryPeek(out IPowerupDuration duration)) {
                if (duration.IsComplete) {
                    _durations.Pop();
                } else {
                    return true;
                }
            }

            return false;
        }

        public bool TryPeek(out IPowerupDuration duration) {
            duration = default;
            return IsTopIncomplete() && _durations.TryPeek(out duration);
        }

        public bool TryPeek<TPowerupDuration>(out TPowerupDuration duration) where TPowerupDuration : IPowerupDuration {
            bool success = TryPeek(out IPowerupDuration iDuration) && iDuration is TPowerupDuration;
            duration = success ? (TPowerupDuration)iDuration : default;
            return success;
        }

        public void Add(IPowerupDuration duration) => _durations.Push(duration);
        public void Add(IPowerup powerup) => Add(powerup.CreateDuration());

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IPowerupDuration> GetEnumerator() => _durations.GetEnumerator();

        public IPowerupDuration RemoveTop() {
            RemoveTop(out IPowerupDuration removed);
            return removed;
        }
        public bool RemoveTop(out IPowerupDuration removed) {
            bool success = _durations.TryPop(out removed);

            if (success) {
                removed.ForceComplete();
            }

            return success;
        }

        public bool Clear() {
            bool collectionWasNotEmpty = _durations.Count > 0;

            foreach (var duration in _durations) {
                duration.ForceComplete();
            }

            _durations.Clear();
            return collectionWasNotEmpty;
        }
    }
}
