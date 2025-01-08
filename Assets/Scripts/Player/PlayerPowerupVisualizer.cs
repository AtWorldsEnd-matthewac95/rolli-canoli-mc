using System;
using System.Collections.Generic;
using UnityEngine;

namespace RolliCanoli {
    public class PlayerPowerupVisualizer : MonoBehaviour {
        private Dictionary<PowerupType, Material> _visualizationMap;

        [SerializeField]
        private MeshRenderer _meshRenderer;

        [SerializeField]
        private PlayerPowerupVisualization[] _visualizations;

        private void Awake() {
            _visualizationMap = new();

            foreach (var visualization in _visualizations) {
                _visualizationMap[visualization.PowerupType] = visualization.PowerupMaterial;
            }

            Debug.Assert(_meshRenderer != null, $"PlayerPowerupVisualizer {gameObject.name} does not have a MeshRenderer!");

            if (!_visualizationMap.ContainsKey(PowerupType.None)) {
                _visualizationMap[PowerupType.None] = _meshRenderer.material;
            }
        }

        public bool VisualizePowerup(PowerupType powerup) {
            bool containsPowerup = _visualizationMap.ContainsKey(powerup);
            _meshRenderer.material = _visualizationMap[containsPowerup ? powerup : PowerupType.None];
            return containsPowerup;
        }
    }
}
