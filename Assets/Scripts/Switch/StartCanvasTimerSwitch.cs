using UnityEngine;

namespace RolliCanoli {
    public class StartCanvasTimerSwitch : CanvasTimerSwitch {
        [SerializeField]
        private bool _setTimerToDescending = false;

        [SerializeField]
        private double _newMaxTime = 0.0;

        protected override bool ActivateOnTimer(CanvasTimer timer) => _setTimerToDescending ? timer.StartTimerDescending(_newMaxTime) : timer.StartTimerAscending(_newMaxTime);
    }
}
