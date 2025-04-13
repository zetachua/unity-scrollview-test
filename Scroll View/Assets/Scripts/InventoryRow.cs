using UnityEngine;
using UnityEngine.UI;

public class InventoryRow : MonoBehaviour
{
    [SerializeField] private InventoryItemDisplay[] rowItems;
    
    public void Init(int rowIndex)
    {
        var startIndex = rowIndex * (rowItems.Length);
        for (var i = 0; i < rowItems.Length; i++)
        {
            rowItems[i].Init( startIndex + i + 1);
            rowItems[i].name = $"item {startIndex + i + 1}";
        }
    }
    public float GetRowWidth()
    {
        if (rowItems.Length == 0) return 0f;
        return (rowItems[0]?.GetComponent<RectTransform>()?.rect.width ?? 0f) * GetNumItems();
    }

    public float GetRowHeight()
    {
        if (rowItems.Length == 0) return 0f;
        return rowItems[0]?.GetComponent<RectTransform>()?.rect.height ?? 0f;
    }
    public void SetRowHeight(float rowHeight)
    {
        if (rowItems.Length == 0) return;
        RectTransform rtOriginal = rowItems[0]?.GetComponent<RectTransform>();
        float originalWidth=rtOriginal.rect.width;

        for (var i = 0; i < rowItems.Length; i++)
        {
            RectTransform rt = rowItems[i]?.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(originalWidth, rowHeight);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public int GetNumItems()
    {
        return rowItems.Length;
    }
}
