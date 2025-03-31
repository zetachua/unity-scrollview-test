using TMPro;
using UnityEngine;

public class InventoryItemDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text textDisplay;
    public void Init(int index)
    {
        textDisplay.text = index.ToString();
    }
}
