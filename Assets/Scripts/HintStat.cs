using UnityEngine;
using UnityEngine.UI;

public class HintStat : MonoBehaviour
{
    [SerializeField]
    private Text text;

    public void SetHintText(string text)
    {
        this.text.text = text;
    }
}
