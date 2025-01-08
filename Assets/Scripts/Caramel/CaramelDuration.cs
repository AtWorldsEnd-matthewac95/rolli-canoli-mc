using System.Diagnostics;

namespace RolliCanoli {
    public class CaramelDuration : IPowerupDuration<Caramel> {
        public static CaramelDuration operator++(CaramelDuration d) {
            d.RemainingFrames++;
            d.CheckIfComplete();
            return d;
        }

        public static CaramelDuration operator--(CaramelDuration d) {
            d.RemainingFrames--;
            d.CheckIfComplete();
            return d;
        }

        public static CaramelDuration operator+(CaramelDuration d, int i) {
            d.RemainingFrames += i;
            d.CheckIfComplete();
            return d;
        }

        public static CaramelDuration operator-(CaramelDuration d, int i) {
            d.RemainingFrames -= i;
            d.CheckIfComplete();
            return d;
        }

        private int _initialFrames;

        IPowerup IPowerupDuration.Parent => Parent;
        public Caramel Parent { get; private set; }
        public PowerupType PowerupType => Parent.PowerupType;
        public int RemainingFrames { get; private set; }
        public bool IsComplete => RemainingFrames <= 0;
        public float PercentIncomplete => _initialFrames > 0 ? (RemainingFrames / (float)_initialFrames) : 1f;

        public CaramelDuration(Caramel parent) : this(parent, parent.ClimbingFrames) { }
        public CaramelDuration(Caramel parent, int initialFrames) {
            Debug.Assert(parent != null, "Null Caramel object given to CaramelDuration constructor!");

            Parent = parent;
            RemainingFrames = initialFrames;
            _initialFrames = RemainingFrames;

            var commitResult = Parent.CommitToDuration(this);
            Debug.Assert(commitResult, $"Caramel {Parent.gameObject.name} is already commited to another duration!");
        }

        public void ForceComplete() => SetFrames(0);

        public bool SetFrames(int frames) {
            RemainingFrames = frames;
            return CheckIfComplete();
        }

        private bool CheckIfComplete() {
            if (IsComplete) {
                RemainingFrames = 0;
                Parent.Respawn();
            }

            return IsComplete;
        }
    }
}
