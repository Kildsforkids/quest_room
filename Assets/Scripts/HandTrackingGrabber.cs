using UnityEngine;

public class HandTrackingGrabber : OVRGrabber
{
    [SerializeField]
    private Transform controller;
    [SerializeField]
    private GameObject fireBall;
    [SerializeField]
    private float pinchTreshold = 0.7f;
    [SerializeField]
    private float pullingSpeed = 1f;
    [SerializeField]
    private bool isInversed = false;

    [SerializeField]
    private Transform handCenter;

    private OVRHand hand;

    private Transform movingObject;
    private GameObject currentFireBall;

    private LineRenderer lineRenderer = null;

    protected override void Awake()
    {
        base.Awake();
        lineRenderer = GetComponent<LineRenderer>();
    }

    protected override void Start()
    {
        base.Start();
        hand = GetComponent<OVRHand>();
    }

    public override void Update()
    {
        base.Update();
        CheckIndexPinch();

        //if (hand.IsTracked)
        //{
        //    controller.gameObject.SetActive(false);
        //}
        //else
        //{
        //    controller.gameObject.SetActive(true);
        //}
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

            if (movingObject == null)
            {
                lineRenderer.enabled = true;

                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, endPos);
            }
            
            var target = hit.transform;

            if (target.tag == "Interactable")
            {
                if (movingObject == null)
                {
                    movingObject = target;
                    movingObject.GetComponent<Rigidbody>().isKinematic = true;
                    lineRenderer.enabled = false;
                }
                    
                //movingObject.position = Vector3.MoveTowards(target.position, transform.position, 1f * Time.deltaTime);

                //if (Vector3.Distance(movingObject.position, transform.position) < 0.1f)
                //{

                //}
            }
            //else
            //{
            //    if (movingObject != null)
            //    {
            //        movingObject.GetComponent<Rigidbody>().isKinematic = false;
            //        movingObject = null;
            //    }
            //}

            //if (movingObject && hit.transform.tag == "Interactable")
            //{
            //    movingObject = hit.transform;
            //}
        }
        else
        {
            lineRenderer.enabled = false;
        }

        if (movingObject != null)
        {
            movingObject.position = Vector3.MoveTowards(movingObject.position, handCenter.position, pullingSpeed * Time.deltaTime);
        }
        // 2 is the duration the line is drawn, afterwards its deleted
        //Debug.DrawLine(startPos, endPos, Color.green, 2);
        return hit;
    }

    //private void CheckHandPinch()
    //{
    //    float pinchStrengthIndex = hand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
    //    float pinchStrengthMiddle = hand.GetFingerPinchStrength(OVRHand.HandFinger.Middle);
    //    //float pinchStrengthThumb = hand.GetFingerPinchStrength(OVRHand.HandFinger.Thumb);
    //    bool isPinching = pinchStrengthIndex > 0.2f && pinchStrengthMiddle > 0.2f;// && pinchStrengthThumb > 0.2f;

    //    if (!m_grabbedObj && isPinching && m_grabCandidates.Count > 0)
    //        GrabBegin();
    //    else if (m_grabbedObj && !isPinching)
    //        GrabEnd();
    //}

    private void CheckIndexPinch()
    {
        float pinchStrength = hand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
        float pinkyPinch = hand.GetFingerPinchStrength(OVRHand.HandFinger.Pinky);
        bool isPinkyPinching = pinkyPinch > pinchTreshold;
        bool isPinching = pinchStrength > pinchTreshold;

        //if (isPinkyPinching && currentFireBall == null)
        //{
        //    currentFireBall = Instantiate(fireBall, transform);
        //    currentFireBall.transform.position = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.05f, transform.localPosition.z);
        //}
        //else if (!isPinkyPinching && currentFireBall != null)
        //{
        //    currentFireBall.transform.SetParent(null);
        //    currentFireBall.GetComponent<Rigidbody>().AddForce(currentFireBall.transform.forward * 100f);
        //    currentFireBall = null;
        //}

        if (!m_grabbedObj && isPinching)
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
        else if (!isPinching)
        {
            lineRenderer.enabled = false;
            if (movingObject != null)
            {
                movingObject.GetComponent<Rigidbody>().isKinematic = false;
                movingObject = null;
            }
        }

        if (!m_grabbedObj && isPinching && m_grabCandidates.Count > 0)
        {
            GrabBegin();
            lineRenderer.enabled = false;
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
