using UnityEngine;

namespace RolliCanoli {
    public class CanvasPowerupDurationManager : MonoBehaviour {
        private const float INVALID_CONFIGURATION_REMINDER_TIME = 5f;
        private const int MINIMUM_ELEMENT_COUNT = 2;

        private PowerupDurationCollection _playerPowerups = null;
        private float invalidConfigurationReminderTime = 0f;

        [SerializeField]
        private CanvasPowerupDuration[] _canvasDurations;

        private void Update() {
            if (_playerPowerups == null || _canvasDurations.Length < MINIMUM_ELEMENT_COUNT) {
                if (invalidConfigurationReminderTime < float.Epsilon) {
                    invalidConfigurationReminderTime = INVALID_CONFIGURATION_REMINDER_TIME;
                    Debug.Assert(_playerPowerups != null, $"CanvasPowerupDurationManager {gameObject.name} does not have a reference to a powerup collection!");
                    Debug.Assert(_canvasDurations.Length >= MINIMUM_ELEMENT_COUNT, $"CanvasPowerupDurationManager {gameObject.name} has less than {MINIMUM_ELEMENT_COUNT} canvas durations to manage!");
                } else {
                    invalidConfigurationReminderTime -= Time.deltaTime;
                }
            } else {
                int count = 0;
                int length = _canvasDurations.Length;

                foreach (var playerPowerup in _playerPowerups) {
                    if (count >= length) {
                        break;
                    }

                    _canvasDurations[count++].PairDuration(playerPowerup);
                }

                if (count < _playerPowerups.Count) {
                    _canvasDurations[count - 1].SetMore(true);
                } else {
                    for (int i = count; i < length; i++) {
                        _canvasDurations[i].UnpairDuration();
                    }
                }
            }
        }

        public bool InitializePlayerPowerups(PowerupDurationCollection playerPowerups) {
            bool hadNoPlayerPowerups = _playerPowerups == null;

            if (hadNoPlayerPowerups) {
                _playerPowerups = playerPowerups;
            }

            return hadNoPlayerPowerups;
        }
    }
}
