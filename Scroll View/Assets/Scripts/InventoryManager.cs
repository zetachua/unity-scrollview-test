using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{    
    [Header("UI References")]
    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
    public RectTransform contentHolder;
    public InventoryRow inventoryRow;
    public ScrollRect scrollRect; 
    public List<InventoryRow> rowPool;

    [Header("Config")]
    public int NumRows = 10;
    [System.NonSerialized] public int VisibleRows;

    // Constants
    private const int BufferRows = 4;

    // Initalizations
    private float _rowHeight =0;
    private float _rowWidth =0;
    private Vector2 _viewportSize;

    private void Start()
    {
        _rowHeight = inventoryRow.GetRowHeight();
        _rowWidth=inventoryRow.GetRowWidth();
        _viewportSize = scrollRect.viewport.rect.size;
        VisibleRows = Mathf.FloorToInt(_viewportSize.y / (_rowHeight))+BufferRows;
        
        PopulateInventoryDisplay();
        ResizeScrollViewToFitRow();

        Debug.Log($"Row width (used for Scroll View sizing): {_rowWidth} and VisibleRows: {VisibleRows}");
    }

    // Instantiate smaller num of rows cuz of row reuse to save memory
    private void PopulateInventoryDisplay()
    {
        // Changed NumRows to VisibleRows (smaller num of rows)
        for (var i = 0; i < VisibleRows; i++)
        {
            var row = Instantiate(inventoryRow, contentHolder);
            row.Init(i);
            row.name = $"Row {i}";
            rowPool.Add(row);
        }
        SetContentHeight();
    }

    private void SetContentHeight()
    {
        var contentHeight = verticalLayoutGroup.spacing * NumRows + 10f;
        contentHolder.sizeDelta = new Vector2(0f, contentHeight);
        StartCoroutine(DisableLayoutGroupNextFrame());
    }

    private IEnumerator DisableLayoutGroupNextFrame()
    {
        yield return null;
        verticalLayoutGroup.enabled = false;
    }

    // Dynamically reduce scroll view width as item count increases, with trial-based tuning
    private void ResizeScrollViewToFitRow()
    {
        int numItems = inventoryRow != null ? inventoryRow.GetNumItems() : BufferRows; 

        // from trial and error found a multiplier formula to calculate the width
        float multiplier = 0.19f - (Mathf.Clamp(numItems, BufferRows, 6) - BufferRows) * 0.005f;
        float width = _rowWidth + _rowWidth * multiplier;

        RectTransform scrollViewRect = contentHolder.parent?.parent?.GetComponent<RectTransform>();
        if (scrollViewRect != null)
        {
            scrollViewRect.sizeDelta = new Vector2(width, scrollViewRect.sizeDelta.y);
            Debug.Log($"ScrollView resized to width: {width}");
        }
    }
}
