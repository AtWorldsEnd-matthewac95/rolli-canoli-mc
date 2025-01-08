using System;
using UnityEngine;
using UnityEngine.UI;

namespace RolliCanoli {
    public class CanvasVictoryScreen : MonoBehaviour {
        [SerializeField]
        private Image _victoryScreenImage;

        [SerializeField]
        private Text _timerText;

        public bool IsOpen { get; private set; }

        public event Action OnDismiss;

        private void Awake() {
            _victoryScreenImage.enabled = false;
            _timerText.enabled = false;
            IsOpen = false;
        }

        private void Update() {
            if (Input.anyKey && IsOpen) {
                Dismiss();
            }
        }

        public bool Open(string timerText) {
            if (!IsOpen) {
                IsOpen = true;
                _victoryScreenImage.enabled = true;
                _timerText.enabled = true;
                _timerText.text = timerText;
            }

            return IsOpen;
        }

        public void Dismiss() {
            if (IsOpen) {
                IsOpen = false;
                OnDismiss();
            }
        }
    }
}
