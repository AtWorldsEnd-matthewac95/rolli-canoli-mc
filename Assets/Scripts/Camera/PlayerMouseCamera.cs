using UnityEngine;

namespace RolliCanoli {
    public class PlayerMouseCamera : BasePlayerCamera {
        [SerializeField]
        [Range(3f, 15f)]
        private float _sensitivity = 6f;

        [SerializeField]
        private float _minDistance = 5f;

        [SerializeField]
        private float _maxDistance = 20f;

        [SerializeField]
        [Range(0f, 2f)]
        private float _scrollSensitivity = 1f;

        [SerializeField]
        private bool _invertCamera = false;

        [SerializeField]
        private float _horizontalAngle = 0f;

        [SerializeField]
        private float _verticalAngle = 0f;

        [SerializeField]
        private float _maxVerticalAngle = 75f;

        [SerializeField]
        private float _minVerticalAngle = -5f;

        private Vector3 _previousMousePosition;
        private float _initialPlayerLength;

        private void Start() {
            _previousMousePosition = Input.mousePosition;
            _initialPlayerLength = _player.Length;
        }

        protected override Vector3 FindDesiredPosition() {
            var mousePosition = Input.mousePosition;
            var mouseDelta = mousePosition - _previousMousePosition;
            mouseDelta = (mouseDelta.magnitude > _sensitivity) ? (_sensitivity * mouseDelta.normalized) : mouseDelta;
            _distance -= _scrollSensitivity * Input.mouseScrollDelta.y;
            _distance = Mathf.Clamp(_distance, _minDistance, _maxDistance);
            _horizontalAngle += (_invertCamera ? -1f : 1f) * mouseDelta.x;
            _verticalAngle += (_invertCamera ? -1f : 1f) * mouseDelta.y;
            _verticalAngle = Mathf.Clamp(_verticalAngle, _minVerticalAngle, _maxVerticalAngle);

            var radianHorizontalAngle = Mathf.Deg2Rad * _horizontalAngle;
            var radianVerticalAngle = Mathf.Deg2Rad * _verticalAngle;

            var newPosition = _previousMousePosition + mouseDelta;

            if ((mousePosition - newPosition).magnitude >= _sensitivity) {
                newPosition = mousePosition - mouseDelta;
            }

            _previousMousePosition = newPosition;

            return _player.transform.position + (_height * Vector3.up) + ((_distance + ((_player.Length - _initialPlayerLength) / 2f)) * new Vector3(
                Mathf.Cos(radianVerticalAngle) * Mathf.Sin(radianHorizontalAngle),
                Mathf.Sin(radianVerticalAngle),
                -1f * Mathf.Cos(radianVerticalAngle) * Mathf.Cos(radianHorizontalAngle))
            );
        }

        public void SetCameraInvert(bool invertOn)
        {
            _invertCamera = invertOn;
        }
    }
}
