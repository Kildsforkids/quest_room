using UnityEngine;
using UnityEngine.UI;

public class HandButton : MonoBehaviour
{
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    private void OnTriggerEnter(Collider other)
    {
        image.color = Color.red;
    }

    private void OnTriggerExit(Collider other)
    {
        image.color = Color.white;
    }
}
