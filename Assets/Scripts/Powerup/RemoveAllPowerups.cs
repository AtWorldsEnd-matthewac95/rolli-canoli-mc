using UnityEngine;

namespace RolliCanoli {
    public class RemoveAllPowerups : MonoBehaviour, IRemovePowerup {
        public bool RemovesAllPowerups => true;

        public bool ValidateRemoval(OblongPlayerController _) => true;
    }
}
