using UnityEngine;

public class HandTouch : MonoBehaviour
{
    [SerializeField]
    private OVRSkeleton handSkeleton;

    [SerializeField]
    private Messenger messenger;

    private void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            var colName = contact.thisCollider.name;
            messenger.ShowMessage(colName);
        }
    }
}
