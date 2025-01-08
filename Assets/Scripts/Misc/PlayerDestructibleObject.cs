using UnityEngine;

namespace RolliCanoli {
    public class PlayerDestructibleObject : MonoBehaviour {
        [SerializeField]
        private float _playerMassDestructibilityThreshold;

        [SerializeField]
        private float _playerVelocitySquaredDestructibilityThreshold;

        private void OnCollisionEnter(Collision collision) {
            if (collision.gameObject.TryGetComponent(out OblongPlayerController player)
                && player.Mass >= _playerMassDestructibilityThreshold
                && player.VelocitySquared >= _playerVelocitySquaredDestructibilityThreshold
            ) {
                gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider collider) {
            if (collider.gameObject.TryGetComponent(out OblongPlayerController player)
                && player.Mass >= _playerMassDestructibilityThreshold
                && player.VelocitySquared >= _playerVelocitySquaredDestructibilityThreshold
            ) {
                gameObject.SetActive(false);
            }
        }
    }
}
