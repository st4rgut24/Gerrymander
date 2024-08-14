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
        int idx = GameManager.Instance.GetIndexFromElectionYear();
        scrollRect.verticalNormalizedPosition = indexToNormalizedPosition(idx);
        Debug.Log("setting normalized position to " + indexToNormalizedPosition(idx));
        //SetSelection(idx);
    }

    void Update()
    {
        // Calculate the index of the currently selected item based on scroll position
        float normalizedPosition = 1 - scrollRect.verticalNormalizedPosition;
        int selectedIndex = Mathf.Clamp((int)Mathf.Round(normalizedPosition * itemCount), 0, itemCount - 1);

        // Set the selected item based on the scroll position
        SetSelection(selectedIndex);
    }

    float indexToNormalizedPosition(int index)
    {
        return (float)(index + 1) / (float)items.Length;
    }

    public void SetSelection(int index)
    {
        selectedIdx = index;
        // Deselect all items
        foreach (var item in items)
        {
            item.GetComponent<Image>().color = Color.white; // Change to your deselected color
        }

        // Select the item at the given index
        Color yellowWithAlpha = new Color(243/255f, 239/255f, 171/255f);
        items[index].GetComponent<Image>().color = yellowWithAlpha; // Change to your selected color or apply other selection effect
    }
}
