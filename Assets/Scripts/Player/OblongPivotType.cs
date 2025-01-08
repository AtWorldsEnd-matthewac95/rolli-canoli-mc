using System.Diagnostics;

namespace RolliCanoli {
    public enum OblongPivotType : uint {
        None = 0,
        Top,
        Bottom,
        __COUNT,
        __MAX = 32
    }

    public static class OblongPivotTypeExtensions {
        static OblongPivotTypeExtensions() {
            Debug.Assert(OblongPivotType.__COUNT >= OblongPivotType.__MAX, $"Too many OblongPivotType members! Max is {(uint)OblongPivotType.__MAX}");
        }

        public static bool IsRelevant(this OblongPivotType opt) => (opt == OblongPivotType.Top || opt == OblongPivotType.Bottom);
    }
}
