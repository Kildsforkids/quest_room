using UnityEngine;

public class HandTrackingGrabber : OVRGrabber
{
    [SerializeField]
    private float pinchTreshold = 0.7f;

    private OVRHand hand;

    protected override void Start()
    {
        base.Start();
        hand = GetComponent<OVRHand>();
    }

    public override void Update()
    {
        base.Update();
        //CheckIndexPinch();
        CheckHandPinch();
    }

    private void CheckHandPinch()
    {
        float pinchStrengthIndex = hand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
        float pinchStrengthMiddle = hand.GetFingerPinchStrength(OVRHand.HandFinger.Middle);
        float pinchStrengthThumb = hand.GetFingerPinchStrength(OVRHand.HandFinger.Thumb);
        bool isPinching = pinchStrengthIndex > 0.2f && pinchStrengthMiddle > 0.2f && pinchStrengthThumb > 0.2f;

        if (!m_grabbedObj && isPinching && m_grabCandidates.Count > 0)
            GrabBegin();
        else if (m_grabbedObj && !isPinching)
            GrabEnd();
    }

    private void CheckIndexPinch()
    {
        float pinchStrength = hand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
        bool isPinching = pinchStrength > pinchTreshold;

        if (!m_grabbedObj && isPinching && m_grabCandidates.Count > 0)
            GrabBegin();
        else if (m_grabbedObj && !isPinching)
            GrabEnd();
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
