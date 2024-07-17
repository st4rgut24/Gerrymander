using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailsPlayBtn : MonoBehaviour
{
    Button button;
    void Start()
    {
        // Find the Button component on the same GameObject
        button = GetComponent<Button>();

        button.interactable = false;

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
        ElectionDetailsManager.Instance.InitGame();
    }
}
