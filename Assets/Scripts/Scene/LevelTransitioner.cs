using UnityEngine;

namespace RolliCanoli {
    public class LevelTransitioner : MonoBehaviour {
        [SerializeField]
        private CanvasVictoryScreen _victoryScreen;

        [SerializeField]
        private SceneTransitioner _nextScene;

        private Timer _timer;

        private void Start() {
            _timer = new();
            _timer.StartTimerAscending();

            _victoryScreen.OnDismiss += _nextScene.LoadScene;
        }

        private void OnTriggerEnter(Collider other) {
            if (!_victoryScreen.IsOpen && other.CompareTag("Player")) {
                var timerString = _timer.ToString();

                if (_victoryScreen.Open(timerString)) {
                    _timer.Pause();
                }
            }
        }
    }
}
