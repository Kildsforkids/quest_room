using UnityEngine;

public class HandTrackingGrabber : OVRGrabber
{
    [SerializeField]
    private float pinchTreshold = 0.7f;
    [SerializeField]
    private bool isInversed = false;

    private OVRHand hand;

    private Transform movingObject;

    protected override void Start()
    {
        base.Start();
        hand = GetComponent<OVRHand>();
    }

    public override void Update()
    {
        base.Update();
        CheckIndexPinch();
        //CheckHandPinch();
    }

    private RaycastHit DetectHit(Vector3 startPos, float distance, Vector3 direction)
    {
        //init ray to save the start and direction values
        Ray ray = new Ray(startPos, direction);
        //varible to hold the detection info
        RaycastHit hit;
        //the end Pos which defaults to the startPos + distance 
        Vector3 endPos = startPos + (distance * direction);
        if (Physics.Raycast(ray, out hit, distance))
        {
            //if we detect something
            endPos = hit.point;

            //if (movingObject && hit.transform.tag == "Interactable")
            //{
            //    movingObject = hit.transform;
            //}
        }
        // 2 is the duration the line is drawn, afterwards its deleted
        Debug.DrawLine(startPos, endPos, Color.green, 2);
        return hit;
    }

    private void CheckHandPinch()
    {
        float pinchStrengthIndex = hand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
        float pinchStrengthMiddle = hand.GetFingerPinchStrength(OVRHand.HandFinger.Middle);
        //float pinchStrengthThumb = hand.GetFingerPinchStrength(OVRHand.HandFinger.Thumb);
        bool isPinching = pinchStrengthIndex > 0.2f && pinchStrengthMiddle > 0.2f;// && pinchStrengthThumb > 0.2f;

        if (!m_grabbedObj && isPinching && m_grabCandidates.Count > 0)
            GrabBegin();
        else if (m_grabbedObj && !isPinching)
            GrabEnd();
    }

    private void CheckIndexPinch()
    {
        float pinchStrength = hand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
        bool isPinching = pinchStrength > pinchTreshold;

        if (isPinching)
        {
            if (isInversed)
            {
                DetectHit(transform.position, 5f, -transform.up);
            }
            else
            {
                DetectHit(transform.position, 5f, transform.up);
            }
        }

        if (!m_grabbedObj && isPinching && m_grabCandidates.Count > 0)
        {
            GrabBegin();
        }
        else if (m_grabbedObj && !isPinching)
        {
            GrabEnd();
        }
    }

    protected override void GrabEnd()
    {
        if (m_grabbedObj)
        {
            Vector3 linearVelocity = (transform.position - m_lastPos) / Time.fixedDeltaTime;
            Vector3 angularVelocity = (transform.eulerAngles - m_lastRot.eulerAngles) / Time.fixedDeltaTime;

            GrabbableRelease(linearVelocity, angularVelocity);
        }

        GrabVolumeEnable(true);
    }
}
