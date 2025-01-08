using System;
using UnityEngine;

namespace RolliCanoli {
    [Serializable]
    public struct VisibilitySwitchTarget {
        [SerializeField]
        private GameObject[] _objects;
        [SerializeField]
        private float _offsetTime;
        [SerializeField]
        private float _resetTime;

        public GameObject[] Objects {
            get {
                var copy = new GameObject[_objects.Length];
                _objects.CopyTo(copy, 0);
                return copy;
            }
        }

        public float OffsetTime => _offsetTime;
        public float ResetTime => _resetTime;
    }
}
