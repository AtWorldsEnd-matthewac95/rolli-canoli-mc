using UnityEngine;

namespace RolliCanoli {
    public interface ISwitch {
        bool IsActivatable { get; }

        bool Activate();
    }
}
