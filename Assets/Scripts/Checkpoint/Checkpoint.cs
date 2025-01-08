using UnityEngine;

namespace RolliCanoli {
    public class Checkpoint : MonoBehaviour {
        [SerializeField]
        private GameObject _respawnPoint;

        public Vector3 RespawnPoint => _respawnPoint == null ? transform.position : _respawnPoint.transform.position;

        private void Awake() {
            var meshRenderer = gameObject.GetComponent<MeshRenderer>();

            if (meshRenderer != null) {
                meshRenderer.forceRenderingOff = true;
            }

            if (_respawnPoint != null) {
                var pointMeshRenderer = _respawnPoint.GetComponent<MeshRenderer>();

                if (pointMeshRenderer != null) {
                    pointMeshRenderer.forceRenderingOff = true;
                }
            }
        }
    }
}
