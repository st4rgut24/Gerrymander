using UnityEngine;
using TMPro;
using UnityEngine.UI; // For Button
using UnityEngine.SceneManagement;

public class GlobalOutlineAdder : Singleton<GlobalOutlineAdder>
{
    public Color outlineColor = Color.black; // Color of the outline
    public float outlineWidth = 0.2f; // Width of the outline

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;

        DontDestroyOnLoad(this.gameObject);
    }

    void OnEnable()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe from the sceneLoaded event to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find all Button components in the loaded scene
        Button[] allButtons = FindObjectsOfType<Button>(true);

        // Iterate through each Button component
        foreach (var button in allButtons)
        {
            bool isActive = button.gameObject.activeSelf;
            button.gameObject.SetActive(true);
            // Find TextMeshProUGUI components in the button's children
            TextMeshProUGUI[] textMeshProUGUIs = button.GetComponentsInChildren<TextMeshProUGUI>(true);

            foreach (var textMeshProUGUI in textMeshProUGUIs)
            {
                // Set outline properties
                textMeshProUGUI.outlineColor = outlineColor;
                textMeshProUGUI.outlineWidth = outlineWidth;
            }
            button.gameObject.SetActive(isActive);
        }
    }
}
