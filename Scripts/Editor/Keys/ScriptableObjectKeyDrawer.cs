using UnityEditor;
using UnityEngine;

namespace KodachiGames.Economy.Editor
{
    [CustomPropertyDrawer(typeof(WalletKey))]
    public class WalletKeyDrawer : ScriptableObjectKeyDrawer<WalletKey> { }

    [CustomPropertyDrawer(typeof(InventoryKey))]
    public class InventoryKeyDrawer : ScriptableObjectKeyDrawer<InventoryKey> { }
}
