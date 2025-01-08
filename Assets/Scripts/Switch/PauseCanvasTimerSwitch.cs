using UnityEngine;

namespace RolliCanoli {
    public class PauseCanvasTimerSwitch : CanvasTimerSwitch {
        [SerializeField]
        private bool _unpauseTimer;

        protected override bool ActivateOnTimer(CanvasTimer timer) => _unpauseTimer ? timer.Unpause() : timer.Pause();
    }
}
