using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManagerRecycle : MonoBehaviour
{
    [SerializeField] private RectTransform contentHolder;
    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField] private InventoryRow inventoryRow;
    [SerializeField] private ScrollRect scrollRect; 
    [SerializeField] private int numRows = 10;
    private int bufferRows = 4;
    private float rowSpacing=10f;
    private float rowInitialOffset = -10f;
    private List<InventoryRow> rowPool = new List<InventoryRow>();
    private float rowWidth;
    private float rowHeight;
    private float totalRowHeight;
    private int firstVisibleRowIndex = 0; // Track first visible row
    private int visibleRows;
    private Vector2 viewportSize;

    private void Start()
    {
        rowHeight = inventoryRow.GetRowHeight();
        totalRowHeight=rowHeight+rowSpacing;
        rowWidth=inventoryRow.GetRowWidth();
        viewportSize = scrollRect.viewport.rect.size;
        visibleRows = Mathf.FloorToInt(viewportSize.y / (rowHeight))+bufferRows;
        
        scrollRect.onValueChanged.AddListener((Vector2 _) => UpdateRowVisibility());
        PopulateInventoryDisplay();
        ResizeScrollViewToFitRow();

        Debug.Log($"Row width (used for Scroll View sizing): {rowWidth} and visibleRows: {visibleRows}");
    }

    private void OnDestroy()
    {
        // Clean up listener
        if (scrollRect != null)
        {
            scrollRect.onValueChanged.RemoveAllListeners(); 
        }
    }

    // Row reuse logic
    private void UpdateRowVisibility()
    {
        // Number of rows that are visible within scrollview frame
        float scrollYPos = contentHolder.anchoredPosition.y;
        int newFirstVisibleRowIndex = Mathf.FloorToInt(scrollYPos / totalRowHeight);

        // Clamp to range 
        newFirstVisibleRowIndex = Mathf.Clamp(newFirstVisibleRowIndex, 0, numRows - visibleRows);

        int direction = newFirstVisibleRowIndex > firstVisibleRowIndex ? 1 : -1;
        Debug.Log(newFirstVisibleRowIndex+ "newFirstVisibleRowIndex" +firstVisibleRowIndex+"firstVisibleRowIndex" );

        while (firstVisibleRowIndex != newFirstVisibleRowIndex)
        {
            if (direction > 0)
            {
                // Scroll down
                if (rowPool.Count == 0) break;

                InventoryRow topRow = rowPool[0];
                rowPool.RemoveAt(0);

                int newIndex = firstVisibleRowIndex + visibleRows;
                if (newIndex >= numRows) break;

                topRow.Init(newIndex);
                topRow.transform.SetAsLastSibling();

                RectTransform topRt=topRow.GetComponent<RectTransform>();
                float originalX = topRt.anchoredPosition.x;
                float posY = -newIndex * totalRowHeight +rowInitialOffset;
                topRt.anchoredPosition = new Vector2(originalX, posY);

                rowPool.Add(topRow);
                firstVisibleRowIndex++;
            }
            else
            {
                // Scroll up
                if (rowPool.Count == 0) break;

                InventoryRow bottomRow = rowPool[rowPool.Count - 1];
                rowPool.RemoveAt(rowPool.Count - 1);

                int newIndex = firstVisibleRowIndex - 1;
                if (newIndex < 0) break;

                bottomRow.Init(newIndex);
                bottomRow.transform.SetAsFirstSibling();

                RectTransform bottomRt=bottomRow.GetComponent<RectTransform>();
                float originalX = bottomRt.anchoredPosition.x;
                float posY = -newIndex * totalRowHeight +rowInitialOffset;
                bottomRt.anchoredPosition = new Vector2(originalX, posY);

                rowPool.Insert(0, bottomRow);
                firstVisibleRowIndex--;
            }
        }
    }

    // Instantiate smaller num of rows cuz of row reuse to save memory
    private void PopulateInventoryDisplay()
    {
        for (var i = 0; i < visibleRows; i++)
        {
            var row = Instantiate(inventoryRow, contentHolder);
            row.Init(i);
            row.name = $"Row {i}";
            rowPool.Add(row);
        }
        SetContentHeight();
        Debug.Log($"Created {visibleRows} pooled rows"+ rowPool);
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

    // Calculate a dynamic width for the scrollview
    private void ResizeScrollViewToFitRow()
    {
        int numItems = inventoryRow != null ? inventoryRow.GetNumItems() : bufferRows; 
        float multiplier = 0.19f - (Mathf.Clamp(numItems, bufferRows, 6) - bufferRows) * 0.005f;
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
