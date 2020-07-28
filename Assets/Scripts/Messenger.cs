using UnityEngine;
using UnityEngine.UI;

public class Messenger : MonoBehaviour
{
    [SerializeField]
    private Text message;
    [SerializeField]
    private Color defaultColor;

    //private void Start()
    //{
    //    message.text = "";
    //}

    public void ShowMessage(string text)
    {
        message.text = text;
        SetMessageColor(defaultColor);
    }

    public void SetMessageColor(Color color)
    {
        message.color = color;
    }

    public Color GetDefaultColor()
    {
        return defaultColor;
    }
}
