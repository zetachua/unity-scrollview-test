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
    private float ItemHeight=> OptimizeScroll.ItemHeight;
    private float ItemSpacing=> OptimizeScroll.ItemSpacing;

    // Initalizations
    private float _rowHeight =0;
    private float _rowWidth =0;
    private float _scrollRectSize;

    private void Start()
    {
        _rowHeight = inventoryRow.GetRowHeight();
        _rowWidth=inventoryRow.GetRowWidth();
        _scrollRectSize = scrollRect.GetComponent<RectTransform>().rect.height;
        VisibleRows = Mathf.FloorToInt(_scrollRectSize / (_rowHeight + ItemSpacing))+ BufferRows;
        Debug.Log(_scrollRectSize+ " "+_rowHeight);
        SetContentHeight();
        ResizeScrollViewToFitRow();
        inventoryRow.SetRowHeight(ItemHeight-ItemSpacing); 
        PopulateInventoryDisplay();

        Debug.Log($"Row width (used for Scroll View sizing): {_rowWidth} and VisibleRows: {VisibleRows}");
    }

    // Instantiate smaller num of rows cuz of row reuse to save memory
    private void PopulateInventoryDisplay()
    {        
        contentHolder.GetComponent<CanvasGroup>().alpha = 0f;  // Set opacity to 0 (fully transparent)

        // Calculate which row index is currently at the top of the scroll view
        int startIndex = Mathf.FloorToInt(contentHolder.anchoredPosition.y / (ItemHeight + ItemSpacing));

        for (int i = 0; i < VisibleRows; i++)
        {
            int rowIndex = startIndex + i;
            var row = Instantiate(inventoryRow, contentHolder);
            row.Init(rowIndex);
            row.name = $"Row {rowIndex}";
            rowPool.Add(row);
        }
    }

    // Changed based on number of items it will update the contentheight
    private void SetContentHeight()
    {
        // var contentHeight = verticalLayoutGroup.spacing * NumRows + 10f;
        float contentHeight = (ItemHeight + ItemSpacing) * NumRows; // 10f is spacing
        contentHolder.sizeDelta = new Vector2(0f, contentHeight);
        StartCoroutine(DisableLayoutGroupNextFrame());
    }
    private IEnumerator DisableLayoutGroupNextFrame()
    {
        yield return new WaitForEndOfFrame();
        verticalLayoutGroup.enabled = false;
        RepositionRows();
        contentHolder.GetComponent<CanvasGroup>().alpha = 1f;  // Set opacity to 0 (fully transparent)
    }

    private void RepositionRows()
    {
        for (int i = 0; i < rowPool.Count; i++)
        {
            var row = rowPool[i];
            OptimizeScroll.RepositionRow(row, i);
            Debug.Log($"Reposition Row {i} to posY: {row.GetComponent<RectTransform>().anchoredPosition.y}");
        }
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
