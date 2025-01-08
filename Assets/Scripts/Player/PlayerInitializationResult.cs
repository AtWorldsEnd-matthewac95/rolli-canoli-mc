namespace RolliCanoli {
    public struct PlayerInitializationResult {
        public bool IsCanvasPowerupsInitialized { get; private set; }

        public PlayerInitializationResult(bool isCanvasPowerupsInitialized) {
            IsCanvasPowerupsInitialized = isCanvasPowerupsInitialized;
        }
    }
}
