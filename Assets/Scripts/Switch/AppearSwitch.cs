using UnityEngine;

namespace RolliCanoli {
    public class AppearSwitch : VisibilitySwitch {
        [SerializeField]
        private bool _autoVanishTargetsOnStart = true;

        private void Start() {
            if (_autoVanishTargetsOnStart) {
                foreach (var target in _targets) {
                    foreach (var obj in target.Objects) {
                        obj.SetActive(false);
                    }
                }
            }
        }

        protected override bool ActivateOnObject(GameObject obj) {
            obj.SetActive(true);
            return obj.activeSelf;
        }

        protected override bool DeactivateOnObject(GameObject obj) {
            obj.SetActive(false);
            return !obj.activeSelf;
        }
    }
}
