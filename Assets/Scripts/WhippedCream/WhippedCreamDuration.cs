using System.Diagnostics;

namespace RolliCanoli {
    public class WhippedCreamDuration : IPowerupDuration<WhippedCream> {
        public static WhippedCreamDuration operator++(WhippedCreamDuration d) {
            d.Health++;
            d.CheckIfComplete();
            return d;
        }

        public static WhippedCreamDuration operator--(WhippedCreamDuration d) {
            d.Health--;
            d.CheckIfComplete();
            return d;
        }

        public static WhippedCreamDuration operator+(WhippedCreamDuration d, int i) {
            d.Health += i;
            d.CheckIfComplete();
            return d;
        }

        public static WhippedCreamDuration operator-(WhippedCreamDuration d, int i) {
            d.Health -= i;
            d.CheckIfComplete();
            return d;
        }

        private int _initialHealth;

        IPowerup IPowerupDuration.Parent => Parent;
        public PowerupType PowerupType => Parent.PowerupType;
        public WhippedCream Parent { get; private set; }
        public int Health { get; private set; }

        public bool IsComplete => Health <= 0;
        public float PercentIncomplete => _initialHealth > 0 ? (Health / (float)_initialHealth) : 1f;

        public WhippedCreamDuration(WhippedCream parent) {
            Debug.Assert(parent != null, "Null parent given to WhippedCreamDuration!");

            Parent = parent;
            Health = parent.Health;
            _initialHealth = Health;

            var committed = Parent.CommitToDuration(this);
            Debug.Assert(committed, $"WhippedCream {Parent.gameObject.name} is already commited to another duration!");
        }

        public void ForceComplete() => SetHealth(0);

        public bool SetHealth(int health) {
            Health = health;
            return CheckIfComplete();
        }

        private bool CheckIfComplete() {
            if (IsComplete) {
                Health = 0;
                Parent.Respawn();
            }

            return IsComplete;
        }
    }
}
