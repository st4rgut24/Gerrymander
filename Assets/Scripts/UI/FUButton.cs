using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public abstract class FUButton : MonoBehaviour
{
    Button button;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            // Add a listener to call the ClickHandler method when the button is clicked
            button.onClick.AddListener(DoSomething);
        }
        else
        {
            Debug.LogWarning("DynamicButtonClickHandler script attached to a GameObject without a Button component.");
        }
    }

    public abstract void DoSomething();
}
