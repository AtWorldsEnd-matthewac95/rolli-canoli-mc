namespace RolliCanoli {
    public static class ScenePlayerStateHolder {
        public static ScenePosition PlayerStartingPosition { get; private set; }
        public static bool PlayHubAnimation { get; private set; }

        public static void ProcessSceneTransition(SceneTransitioner transitioner) {
            PlayerStartingPosition = transitioner.PositionToSpawnAt;
            PlayHubAnimation = transitioner.PlayHubAnimation;
        }
    }
}
