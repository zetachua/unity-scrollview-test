using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OptimizeScroll : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform viewPort;
    [SerializeField] private InventoryManager inventoryManager;

    // Constants
    public const float ItemHeight = 110f; // includes spacing of 10
    public const float ItemSpacing = 10f;

    // Shortcuts / Accessors
    private int NumRows => inventoryManager.NumRows;
    private int VisibleRows => inventoryManager.VisibleRows;
    private ScrollRect scrollRect => inventoryManager.scrollRect;
    private RectTransform contentHolder => inventoryManager.contentHolder;
    private List<InventoryRow> rowPool => inventoryManager.rowPool;

    // Initalizations
    private int _firstVisibleRowIndex = 0; // Track first visible row

    private void OnEnable()
    {
        scrollRect.onValueChanged.AddListener(HandleScroll);
    }
    
    private void HandleScroll(Vector2 value)
    {
        StopAllCoroutines();
        // Delay Scroll Handling Until Layout is Applied 
        StartCoroutine(DelayedUpdateVisibleItems());
    }

    private IEnumerator DelayedUpdateVisibleItems()
    {
        yield return null; // wait for layout to update
        UpdateVisibleItems();
    }

    private void OnDestroy()
    {
        // Clean up listener
        if (scrollRect != null)
        {
            scrollRect.onValueChanged.RemoveAllListeners(); 
        }
    }

    private void UpdateVisibleItems()
    {
        // Implement your solution here
        // Access the array of inventory rows as needed: inventoryManager.inventoryRows
          
        int newFirstVisibleRowIndex = GetNewFirstVisibleRowIndex();
        if (newFirstVisibleRowIndex == _firstVisibleRowIndex) return;   //return early if no change
        int direction = newFirstVisibleRowIndex > _firstVisibleRowIndex ? 1 : -1;    

        while (_firstVisibleRowIndex != newFirstVisibleRowIndex)
        {
            if (direction > 0)
            {
                if (!ScrollDown()) break;
            }
            else
            {
                if (!ScrollUp()) break;
            }
        }
    }

    private int GetNewFirstVisibleRowIndex()
    {
        float scrollYPos = contentHolder.anchoredPosition.y;        // scrollYPos: how much posY has been scrolled 
        int newIndex = Mathf.FloorToInt(scrollYPos / ItemHeight);   // eg 330/110 =3
        return Mathf.Clamp(newIndex, 0, NumRows - VisibleRows);
    }

    private bool ScrollDown()
    {
        if (rowPool.Count == 0) return false;

        InventoryRow topRow = rowPool[0];                     // Recycle the top row
        rowPool.RemoveAt(0);

        int newIndex = _firstVisibleRowIndex + VisibleRows;    // Find the new row to display
        if (newIndex >= NumRows) return false;

        topRow.Init(newIndex);                                // Initialize with new index
        topRow.transform.SetAsLastSibling();                  // Move it to bottom of UI hierarchy
        RepositionRow(topRow, newIndex);                      // Update posY eg => -(6 * 110) -10f =-670

        rowPool.Add(topRow);
        _firstVisibleRowIndex++;
        return true;
    }

    private bool ScrollUp()
    {
        if (rowPool.Count == 0) return false;

        InventoryRow bottomRow = rowPool[rowPool.Count - 1];
        rowPool.RemoveAt(rowPool.Count - 1);

        int newIndex = _firstVisibleRowIndex - 1;
        if (newIndex < 0) return false;

        bottomRow.Init(newIndex);
        bottomRow.transform.SetAsFirstSibling();
        RepositionRow(bottomRow, newIndex);

        rowPool.Insert(0, bottomRow);
        _firstVisibleRowIndex--;
        return true;
    }

    public static void RepositionRow(InventoryRow row, int index)
    {
        RectTransform rt = row.GetComponent<RectTransform>();
        float originalX = rt.anchoredPosition.x;
        float posY = - index * (ItemHeight + ItemSpacing);         // Adjust posY of item 
        rt.anchoredPosition = new Vector2(originalX, posY);        // eg - 6*(110+10) = -670 
    }
}
