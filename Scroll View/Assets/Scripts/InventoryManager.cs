using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private RectTransform contentHolder;
    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField] private InventoryRow inventoryRow;
    
    [SerializeField] private int numRows = 10;
    public GameObject[] inventoryRows;

    private void Start()
    {
        inventoryRows = new GameObject[numRows];
        PopulateInventoryDisplay();
    }

    private void PopulateInventoryDisplay()
    {
        for (var i = 0; i < numRows; i++)
        {
            var row = Instantiate(inventoryRow, contentHolder);
            row.Init(i);
            row.name = $"Row {i}";
            inventoryRows[i] = row.gameObject;
        }
        SetContentHeight();
    }

    private void SetContentHeight()
    {
        var contentHeight = verticalLayoutGroup.spacing * numRows + 10f;
        contentHolder.sizeDelta = new Vector2(0f, contentHeight);
        StartCoroutine(DisableLayoutGroupNextFrame());
    }

    private IEnumerator DisableLayoutGroupNextFrame()
    {
        yield return null;
        verticalLayoutGroup.enabled = false;
    }
}
