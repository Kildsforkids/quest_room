using UnityEngine;

public class HingeJointTarget : MonoBehaviour
{
    public enum CoordAxis { X, Y, Z }

    [SerializeField]
    private HingeJoint hj;
    [SerializeField]
    private Transform target;

    [SerializeField]
    private CoordAxis axis = CoordAxis.X;
    [SerializeField]
    private bool invert;

    private void Update()
    {
        if (hj != null)
        {
            DoSomeStuff();
        }
    }

    private void DoSomeStuff()
    {
        var js = hj.spring;

        js.targetPosition = axis == CoordAxis.X ? target.localEulerAngles.x : axis == CoordAxis.Y ? target.localEulerAngles.y : target.localEulerAngles.z;
        if (js.targetPosition > 180)
            js.targetPosition = js.targetPosition - 360;
        if (invert)
            js.targetPosition = js.targetPosition * -1;

        js.targetPosition = Mathf.Clamp(js.targetPosition, hj.limits.min + 5, hj.limits.max - 5);

        hj.spring = js;
    }
}
