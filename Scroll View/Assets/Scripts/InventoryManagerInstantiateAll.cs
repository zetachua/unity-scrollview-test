using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManagerInstantiateAll : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform contentHolder;
    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField] private InventoryRow inventoryRow;
    [SerializeField] private ScrollRect scrollRect; 
    public GameObject[] inventoryRows;

    [Header("Configurations")]
    [SerializeField] private int numRows = 10;

    // Internal state
    private float _scrollRectSize;
    private float _rowWidth;
    private float _rowHeight;
    private float _lastScrollPos = 0f; 
    private int _visibleRows=0;

    // Constants
    private const float RowSpacing=10f;
    private const int BufferRows = 1;

    private void Start()
    {
        inventoryRows = new GameObject[numRows];
        PopulateInventoryDisplay();
        scrollRect.onValueChanged.AddListener(TrackScrollPosition);

        _rowHeight = inventoryRow.GetRowHeight();
        _rowWidth=inventoryRow.GetRowWidth();
        _scrollRectSize = scrollRect.GetComponent<RectTransform>().rect.height;

        _visibleRows = Mathf.FloorToInt(_scrollRectSize/ (_rowHeight+RowSpacing))+BufferRows;

        ResizeScrollViewToFitRow();
        Debug.Log($"Row width (used for Scroll View sizing): {_rowWidth}");
    }

    private void OnDestroy()
    {
        // Clean up listener
        if (scrollRect != null)
        {
            scrollRect.onValueChanged.RemoveListener(TrackScrollPosition);
        }
    }

    private void TrackScrollPosition(Vector2 scrollPos)
    {
        // Get normalized scroll position (0 = top, 1 = bottom)
        float normalizedPos = scrollRect.verticalNormalizedPosition;
        // Calculate scroll percentage
        float scrollPercentage = (1f - normalizedPos) * 100f;

        _lastScrollPos = normalizedPos; 
        UpdateRowVisibility();
    }

    private void UpdateRowVisibility()
    {
        // Ensure we don't go beyond available rows
        _visibleRows = Mathf.Min(_visibleRows, numRows);

        // Get the current content position (top and bottom of the scroll area)
        float contentTop = contentHolder.anchoredPosition.y;
        float contentBottom = contentTop - _scrollRectSize;

        // Calculate the first and last row index that should be visible based on the scroll position
        int firstVisibleRow = Mathf.FloorToInt(Mathf.Abs(contentTop) / (_rowHeight));
        int lastVisibleRow = firstVisibleRow + _visibleRows - 1;

        firstVisibleRow = Mathf.Max(0, firstVisibleRow);
        lastVisibleRow = Mathf.Min(numRows - 1, lastVisibleRow);

        // Disable rows ABOVE the visible range
        for (int i = 0; i < firstVisibleRow; i++)
        {
            if (i >= 0 && i < inventoryRows.Length && inventoryRows[i] != null)
                inventoryRows[i].SetActive(false);
        }

        // Disable rows BELOW the visible range
        for (int i = lastVisibleRow + 1; i < numRows; i++)
        {
            if (i >= 0 && i < inventoryRows.Length && inventoryRows[i] != null)
                inventoryRows[i].SetActive(false);
        }

        // Enable rows WITHIN the visible range
        for (int i = firstVisibleRow; i <= lastVisibleRow; i++)
        {
            if (i >= 0 && i < inventoryRows.Length && inventoryRows[i] != null)
                inventoryRows[i].SetActive(true);
        }
        // Debug.Log($"First visible row: {firstVisibleRow}, Last visible row: {lastVisibleRow}");
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
        var contentHeight = verticalLayoutGroup.spacing * numRows + RowSpacing;
        contentHolder.sizeDelta = new Vector2(0f, contentHeight);
        StartCoroutine(DisableLayoutGroupNextFrame());
    }

    private IEnumerator DisableLayoutGroupNextFrame()
    {
        yield return null;
        verticalLayoutGroup.enabled = false;
    }


    // Calculate the right width for the scrollview
    private void ResizeScrollViewToFitRow()
    {
        int numItems = inventoryRow != null ? inventoryRow.GetNumItems() : 4; 
        float multiplier = 0.19f - (Mathf.Clamp(numItems, 4, 6) - 4) * 0.005f;
        float width = _rowWidth + _rowWidth * multiplier;

        // Assuming contentHolder is inside Viewport → ScrollView
        RectTransform scrollViewRect = contentHolder.parent?.parent?.GetComponent<RectTransform>();
        if (scrollViewRect != null)
        {
            scrollViewRect.sizeDelta = new Vector2(width, scrollViewRect.sizeDelta.y);
            Debug.Log($"ScrollView resized to width: {width}");
        }
    }
}
