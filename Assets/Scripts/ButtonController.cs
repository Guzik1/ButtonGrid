using Assets.Scripts.PathFinding;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public int X;
    public int Y;

    public ButtonEvent ButtonEvent;

    public PathNode<ButtonController> CurrentNode;

    public bool Walkable;

    Text buttonText;
    Image image;
    RectTransform rectTransform;
    Button button;

    void Awake()
    {
        buttonText = GetComponentInChildren<Text>();
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();
    }
     
    public void OnButtonClick()
    {
        if (!Walkable)
            return;

        ButtonEvent?.OnButtonClick(this);
    }

    public void SetPosition(float x, float y)
    {
        rectTransform.anchoredPosition = new Vector2(x, y);
    }

    public void SetScale(float width, float height)
    {
        rectTransform.sizeDelta = new Vector2(width, height);
    }

    public void SetCordsInText()
    {
        buttonText.text = $"{ X }/{ Y }";
    }

    public void SetCordsWithDistanceInText(int distance)
    {
        buttonText.text = $"{ X }/{ Y } - { distance }";
    }

    public void SetText(string text)
    {
        buttonText.text = text;
    }

    public void SetActive(bool active)
    {
        this.Walkable = active;
        button.interactable = active;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void SetColor(Color color)
    {
        image.color = color;
    }

    public void ClearText()
    {
        buttonText.text = "";
    }
}