using UnityEngine;

public class HandTrackingGrabber : OVRGrabber
{
    [SerializeField]
    private GameObject light;
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
    }

    // Бросает луч и запускает взаимодействие при попадании
    private RaycastHit DetectHit(Vector3 startPos, float distance, Vector3 direction)
    {
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        Ray ray = new Ray(startPos, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distance, layerMask))
        {
            var endPos = hit.point;

            if (movingObject == null)
            {
                lineRenderer.enabled = true;

                lineRenderer.SetPosition(0, handCenter.position - (handCenter.position - endPos).normalized * 0.1f);
                lineRenderer.SetPosition(1, endPos);

                light.transform.position = handCenter.position - (handCenter.position - endPos).normalized * 0.1f;
                light.SetActive(true);
            }
            
            var target = hit.transform;

            if (target.tag == "Interactable")
            {
                if (movingObject == null)
                {
                    movingObject = target;
                    movingObject.GetComponent<Rigidbody>().isKinematic = true;

                    light.SetActive(false);

                    lineRenderer.enabled = false;
                }
            }
        }
        else
        {
            lineRenderer.enabled = false;

            light.SetActive(false);
        }

        if (movingObject != null)
        {
            movingObject.position = Vector3.MoveTowards(movingObject.position, handCenter.position, pullingSpeed * Time.deltaTime);
        }
        //Debug.DrawLine(startPos, endPos, Color.green, 2);
        return hit;
    }

    // Проверка на пинч (щипок)
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
                DetectHit(handCenter.position, 5f, -handCenter.up);
            }
            else
            {
                DetectHit(handCenter.position, 5f, handCenter.up);
            }
        }
        else if (!isPinching)
        {
            lineRenderer.enabled = false;

            light.SetActive(false);
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

            light.SetActive(false);
        }
        else if (m_grabbedObj && !isPinching)
        {
            GrabEnd();
        }
    }

    // Переписываем GrabEnd, чтобы применять ускроение и поворот от руки при отпускании предмета
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
