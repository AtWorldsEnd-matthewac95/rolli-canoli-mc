using System.Collections.Generic;
using UnityEngine;

namespace RolliCanoli {
    public class VisibilitySwitch : MonoBehaviour, ISwitch {
        protected const float MILLISECOND = 0.001f;

        protected float _activatedTime;
        protected float _deactivateTime = 0f;
        protected bool _isActivatable = true;
        protected Dictionary<int, List<GameObject>> _delayedActives = new();
        protected Dictionary<int, List<GameObject>> _earlyResets = new();

        [SerializeField]
        protected VisibilitySwitchTarget[] _targets;

        public virtual bool IsActivatable => _isActivatable;
        public float ActivatedTime => _activatedTime;

        protected virtual bool ActivateOnObject(GameObject obj) => true;
        protected virtual bool DeactivateOnObject(GameObject obj) => true;

        protected int ConvertTimeToMilli(float time) => Mathf.FloorToInt(time * 1000f);

        protected virtual void Awake() {
            Debug.Assert(gameObject.TryGetComponent(out Collider _), $"Switch {gameObject.name} has no collider!");
        }

        protected bool ActivateOnTargets(in VisibilitySwitchTarget[] targets) {
            bool success = true;
            _deactivateTime = FindDeactivateTime();

            foreach (var target in targets) {
                bool earlyReset = (target.ResetTime > MILLISECOND) && (target.ResetTime < (_deactivateTime - MILLISECOND));
                int resetMilli = earlyReset ? ConvertTimeToMilli(target.ResetTime) : 0;
                int offsetMilli = ConvertTimeToMilli(target.OffsetTime);

                foreach (var obj in target.Objects) {
                    if (target.OffsetTime < MILLISECOND) {
                        success &= ActivateOnObject(obj);
                    } else {
                        if (_delayedActives.ContainsKey(offsetMilli)) {
                            _delayedActives[offsetMilli].Add(obj);
                        } else {
                            _delayedActives[offsetMilli] = new List<GameObject> { obj };
                        }
                    }

                    if (earlyReset) {
                        if (_earlyResets.ContainsKey(resetMilli)) {
                            _earlyResets[resetMilli].Add(obj);
                        } else {
                            _earlyResets[resetMilli] = new List<GameObject> { obj };
                        }
                    }
                }
            }

            return success;
        }

        protected bool DeactivateOnTargets(in VisibilitySwitchTarget[] targets) {
            bool success = true;

            _activatedTime = 0f;
            _deactivateTime = 0f;
            _delayedActives.Clear();
            _earlyResets.Clear();

            foreach (var target in targets) {
                foreach (var obj in target.Objects) {
                    success &= DeactivateOnObject(obj);
                }
            }

            return success;
        }

        public virtual bool Activate() {
            if (_isActivatable && ActivateOnTargets(_targets)) {
                _isActivatable = false;
                _deactivateTime = FindDeactivateTime();
                return true;
            }

            return false;
        }

        public virtual bool Deactivate() {
            if (!_isActivatable && DeactivateOnTargets(_targets)) {
                _isActivatable = true;
                _activatedTime = 0f;
                _deactivateTime = 0f;
                _delayedActives.Clear();
                _earlyResets.Clear();
                return true;
            }

            return false;
        }

        public virtual float FindDeactivateTime() {
            float max = 0f;

            foreach (VisibilitySwitchTarget target in _targets) {
                max = target.ResetTime > max ? target.ResetTime : max;
            }

            return max;
        }

        protected virtual void Update() {
            if (!_isActivatable && _deactivateTime > MILLISECOND) {
                _activatedTime += Time.deltaTime;

                if (_activatedTime >= _deactivateTime) {
                    _activatedTime = 0f;
                    _deactivateTime = 0f;
                    Deactivate();
                }

                HandleEarlyResets();
                HandleDelayedActives();
            }
        }

        private void HandleDelayedActives() {
            if (_delayedActives.Count > 0) {
                int activatedMillis = ConvertTimeToMilli(_activatedTime);
                HashSet<int> activations = new();

                foreach (var delay in _delayedActives.Keys) {
                    if (delay <= activatedMillis) {
                        activations.Add(delay);
                    }
                }

                foreach (var activation in activations) {
                    bool removeActivations = true;

                    foreach (var obj in _delayedActives[activation]) {
                        removeActivations &= ActivateOnObject(obj);
                    }

                    if (removeActivations) {
                        _delayedActives.Remove(activation);
                    }
                }
            }
        }

        private void HandleEarlyResets() {
            if (_earlyResets.Count > 0) {
                int activationMillis = ConvertTimeToMilli(_activatedTime);
                HashSet<int> resets = new();

                foreach (var early in _earlyResets.Keys) {
                    if (early <= activationMillis) {
                        resets.Add(early);
                    }
                }

                foreach (var reset in resets) {
                    bool removeReset = true;

                    foreach (var obj in _earlyResets[reset]) {
                        removeReset &= DeactivateOnObject(obj);
                    }

                    if (removeReset) {
                        _earlyResets.Remove(reset);
                    }
                }
            }
        }
    }
}
