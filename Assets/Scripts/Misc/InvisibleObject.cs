using UnityEngine;

namespace RolliCanoli {
    public class InvisibleObject : MonoBehaviour {
        private void Awake() {
            var meshRenderer = gameObject.GetComponent<MeshRenderer>();

            if (meshRenderer != null) {
                meshRenderer.forceRenderingOff = true;
            }
        }
    }
}
