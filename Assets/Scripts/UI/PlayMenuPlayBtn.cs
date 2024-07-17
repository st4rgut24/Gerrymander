using UnityEngine;
using UnityEngine.UI;
public class PlayMenuPlayBtn : MonoBehaviour
{
    void Start()
    {
        // Find the Button component on the same GameObject
        Button button = GetComponent<Button>();

        if (button != null)
        {
            // Add a listener to call the ClickHandler method when the button is clicked
            button.onClick.AddListener(ClickHandler);
        }
        else
        {
            Debug.LogWarning("DynamicButtonClickHandler script attached to a GameObject without a Button component.");
        }
    }

    void ClickHandler()
    {
        GameManager.Instance.LoadElectionDetailsScene();
    }
}
