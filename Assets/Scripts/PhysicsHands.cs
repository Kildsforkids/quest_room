using UnityEngine;

public class PhysicsHands : MonoBehaviour
{
    [SerializeField]
    private Transform handTarget;
    [SerializeField]
    private float forceAmount;

    public Vector3 Pos { get; set; }

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        var dir = handTarget.position - transform.position;
        Pos = dir.normalized * dir.magnitude * forceAmount;

        rb.AddForce(Pos);
    }
}
