using UnityEngine;

public class RagdollHandSnap : MonoBehaviour
{
    [SerializeField]
    private Transform hand;

    private void LateUpdate()
    {
        transform.localPosition = hand.localPosition;
        transform.localRotation = hand.localRotation;
        transform.Rotate(Vector3.up, 180f, Space.Self);
    }
}
