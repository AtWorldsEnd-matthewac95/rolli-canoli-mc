using UnityEngine;

namespace RolliCanoli {
    public class VanishSwitch : VisibilitySwitch {
        protected override bool ActivateOnObject(GameObject obj) {
            obj.SetActive(false);
            return !obj.activeSelf;
        }

        protected override bool DeactivateOnObject(GameObject obj) {
            obj.SetActive(true);
            return obj.activeSelf;
        }
    }
}
