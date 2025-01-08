using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RolliCanoli {
    public class OneTimeAnimation : MonoBehaviour {
        [SerializeField]
        private PlayableDirector _director;

        [SerializeField]
        [Tooltip("Play this animation anyway regardless of how this scene was entered. Used for debugging.")]
        private bool _debugPlayAnyway = false;

        private void Awake() {
            if (_director == null) {
                gameObject.TryGetComponent(out _director);
                Debug.Assert(_director != null, $"No PlayableDirector on OneTimeAnimation {gameObject.name}!");
            }

            if (!_debugPlayAnyway && !ScenePlayerStateHolder.PlayHubAnimation) {
                gameObject.SetActive(false);
            }
        }
    }
}
