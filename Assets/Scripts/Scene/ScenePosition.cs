using System;
using UnityEngine;

namespace RolliCanoli {
    [Serializable]
    public struct ScenePosition {
        [SerializeField]
        [Tooltip("Should this position be used? This is useful if the player should just begin wherever they're placed in the Unity scene editor when the scene loads.")]
        private bool _useThisPosition;

        [SerializeField]
        [Tooltip("The position to spawn the player at. Note this is world position.")]
        private Vector3 _position;

        public bool UseThisPosition => _useThisPosition;
        public Vector3 Position => _position;

        public ScenePosition (Vector3 position, bool useThisPosition = true) {
            _position = position;
            _useThisPosition = useThisPosition;
        }
    }
}
