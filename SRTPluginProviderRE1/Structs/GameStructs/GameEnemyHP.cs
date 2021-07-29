using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SRTPluginProviderRE1.Structs.GameStructs
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x8)]
    [DebuggerDisplay("{_DebuggerDisplay,nq}")]
    public struct GameEnemyHP
    {
        [FieldOffset(0x0)] private int currentHP;
        [FieldOffset(0x4)] private int maxHP;
        /// <summary>
        /// Debugger display message.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string _DebuggerDisplay
        {
            get
            {
                if (IsTrigger)
                    return string.Format("TRIGGER", CurrentHP, MaximumHP, Percentage);
                else if (IsAlive)
                    return string.Format("{0} / {1} ({2:P1})", CurrentHP, MaximumHP, Percentage);
                else
                    return "DEAD / DEAD (0%)";
            }
        }

        public int CurrentHP => currentHP;
        public int MaximumHP => maxHP;

        public bool IsTrigger => MaximumHP <= 100 || MaximumHP >= 65535 || CurrentHP > MaximumHP; // Some triggers load in as enemies as 100/100 hp. We're excluding those.
        public bool IsAlive => !IsTrigger && CurrentHP != 0 && MaximumHP != 0 && CurrentHP > 0 && CurrentHP <= MaximumHP;
        public bool IsDamaged => MaximumHP > 0 && CurrentHP > 0 && CurrentHP < MaximumHP;
        public float Percentage => IsAlive ? (float)CurrentHP / (float)MaximumHP : 0f;
 
    }
}
