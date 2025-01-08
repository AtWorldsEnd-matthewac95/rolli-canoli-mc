using System.Collections.Generic;
using UnityEngine;

namespace RolliCanoli {
    public class OblongPlayerPivot : MonoBehaviour {
        private const float RAY_RADIUS_MULTIPLIER = 1.5f;
        private const uint AIRBORNE_FRAME_DIVIDER = 4;
        private const uint MAX_GROUNDED_DURATION_FRAMES = 4;

        private SphereCollider _sphereCollider;
        private readonly Dictionary<int, uint> _grounds = new();
        private bool _isNearGrounded = false;
        private uint _airborneFrames = 0;

        [SerializeField]
        private OblongPlayerController _player;

        [SerializeField]
        private OblongPivotType _pivotType;

        public OblongPivotType PivotType => _pivotType;
        public bool IsGrounded => _grounds.Count > 0;
        public bool IsNearGrounded => _isNearGrounded;

        private void Start() {
            _sphereCollider = GetComponent<SphereCollider>();
            _player.UpdateGroundedStatus(this);
        }

        private void OnTriggerEnter(Collider collider) {
            if (_player.IsColliderFloor(collider)) {
                float rayDistance = DetermineRayDistance();
                if (!Physics.Raycast(transform.position, _player.LastMovementDirection, out RaycastHit rayhit, rayDistance, LayerMask.GetMask(_player.FloorTags))) {
                    Physics.Raycast(transform.position, _player.CurrentGravity, out rayhit, rayDistance, LayerMask.GetMask(_player.FloorTags));
                }

                if (rayhit.collider != null && (_player.IsClimbing || Vector3.Angle(-1f * OblongPlayerController.DefaultGravity, rayhit.normal) <= _player.AdmissivenessAngle)) {
                    _isNearGrounded = true;
                    _airborneFrames = 0;
                    _grounds.Add(collider.gameObject.GetInstanceID(), 0);
                    _player.UpdateGroundedStatus(this);
                    _player.ChangePivot(_pivotType, rayhit.normal);
                }
            }
        }

        private void OnTriggerStay(Collider other) {
            var id = other.gameObject.GetInstanceID();
            if (_grounds.ContainsKey(id)) {
                _grounds[id] = 0;
            }
        }

        private void OnTriggerExit(Collider collider) {
            _grounds.Remove(collider.gameObject.GetInstanceID());
            _player.UpdateGroundedStatus(this);
        }

        private void Update() {
            int[] keys = new int[_grounds.Count];
            _grounds.Keys.CopyTo(keys, 0);
            foreach (var key in keys) {
                _grounds[key]++;

                if (_grounds[key] >= MAX_GROUNDED_DURATION_FRAMES) {
                    _grounds.Remove(key);
                }
            }
        }

        private void FixedUpdate() {
            if (!IsGrounded && !_player.IsGrounded) {
                if (_airborneFrames > 0) {
                    _airborneFrames--;
                } else {
                    _airborneFrames = (_player.AirborneResetFrames / AIRBORNE_FRAME_DIVIDER);
                    _isNearGrounded = Physics.Raycast(transform.position, _player.CurrentGravity, RAY_RADIUS_MULTIPLIER * _player.Length, LayerMask.GetMask(_player.FloorTags));
                    _player.UpdateNearGroundedStatus(this);
                }
            }
        }

        private float DetermineRayDistance() => transform.lossyScale.magnitude * (_sphereCollider == null ? (RAY_RADIUS_MULTIPLIER * _sphereCollider.radius) : 1f);
    }
}
