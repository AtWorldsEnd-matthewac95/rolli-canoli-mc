using UnityEngine;

namespace RolliCanoli {
    public class Caramel : MonoBehaviour, IPowerup<CaramelDuration> {
        public const int DEFAULT_CLIMBING_FRAMES_GIVEN = 80;

        private CaramelDuration _commitedDuration;

        [SerializeField]
        private int _climbingFramesGiven = DEFAULT_CLIMBING_FRAMES_GIVEN;

        [SerializeField]
        private AudioSource _pickupSound;

        public PowerupType PowerupType => PowerupType.Caramel;
        public int ClimbingFrames => _climbingFramesGiven;

        private void Awake() {
            _commitedDuration = null;
        }

        private void Start() {
            if (_pickupSound == null && !gameObject.TryGetComponent(out _pickupSound)) {
                Debug.Log($"Caramel {gameObject.name} has no audio source! This object will not play sounds.");
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
        public CaramelDuration CreateDuration() => gameObject.activeSelf ? new CaramelDuration(this) : null;

        bool IPowerup.CreateDuration(out IPowerupDuration duration) {
            bool result = CreateDuration(out CaramelDuration caramelDuration);
            duration = caramelDuration;
            return result;
        }
        public bool CreateDuration(out CaramelDuration duration) {
            duration = CreateDuration();
            return duration != null;
        }

        public bool CommitToDuration(CaramelDuration duration) {
            bool commitable = gameObject.activeSelf;

            if (commitable) {
                gameObject.SetActive(false);
                _commitedDuration = duration;
            }

            return commitable;
        }

        public bool GetCommitedDuration(out CaramelDuration duration) {
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
