using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SRTPluginProviderRE1.Structs.GameStructs
{
    [DebuggerDisplay("{_DebuggerDisplay,nq}")]
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x8)]
    public struct GameInventoryEntry
    {
        [FieldOffset(0x0)] private int item;
        [FieldOffset(0x4)] private int quantity;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string _DebuggerDisplay
        {
            get => string.Format("[#{0}] Name: {1} Quantity: {2}", (int)Item, ItemName, Quantity);
        }

        public ItemEnumeration Item => (ItemEnumeration)item;
        public string ItemName => Item.ToString();
        public int Quantity => quantity;
    }
}