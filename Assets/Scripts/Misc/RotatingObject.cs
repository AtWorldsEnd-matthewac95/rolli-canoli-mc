using UnityEngine;

namespace RolliCanoli {
    public class RotatingObject : MonoBehaviour {
        [SerializeField]
        private float _rotationalSpeed;

        private void Update() {
            transform.Rotate(Time.deltaTime * _rotationalSpeed * Vector3.up);
        }
    }
}
