using System;
using UnityEngine;

namespace RolliCanoli {
    [Serializable]
    public class PlayerPowerupVisualization {
        [SerializeField]
        private PowerupType _powerupType;

        [SerializeField]
        private Material _powerupMaterial;

        public PowerupType PowerupType => _powerupType;
        public Material PowerupMaterial => _powerupMaterial;
    }
}
