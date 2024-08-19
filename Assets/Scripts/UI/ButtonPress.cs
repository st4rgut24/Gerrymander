using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class ButtonPress : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private TextMeshProUGUI buttonText;

    [SerializeField]
    private string buttonString;

    [SerializeField]
    private Sprite unpressedSprite;

    [SerializeField]
    private Sprite pressedSprite;

    [SerializeField]
    private TextMeshProUGUI tmpro;

    private bool buttonPressed = false;

    private Vector2 PressedTextOffset = new Vector2(0, -6.5f);

    private RectTransform textRect;

    private void Start()
    {
        textRect = tmpro.GetComponent<RectTransform>();
        GetComponent<Image>().sprite = unpressedSprite;

        buttonText.text = buttonString;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!buttonPressed)
        {
            SoundManager.Instance.PlaySoundEffect(Consts.ButtonPress);
            GetComponent<Image>().sprite = pressedSprite;
            textRect.anchoredPosition = PressedTextOffset;

            buttonPressed = true;
        }
    }

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    buttonPressed = false;
    //    // You can add more logic here if needed when the button is released
    //}

    // Update is called once per frame
    void Update()
    {

    }
}