using UnityEngine;
using UnityEngine.UI;

public class VerticalScrollSelector : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform content;
    public RectTransform[] items; // Reference to the UI elements representing selectable items

    public int selectedIdx;

    private float itemHeight;
    private int itemCount;

    void Start()
    {
        itemCount = items.Length;
        itemHeight = items[0].rect.height; // Assuming all items have the same height

        // Initialize scroll position to select the top item
        SetSelection(0);
    }

    void Update()
    {
        // Calculate the index of the currently selected item based on scroll position
        float normalizedPosition = 1 - scrollRect.verticalNormalizedPosition;
        Debug.Log("Normalized position " + normalizedPosition);
        int selectedIndex = Mathf.Clamp(Mathf.FloorToInt(normalizedPosition * (itemCount - 1)), 0, itemCount - 1);

        // Set the selected item based on the scroll position
        SetSelection(selectedIndex);
    }

    void SetSelection(int index)
    {
        selectedIdx = index;
        // Deselect all items
        foreach (var item in items)
        {
            item.GetComponent<Image>().color = Color.white; // Change to your deselected color
        }

        // Select the item at the given index
        items[index].GetComponent<Image>().color = Color.yellow; // Change to your selected color or apply other selection effect
    }
}
