using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OptimizeScroll : MonoBehaviour
{
    [SerializeField] private RectTransform viewPort;
    [SerializeField] private ScrollRect scrollRect;
    
    private const float ItemHeight = 110f; // includes spacing of 10
    
    [SerializeField] private InventoryManager inventoryManager;

    private void OnEnable()
    {
        scrollRect.onValueChanged.AddListener(HandleScroll);
    }

    private void HandleScroll(Vector2 value)
    {
        UpdateVisibleItems();
    }

    private void UpdateVisibleItems()
    {
        // Implement your solution here
        // Access the array of inventory rows as needed: inventoryManager.inventoryRows
        
        
    }
}
