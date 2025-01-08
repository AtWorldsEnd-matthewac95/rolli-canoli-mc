using UnityEngine;

namespace RolliCanoli {
    public class PersistentObject : MonoBehaviour {
        public void Awake() {
            Object.DontDestroyOnLoad(this);
        }
    }
}
