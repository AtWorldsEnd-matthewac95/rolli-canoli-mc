using UnityEngine;

namespace RolliCanoli {
    public class RemoveOnePowerup : MonoBehaviour, IRemovePowerup {
        private const float RE_ENABLE_MULTIPLIER = 1.5f;

        private Collider _collider;
        private OblongPlayerController _player;

        [SerializeField]
        private AudioSource _removalSound;

        public bool RemovesAllPowerups => false;

        private void Awake() {
            gameObject.TryGetComponent(out _collider);
            Debug.Assert(_collider != null, $"{gameObject.name} has no collider!");
        }

        private void Start() {
            if (_removalSound == null && !gameObject.TryGetComponent(out _removalSound)) {
                Debug.Log($"RemoveOnePowerup object {gameObject.name} has no audio source! This object will not play sounds.");
            }
        }

        private void Update() {
            if (!_collider.enabled && (_player == null || (_player.transform.position - transform.position).magnitude >= (_player.Length * RE_ENABLE_MULTIPLIER))) {
                _collider.enabled = true;
                _player = null;
            }
        }

        public bool ValidateRemoval(OblongPlayerController player) {
            bool success = _collider.enabled;

            if (success) {
                _collider.enabled = false;
                _player = player;

                if (_removalSound != null) {
                    _removalSound.Play();
                }
            }

            return success;
        }
    }
}
