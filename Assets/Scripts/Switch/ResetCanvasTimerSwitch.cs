namespace RolliCanoli {
    public class ResetCanvasTimerSwitch : CanvasTimerSwitch {
        protected override bool ActivateOnTimer(CanvasTimer timer) {
            timer.ResetTimer();
            return true;
        }
    }
}
