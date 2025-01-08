using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RolliCanoli {
    public class CanvasPowerupDuration : MonoBehaviour {
        private Dictionary<PowerupType, CanvasPowerupDurationSprite> _durationSpriteMap;
        private IPowerupDuration _powerupDuration;
        private CanvasPowerupDurationSprite _currentDurationSprite;
        private bool _isMore;
        private bool _unconditionalChange;
        private Image _image;

        [SerializeField]
        private CanvasPowerupDurationSprite[] _durationSprites;

        [SerializeField]
        private Sprite _moreSprite;

        private void Start() {
            gameObject.TryGetComponent(out _image);
            Debug.Assert(_image != null, $"CanvasPowerupDuration {gameObject.name} doesn't have an Image component!");

            _durationSpriteMap = new();
            foreach (var sprite in _durationSprites) {
                _durationSpriteMap[sprite.PowerupType] = sprite;
            }

            _image.type = Image.Type.Filled;
            _image.fillMethod = Image.FillMethod.Radial360;
            _image.fillClockwise = false;
            _unconditionalChange = false;

            _powerupDuration = null;
            _currentDurationSprite = null;
        }

        private void Update() {
            _image.enabled = _isMore || _powerupDuration != null;

            if (_isMore) {
                _image.sprite = _moreSprite;
            } else if (_powerupDuration == null) {
                _image.sprite = null;
                _currentDurationSprite = null;
            } else {
                if (_unconditionalChange || _powerupDuration.PowerupType != _currentDurationSprite?.PowerupType) {
                    _unconditionalChange = false;
                    _currentDurationSprite = _durationSpriteMap[_powerupDuration.PowerupType];
                    _image.sprite = _currentDurationSprite.DurationSprite;
                }

                _image.fillAmount = _powerupDuration.PercentIncomplete;
            }
        }

        public bool PairDuration(IPowerupDuration duration) {
            _unconditionalChange = _isMore;
            _isMore = false;
            bool changed = _powerupDuration != duration;
            _powerupDuration = duration;
            return changed;
        }

        public bool UnpairDuration() {
            _isMore = false;
            bool wasPaired = _powerupDuration != null;
            _powerupDuration = null;
            return wasPaired;
        }

        public void SetMore(bool newval) {
            _isMore = newval;

            if (_isMore) {
                _image.fillAmount = 1f;
            }
        }
    }
}
