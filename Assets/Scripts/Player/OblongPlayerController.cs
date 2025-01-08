using UnityEngine;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace RolliCanoli {
    public class OblongPlayerController : MonoBehaviour {
        public static Vector3 DefaultGravity = Physics.gravity;

        private Rigidbody _rigidbody;
        private bool _isBottomPivotAnchor;
        private BitVector32 _grounded;
        private BitVector32 _nearGrounded;
        private uint _airborneFrames;
        private Vector3 _gravity;
        private Quaternion _gravityRotation;
        private Vector3 _lastMovementDirection;
        private Vector3 _lastHorizontalDirection;
        private PowerupDurationCollection _powerups;
        private Vector3 _lastGravity;
        private Checkpoint _lastCheckpoint;
        private int _currentPowerupCount;
        private int _currentPowerupEffectiveCount;
        private Vector3 _initialPosition;
        private Vector3 _lastPosition;
        private PlayerPowerupVisualizer _powerupVisualizer;

        private float __length;
        private float __gravityMagnitude;

        [SerializeField]
        [Tooltip("Array of string tags which the player considers to be a valid floor to stand on. These tags should be on objects the player should stand on (or climb on).")]
        private string[] _floorTags;

        [SerializeField]
        [Tooltip("Transform of the top pivot. The transform should be a child of the player and the position of the transform should be on the surface of or within the player model.")]
        private Transform _topPivot;

        [SerializeField]
        [Tooltip("Transform of the bottom pivot. This pivot will be the default anchor. The transform should be a child of the player and the position of the transform should be on the surface of or within the player model.")]
        private Transform _bottomPivot;

        [SerializeField]
        [Tooltip("Amount of force which should be applied towards moving. Even though the force is applied linearly at a pivot, the force should still result in rotational movement.")]
        private float _movementForce = 10f;

        [SerializeField]
        [Tooltip("Amount of torque which should be applied while moving.")]
        private float _movementTorque = 2f;

        [SerializeField]
        [Tooltip("Multiplier on the gravity force which is applied while moving. Used to prevent the player from \"skipping\" into the air while rolling.")]
        private float _movementGravityMultiplier = 2f;

        [SerializeField]
        [Tooltip("If greater than 1, makes the player come to a halt faster when they're no longer giving input and are grounded. Does the opposite if less than 1.")]
        private float _stoppingVelocityMultiplier = 2f;

        [SerializeField]
        [Tooltip("If the player is airborne for this many frames, their gravity will be reset back to the default.")]
        private uint _airborneResetFrames = 30;

        [SerializeField]
        [Tooltip("If the angle between a contacted surface's normal and the default normal is less than this amount, then the player will stick to that surface.")]
        private float _admissivenessAngle = 80f;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Used to straighten the player when they're not moving. This is a ratio of the current gravity force which will be inverted and applied to the guide pivot (The pivot opposite to the anchor pivot).")]
        private float _straighteningScale = 0.5f;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("When 1, strictly horizontal (that is, perpendicular to the normal) force will be used to move the player. When 0, strictly tangential force is used. Any value in between is a blend of those two forces.")]
        private float _horizontalOverrideScale = 0.5f;

        [SerializeField]
        [Tooltip("When set, the player will use this position to determine what \"forward\" is.")]
        private Transform _movementReference;

        [SerializeField]
        [Tooltip("The player grows by a set amount for each powerup they've accumulated. When the player has this many power ups, they'll have doubled in size.")]
        private float _doubleSizePowerupCount = 8;

        [SerializeField]
        [Tooltip("UI manager which handles displaying powerup durations to the player.")]
        private CanvasPowerupDurationManager _canvasDurationManager;

        public string[] FloorTags {
            get {
                var copy = new string[_floorTags.Length];
                Array.Copy(_floorTags, copy, copy.Length);
                return copy;
            }
        }

        public bool IsGrounded => _grounded.Data > 0;
        public bool IsNearGrounded => _nearGrounded.Data > 0;
        public Vector3 CurrentGravity => _gravity;
        public Vector3 LastMovementDirection => _lastMovementDirection;
        public Vector3 LastHorizontalDirection => _lastHorizontalDirection;
        public float AdmissivenessAngle => _admissivenessAngle;
        public OblongPivotType CurrentAnchor => _isBottomPivotAnchor ? OblongPivotType.Bottom : OblongPivotType.Top;
        public bool IsClimbing => _powerups.TryPeek(out CaramelDuration duration) && !duration.IsComplete;
        public uint AirborneResetFrames => _airborneResetFrames;
        public float Length => __length * Scale;
        public float Scale => 1f + (_currentPowerupEffectiveCount / _doubleSizePowerupCount);
        public float Mass => 1f + _currentPowerupEffectiveCount;
        public float VelocitySquared => _rigidbody.velocity.sqrMagnitude;

        private void Awake() {
            if (ScenePlayerStateHolder.PlayerStartingPosition.UseThisPosition) {
                transform.position = ScenePlayerStateHolder.PlayerStartingPosition.Position;
            }
        }

        private void Start() {
            var initialized = InitializeFields();
            AssertStart(initialized);
        }

        private PlayerInitializationResult InitializeFields() {
            _isBottomPivotAnchor = true;
            _grounded = new BitVector32();
            _airborneFrames = 0;
            _gravity = DefaultGravity;
            _lastMovementDirection = Vector3.zero;
            _powerups = new PowerupDurationCollection();
            _lastGravity = DefaultGravity;
            _initialPosition = transform.position;
            _lastPosition = transform.position;
            _currentPowerupCount = 0;
            _currentPowerupEffectiveCount = 0;

            __gravityMagnitude = _gravity.magnitude;
            __length = (_topPivot.position - _bottomPivot.position).magnitude;

            gameObject.TryGetComponent(out _rigidbody);
            gameObject.TryGetComponent(out _powerupVisualizer);

            return new PlayerInitializationResult(isCanvasPowerupsInitialized: _canvasDurationManager.InitializePlayerPowerups(_powerups));
        }

        private void AssertStart(in PlayerInitializationResult initialized) {
            Debug.Assert(_rigidbody != null, "Player has no Rigidbody component!");
            Debug.Assert(_powerupVisualizer != null, "Player has no PlayerPowerupVisualizer component!");
            Debug.Assert(_topPivot != null, "Player has no top pivot!");
            Debug.Assert(_bottomPivot != null, "Player has no bottom pivot!");
            Debug.Assert(_canvasDurationManager != null, "Player can't initialize the canvas duration manager!");
            Debug.Assert(initialized.IsCanvasPowerupsInitialized, "Canvas duration manager initialization failed!");
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.R)) {
                Respawn();
            }
        }

        public void Respawn() {
            transform.position = _lastCheckpoint == null ? _initialPosition : _lastCheckpoint.RespawnPoint;
        }

        private void FixedUpdate() {
            HandlePlayerAppearance();

            HandleClimbingFrames();
            HandleAirborneFrames();

            bool isMoving = DetermineMovementDirection(out Vector3 direction, out Vector3 horizontal);
            var anchor = _isBottomPivotAnchor ? _bottomPivot.position : _topPivot.position;
            var guide = _isBottomPivotAnchor ? _topPivot.position : _bottomPivot.position;

            if (isMoving) {
                _rigidbody.AddForceAtPosition((IsGrounded ? _movementGravityMultiplier : 1f) * _gravity, anchor, ForceMode.Acceleration);
                _rigidbody.AddForceAtPosition(direction, guide, ForceMode.Acceleration);
                _rigidbody.AddTorque(_movementTorque * Vector3.Cross(horizontal, _gravity).normalized, ForceMode.Acceleration);
                _lastMovementDirection = direction;
                _lastHorizontalDirection = horizontal;
            } else {
                _rigidbody.AddForceAtPosition((1f + (IsGrounded ? 0f : _straighteningScale)) * _gravity, anchor, ForceMode.Acceleration);
                _rigidbody.AddForceAtPosition(-1f * _straighteningScale * _gravity, guide, ForceMode.Acceleration);

                if (IsGrounded) {
                    _rigidbody.AddForceAtPosition(_stoppingVelocityMultiplier * Time.fixedDeltaTime * DetermineStoppingVelocity(), guide, ForceMode.VelocityChange);
                }
            }

            if (CheckForStuckMovement(direction)) {
                _isBottomPivotAnchor = !_isBottomPivotAnchor;
            }
        }

        private void OnTriggerEnter(Collider collider) {
            var other = collider.gameObject;

            if (other.CompareTag("Powerup")) {
                _powerups.Add(other.GetComponent<IPowerup>());
            }

            if (other.CompareTag("Marshmellow")) {
                HandleMarshmellowBounce(other.GetComponent<Marshmellow>());
            }

            if (other.CompareTag("Checkpoint")) {
                var checkpoint = other.GetComponent<Checkpoint>();
                Debug.Assert(checkpoint != null, $"Checkpoint {other.name} has no checkpoint script!");

                if (checkpoint != null) {
                    _lastCheckpoint = checkpoint;
                }
            }

            if (other.CompareTag("Switch")) {
                other.GetComponent<ISwitch>().Activate();
            }

            if (other.CompareTag("Scene")) {
                other.GetComponent<SceneTransitioner>().LoadScene();
            }

            if (other.CompareTag("Killbox")) {
                Respawn();
            }

            if (other.CompareTag("RemovePowerup")) {
                var remover = other.GetComponent<IRemovePowerup>();
                Debug.Assert(remover != null, $"{other.name} is tagged with \"RemovePowerup\" but has no corresponding script!");

                if (remover != null && remover.ValidateRemoval(this)) {
                    if (remover.RemovesAllPowerups) {
                        _powerups.Clear();
                    } else {
                        _powerups.RemoveTop();
                    }
                }
            }
        }

        private bool HandleMarshmellowBounce(Marshmellow marshmellow) {
            if (marshmellow == null || marshmellow.OnCooldown) {
                return false;
            }

            _rigidbody.velocity = Vector3.zero;
            _rigidbody.AddForceAtPosition(2f * marshmellow.GiveUpwardForce(), transform.position, ForceMode.VelocityChange);
            _isBottomPivotAnchor = !_isBottomPivotAnchor;

            return true;
        }

        private void HandlePlayerAppearance() {
            int count = _powerups.Count;

            if (count != _currentPowerupCount) {
                _currentPowerupCount = count;
                _currentPowerupEffectiveCount = _powerups.FindEffectiveCount();
                transform.localScale = new Vector3(Scale, Scale, Scale);
                _rigidbody.mass = Mass;
                _powerupVisualizer.VisualizePowerup(_powerups.TryPeek(out IPowerupDuration duration) ? duration.PowerupType : PowerupType.None);
            }
        }

        private bool HandleAirborneFrames() {
            bool resetFramesReached = _airborneFrames >= _airborneResetFrames;
            bool changeGravity = !IsNearGrounded;

            if (!IsGrounded && (_airborneFrames < _airborneResetFrames)) {
                _airborneFrames++;
                resetFramesReached = _airborneFrames >= _airborneResetFrames;
                changeGravity |= resetFramesReached;
            }

            if (changeGravity) {
                _lastGravity = _gravity;
                _gravity = DefaultGravity;
                _gravityRotation = Quaternion.FromToRotation(DefaultGravity, _gravity);
            }

            return resetFramesReached;
        }

        private int HandleClimbingFrames() {
            CaramelDuration duration = null;

            if (IsCurrentGravityInadmissible()) {
                if (_powerups.TryPeek(out duration)) {
                    duration--;
                }

                if (!_powerups.TryPeek(out CaramelDuration _)) {
                    ReleaseClimbing();
                }
            }

            return duration?.RemainingFrames ?? 0;
        }

        private bool DetermineMovementDirection (out Vector3 direction, out Vector3 horizontal) {
            direction = Vector3.zero;
            horizontal = Vector3.zero;

            var input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            bool isMoving = (Mathf.Abs(input.x) + Mathf.Abs(input.z)) > float.Epsilon;

            if (isMoving) {
                var unit = input.normalized;
                var referenceRotation = Quaternion.FromToRotation(Vector3.forward, DetermineForwardFromReference());
                horizontal = (unit.x * (_gravityRotation * (referenceRotation * Vector3.right))) + (unit.z * (_gravityRotation * (referenceRotation * Vector3.forward)));

                float angle = Mathf.Deg2Rad * Vector3.Angle(-1f * horizontal, (_isBottomPivotAnchor ? 1f : -1f) * (_topPivot.position - _bottomPivot.position));
                direction = _movementForce * (
                    (_horizontalOverrideScale * horizontal) + (
                        (1f - _horizontalOverrideScale)
                        * ((Mathf.Cos(angle) * -1f * _gravity.normalized) + (Mathf.Sin(angle) * horizontal))
                    )
                );
            }

            return isMoving;
        }

        private Vector3 DetermineForwardFromReference() {
            if (_movementReference == null) {
                return Vector3.forward;
            }

            var diff = transform.position - _movementReference.transform.position;
            return new Vector3(diff.x, 0f, diff.z);
        }

        private bool CheckForStuckMovement(in Vector3 direction)
            => _grounded[(int)OblongPivotType.Top] && _grounded[(int)OblongPivotType.Bottom] && (Vector3.Dot(_gravity, direction) > float.Epsilon);

        private Vector3 DetermineStoppingVelocity() {
            var velocity = _rigidbody.velocity;
            var verticalVelocity = Vector3.Project(velocity, _gravity);
            return verticalVelocity - velocity;
        }

        private bool ReleaseClimbing() {
            bool gravityChanged = false;

            if (IsCurrentGravityInadmissible()) {
                _grounded[(int)CurrentAnchor] = false;
                _gravity = _lastGravity;
                _lastGravity = DefaultGravity;
                _isBottomPivotAnchor = !_isBottomPivotAnchor;

                if (IsCurrentGravityInadmissible()) {
                    _grounded[(int)CurrentAnchor] = false;
                    _gravity = DefaultGravity;
                }

                _gravityRotation = Quaternion.FromToRotation(DefaultGravity, _gravity);
                gravityChanged = true;
            }

            return gravityChanged;
        }

        private bool IsCurrentGravityInadmissible() => Vector3.Angle(DefaultGravity, _gravity) > _admissivenessAngle;

        public bool IsColliderFloor(Collider collider) {
            bool isFloor = false;

            foreach (var tag in _floorTags) {
                if (collider.CompareTag(tag)) {
                    isFloor = true;
                    break;
                }
            }

            return isFloor;
        }

        public bool ChangePivot(in OblongPivotType newPivot, in Vector3 normal) {
            bool changeActuallyMade = false;

            if (newPivot.IsRelevant() && (_isBottomPivotAnchor ^ (newPivot == OblongPivotType.Bottom))) {
                _lastGravity = _gravity;
                _isBottomPivotAnchor = !_isBottomPivotAnchor;
                _gravity = -1f * __gravityMagnitude * normal.normalized;
                _gravityRotation = Quaternion.FromToRotation(DefaultGravity, _gravity);
                changeActuallyMade = true;
            }

            return changeActuallyMade;
        }

        public bool UpdateGroundedStatus(OblongPlayerPivot pivot) {
            if (pivot.PivotType.IsRelevant()) {
                int p = (int)pivot.PivotType;
                _grounded[p] = pivot.IsGrounded;

                if (_grounded[p]) {
                    _nearGrounded[p] = true;
                    _airborneFrames = 0;
                }
            }

            return IsGrounded;
        }

        public bool UpdateNearGroundedStatus(OblongPlayerPivot pivot) {
            if (pivot.PivotType.IsRelevant()) {
                int p = (int)pivot.PivotType;
                _nearGrounded[p] = pivot.IsNearGrounded;
            }

            return IsNearGrounded;
        }
    }
}
