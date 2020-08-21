using UnityEngine;

public class SnapTransform : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private bool rotate = false;

    private void Update()
    {
        transform.position = target.position;
        transform.rotation = target.rotation;

        if (rotate)
            transform.Rotate(Vector3.up, 180f, Space.Self);
    }
}
