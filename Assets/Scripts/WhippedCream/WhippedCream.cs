using UnityEngine;

namespace RolliCanoli {
    public class WhippedCream : MonoBehaviour, IPowerup<WhippedCreamDuration> {
        public const int DEFAULT_EXTRA_SIZE = 3;
        public const int DEFAULT_HEALTH = 100;

        private WhippedCreamDuration _commitedDuration;

        [SerializeField]
        private int _extraSize = DEFAULT_EXTRA_SIZE;

        [SerializeField]
        private int _health = 100;

        [SerializeField]
        private AudioSource _pickupSound;

        public PowerupType PowerupType => PowerupType.WhippedCream;
        public int ExtraSize => _extraSize;
        public int Health => _health;

        private void Start() {
            if (_pickupSound == null && !gameObject.TryGetComponent(out _pickupSound)) {
                Debug.Log($"WhippedCream {gameObject.name} has no audio source! This object will not play sounds.");
            }
        }

        public bool Respawn() {
            if (!gameObject.activeInHierarchy) {
                gameObject.SetActive(true);
                _commitedDuration = null;
                return true;
            }

            return false;
        }

        IPowerupDuration IPowerup.CreateDuration() => CreateDuration();
        public WhippedCreamDuration CreateDuration() {
            CreateDuration(out WhippedCreamDuration duration);
            return duration;
        }

        bool IPowerup.CreateDuration(out IPowerupDuration duration) {
            bool result = CreateDuration(out WhippedCreamDuration whippedCreamDuration);
            duration = whippedCreamDuration;
            return result;
        }
        public bool CreateDuration(out WhippedCreamDuration duration) {
            duration = gameObject.activeSelf ? new WhippedCreamDuration(this) : null;
            return true;
        }

        public bool CommitToDuration(WhippedCreamDuration duration) {
            bool commitable = gameObject.activeSelf;

            if (commitable) {
                gameObject.SetActive(false);
                _commitedDuration = duration;
            }

            return commitable;
        }

        public bool GetCommitedDuration(out WhippedCreamDuration duration) {
            duration = _commitedDuration;
            bool isCommited = duration != null;
            return isCommited;
        }

        public bool PlaySound() {
            bool hasSound = _pickupSound != null;

            if (hasSound) {
                _pickupSound.Play();
            }

            return hasSound;
        }
    }
}
