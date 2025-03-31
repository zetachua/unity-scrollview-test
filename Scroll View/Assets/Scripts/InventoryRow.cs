using UnityEngine;

public class InventoryRow : MonoBehaviour
{
    [SerializeField] private InventoryItemDisplay[] rowItems;
    
    public void Init(int rowIndex)
    {
        var startIndex = rowIndex * rowItems.Length;
        for (var i = 0; i < rowItems.Length; i++)
        {
            rowItems[i].Init( startIndex+ i + 1);
            rowItems[i].name = $"item {startIndex + i + 1}";
        }
    }
}
