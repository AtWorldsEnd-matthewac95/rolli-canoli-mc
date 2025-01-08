using UnityEngine;

namespace RolliCanoli {
    public class CanvasTimerSwitch : MonoBehaviour, ISwitch {
        [SerializeField]
        private CanvasTimer _timer;

        public virtual bool IsActivatable => true;

        protected virtual void Awake() {
            Debug.Assert(_timer != null, $"Canvas timer switch {gameObject.name} does not have a reference to a canvas timer!");
        }

        protected virtual bool ActivateOnTimer(CanvasTimer timer) => true;

        public virtual bool Activate() => ActivateOnTimer(_timer);
    }
}
