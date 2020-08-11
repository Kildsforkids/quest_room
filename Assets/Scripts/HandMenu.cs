using UnityEngine;

public class HandMenu : MonoBehaviour
{
    [SerializeField]
    private Transform head;

    private void LateUpdate()
    {
        transform.LookAt(head, Vector3.up);
        transform.Rotate(Vector3.up, 180f, Space.Self);
    }
}
