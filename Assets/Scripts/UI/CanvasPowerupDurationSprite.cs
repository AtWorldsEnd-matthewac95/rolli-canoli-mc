using System;
using UnityEngine;

namespace RolliCanoli {
    [Serializable]
    public class CanvasPowerupDurationSprite {
        [SerializeField]
        private PowerupType _powerupType;

        [SerializeField]
        private Sprite _durationSprite;

        public PowerupType PowerupType => _powerupType;
        public Sprite DurationSprite => _durationSprite;
    }
}
