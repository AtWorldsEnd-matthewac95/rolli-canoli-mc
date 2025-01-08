using UnityEngine;

namespace RolliCanoli {
    public class Marshmellow : MonoBehaviour {
        [SerializeField]
        private float _upwardForce = 100f;

        [SerializeField]
        private int _cooldownFrames = 30;

        [SerializeField]
        private AudioSource _bounceSound;

        private int _currentCooldownFrame = 0;

        public bool OnCooldown => _currentCooldownFrame > 0;

        private void Start() {
            if (_bounceSound == null && !gameObject.TryGetComponent(out _bounceSound)) {
                Debug.Log($"Marshmellow {gameObject.name} has no audio source! This object will not play sounds.");
            }
        }

        private void FixedUpdate() {
            if (_currentCooldownFrame > 0) {
                _currentCooldownFrame--;
            }
        }

        public Vector3 GiveUpwardForce() {
            if (OnCooldown) {
                return Vector3.zero;
            }

            if (_bounceSound != null) {
                _bounceSound.Play();
            }

            _currentCooldownFrame = _cooldownFrames;
            return _upwardForce * transform.up;
        }
    }
}
