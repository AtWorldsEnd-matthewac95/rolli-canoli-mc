using UnityEngine;

namespace RolliCanoli {
    public class PlayerChecker : MonoBehaviour {
        private Collider _collider;
        private bool _isPlayerStaying;
        private float _remainingGracePeriod;

        [SerializeField]
        private float _gracePeriod;

        [SerializeField]
        private bool _isContainingPlayer;

        public bool IsContainingPlayer => _isContainingPlayer;

        private void Awake() {
            gameObject.TryGetComponent(out _collider);
            Debug.Assert(_collider != null && _collider.isTrigger, $"PlayerChecker {gameObject.name} does not have a trigger collider!");

            _isPlayerStaying = false;
            _isContainingPlayer = false;
            _remainingGracePeriod = 0f;
        }

        private void Update() {
            if (_isPlayerStaying) {
                _isPlayerStaying = false;
            } else if (_isContainingPlayer) {
                _remainingGracePeriod -= Time.deltaTime;

                if (_remainingGracePeriod <= float.Epsilon) {
                    _isContainingPlayer = false;
                }
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Player")) {
                AcknowledgePlayerStay();
            }
        }

        private void OnTriggerStay(Collider other) {
            if (other.gameObject.CompareTag("Player")) {
                AcknowledgePlayerStay();
            }
        }

        private void AcknowledgePlayerStay() {
            _isPlayerStaying = true;
            _isContainingPlayer = true;
            _remainingGracePeriod = _gracePeriod;
        }
    }
}
