using UnityEngine;
using UnityEngine.UI;

public class Messenger : MonoBehaviour
{
    [SerializeField]
    private Text message;

    private void Start()
    {
        message.text = "";
    }

    public void ShowMessage(string text)
    {
        message.text = text;
    }
}
