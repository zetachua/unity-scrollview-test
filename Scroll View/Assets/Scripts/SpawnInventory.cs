using UnityEngine;

public class SpawnInventory : MonoBehaviour
{
    [SerializeField] private Transform contentHolder;
    [SerializeField] private InventoryItemDisplay displayItem;
    [SerializeField] private int inventorySize;
    
    private void Start()
    {
        PopulateInventoryDisplay();
    }

    private void PopulateInventoryDisplay()
    {
        for (var i = 0; i < inventorySize; i++)
        {
            var item = Instantiate(displayItem, contentHolder);
            item.Init(i + 1);
            item.name = $"item {i + 1}";
        }
    }
}
