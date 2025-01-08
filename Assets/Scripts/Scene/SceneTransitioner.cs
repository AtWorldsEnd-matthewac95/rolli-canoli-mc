using UnityEngine;
using UnityEngine.SceneManagement;

namespace RolliCanoli {
    public class SceneTransitioner : MonoBehaviour {
        [SerializeField]
        [Tooltip("Name of the scene to load. Note, if the scene to load is not directly in the \"Scenes\" folder but is in a sub-folder, the entire path from the Scenes folder must be entered here.")]
        private string _sceneToLoad;

        [SerializeField]
        [Tooltip("Position the player should appear at when the scene is loaded. If the \"Use This Position\" flag is not set, then the player will spawn at whatever location they were placed at in the Unity scene editor.")]
        private ScenePosition _positionToSpawnAt;

        [SerializeField]
        [Tooltip("Should the hub animation play after this scene transition?")]
        private bool _playHubAnimation = false;

        public ScenePosition PositionToSpawnAt => _positionToSpawnAt;
        public bool PlayHubAnimation => _playHubAnimation;

        protected virtual void Awake() {
            if (gameObject.TryGetComponent<MeshRenderer>(out var meshRenderer)) {
                meshRenderer.forceRenderingOff = true;
            }
        }

        public virtual void LoadScene() {
            ScenePlayerStateHolder.ProcessSceneTransition(this);
            SceneManager.LoadSceneAsync($"Scenes/{_sceneToLoad}");
        }
    }
}
