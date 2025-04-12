using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManagerInstantiateAll : MonoBehaviour
{
    [SerializeField] private RectTransform contentHolder;
    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField] private InventoryRow inventoryRow;
    [SerializeField] private ScrollRect scrollRect; 
    [SerializeField] private int numRows = 10;
    public GameObject[] inventoryRows;
    private float rowWidth;
    private float rowHeight;
    private int firstVisibleRowIndex = 0; // Track first visible row
    private int lastVisibleRowIndex = 1;  // Track last visible row

    private float lastScrollPos = 0f; // Store the last scroll position for comparison
    private int visibleRows=0;
    private Vector2 viewportSize;

    private void Start()
    {
        inventoryRows = new GameObject[numRows];
        PopulateInventoryDisplay();
        scrollRect.onValueChanged.AddListener(TrackScrollPosition);

        rowHeight = inventoryRow.GetRowHeight();
        rowWidth=inventoryRow.GetRowWidth();
        viewportSize = scrollRect.viewport.rect.size;
        visibleRows = Mathf.FloorToInt(viewportSize.y / (rowHeight))+4;
        ResizeScrollViewToFitRow();
        Debug.Log($"Row width (used for Scroll View sizing): {rowWidth}");
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

        lastScrollPos = normalizedPos; 
        UpdateRowVisibility();
    }

    private void UpdateRowVisibility()
    {
        // Ensure we don't go beyond available rows
        visibleRows = Mathf.Min(visibleRows, numRows);

        // Get the current content position (top and bottom of the scroll area)
        float contentTop = contentHolder.anchoredPosition.y;
        float contentBottom = contentTop - viewportSize.y;

        // Calculate the first and last row index that should be visible based on the scroll position
        int firstVisibleRow = Mathf.FloorToInt(Mathf.Abs(contentTop) / (rowHeight));
        int lastVisibleRow = firstVisibleRow + visibleRows - 1;

        firstVisibleRow = Mathf.Max(0, firstVisibleRow);
        lastVisibleRow = Mathf.Min(numRows - 1, lastVisibleRow);

        // 🔹 Disable rows ABOVE the visible range
        for (int i = 0; i < firstVisibleRow; i++)
        {
            if (i >= 0 && i < inventoryRows.Length && inventoryRows[i] != null)
                inventoryRows[i].SetActive(false);
        }

        // 🔹 Disable rows BELOW the visible range
        for (int i = lastVisibleRow + 1; i < numRows; i++)
        {
            if (i >= 0 && i < inventoryRows.Length && inventoryRows[i] != null)
                inventoryRows[i].SetActive(false);
        }

        // ✅ Enable rows WITHIN the visible range
        for (int i = firstVisibleRow; i <= lastVisibleRow; i++)
        {
            if (i >= 0 && i < inventoryRows.Length && inventoryRows[i] != null)
            {
                inventoryRows[i].SetActive(true);
                Debug.Log($"Row {i} is visible.");
            }
            else
            {
                Debug.LogWarning($"Trying to access inventoryRows[{i}], but it's out of bounds or null.");
            }
        }
        // Debug the calculated indices
        Debug.Log($"First visible row: {firstVisibleRow}, Last visible row: {lastVisibleRow}");
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


    // Calculate the right width for the scrollview
    private void ResizeScrollViewToFitRow()
    {
        int numItems = inventoryRow != null ? inventoryRow.GetNumItems() : 4; 
        float multiplier = 0.19f - (Mathf.Clamp(numItems, 4, 6) - 4) * 0.005f;
        float width = rowWidth + rowWidth * multiplier;

        // Assuming contentHolder is inside Viewport → ScrollView
        RectTransform scrollViewRect = contentHolder.parent?.parent?.GetComponent<RectTransform>();
        if (scrollViewRect != null)
        {
            scrollViewRect.sizeDelta = new Vector2(width, scrollViewRect.sizeDelta.y);
            Debug.Log($"ScrollView resized to width: {width}");
        }
    }
}
