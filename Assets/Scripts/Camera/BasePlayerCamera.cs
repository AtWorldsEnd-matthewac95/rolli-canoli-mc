using UnityEngine;

namespace RolliCanoli {
    public class BasePlayerCamera : MonoBehaviour {
        [SerializeField]
        protected float _height = 2f;

        [SerializeField]
        protected float _distance = 15f;

        [SerializeField]
        protected OblongPlayerController _player;

        protected virtual bool IsMovementFixed() => false;
        protected virtual Vector3 FindDesiredPosition() => transform.position;

        private bool IsPlayerActive => _player != null && _player.gameObject.activeSelf;

        protected virtual void Update() {
            if (!IsMovementFixed() && IsPlayerActive) {
                transform.position = FindDesiredPosition();
                transform.LookAt(_player.transform.position);
            }
        }

        protected virtual void FixedUpdate() {
            if (IsMovementFixed() && IsPlayerActive) {
                transform.position = FindDesiredPosition();
                transform.LookAt(_player.transform.position);
            }
        }
    }
}
