using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Doceragnum
{
    [CreateAssetMenu(fileName = "InventorySO", menuName = "Scriptable Objects/InventorySO")]
    public class InventorySO : ScriptableObject
    {
        public VideojuegoSO[] _videojuegos;
        
        public StoreItemSO[] _items;
        
        public BillSO[] _bills;
    }
}