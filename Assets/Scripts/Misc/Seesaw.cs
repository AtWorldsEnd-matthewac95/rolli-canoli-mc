using UnityEngine;

namespace RolliCanoli {
    public class Seesaw : MonoBehaviour {
        private Rigidbody _rigidbody;
        private Vector3 _leftStraighteningDirection;
        private Vector3 _rightStraighteningDirection;

        [SerializeField]
        [Range(0f, 90f)]
        private float _maxRotation = 75f;

        [SerializeField]
        private Transform _leftAnchor;

        [SerializeField]
        private Transform _rightAnchor;

        [SerializeField]
        private float _straighteningForce = 10f;

        [SerializeField]
        private float _resistanceForce = 1f;

        [SerializeField]
        private PlayerChecker _playerChecker;

        private void Start() {
            _leftStraighteningDirection = -1f * _leftAnchor.right.normalized;
            _rightStraighteningDirection = _rightAnchor.right.normalized;

            gameObject.TryGetComponent(out _rigidbody);
            Debug.Assert(_rigidbody != null, $"Seesaw {gameObject.name} has no rigidbody!");
        }

        private void FixedUpdate() {
            HandleAngle();
            HandleStraightening();
        }

        private void HandleAngle() {
            var angle = transform.localEulerAngles.z;

            while (angle >= 180f) {
                angle -= 360f;
            }

            while (angle <= -180f) {
                angle += 360f;
            }

            transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Clamp(angle, -1f * _maxRotation, _maxRotation));
        }

        private void HandleStraightening() {
            var force = (_playerChecker != null && !_playerChecker.IsContainingPlayer) ? _straighteningForce : _resistanceForce;

            _rigidbody.AddForceAtPosition(force * _leftStraighteningDirection, _leftAnchor.position, ForceMode.Acceleration);
            _rigidbody.AddForceAtPosition(force * _rightStraighteningDirection, _rightAnchor.position, ForceMode.Acceleration);
        }
    }
}
