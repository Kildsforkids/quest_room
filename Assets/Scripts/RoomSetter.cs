using System;
using UnityEngine;

public class RoomSetter : MonoBehaviour
{
    public enum Mode { SetFloor, SetRotation, SetOffsetX, SetOffsetZ, TrackHeadPos, TurnHandsMode }

    [SerializeField]
    private OVRInput.Controller m_controller = OVRInput.Controller.None;
    [SerializeField]
    private Messenger messenger;
    [SerializeField]
    private GameObject pointPrefab;
    [SerializeField]
    private float pointLifeTime;

    [SerializeField]
    private GameObject leftController;
    [SerializeField]
    private GameObject rightController;
    [SerializeField]
    private GameObject leftHand;
    [SerializeField]
    private GameObject rightHand;

    [SerializeField]
    private Transform roomFloor;
    [SerializeField]
    private Transform head;
    [SerializeField]
    private Transform pointer;

    [SerializeField]
    private float movementSensitivity;
    [SerializeField]
    private float rotationSensitivity;

    [SerializeField]
    private float trackRate;

    [SerializeField]
    private float roomHalfX = -1.795f;
    [SerializeField]
    private float roomHalfZ = 2.883f;

    private Mode currentMode = Mode.SetFloor;
    private int modesCount;

    private Transform firstPoint;
    private Transform secondPoint;

    //private bool trackingIsStarted = false;

    //private float nextTimeToTrack = 0f;

    private void Start()
    {
        modesCount = Enum.GetValues(typeof(Mode)).Length;
        messenger.ShowMessage("Установка пола");
    }

    private void Update()
    {
        Tune();
        ChangeMode();
    }

    private void ChangeMode()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, m_controller))
        {
            int cm = (int)currentMode;
            cm++;
            if (cm == modesCount) cm = 0;
            currentMode = (Mode)cm;
            switch (currentMode)
            {
                case Mode.SetFloor:
                    messenger.ShowMessage("Установка пола");
                    break;
                case Mode.SetRotation:
                    messenger.ShowMessage("Поворот комнаты");
                    break;
                case Mode.SetOffsetX:
                    messenger.ShowMessage("Смещение комнаты по X");
                    break;
                case Mode.SetOffsetZ:
                    messenger.ShowMessage("Смещение комнаты по Z");
                    break;
                case Mode.TrackHeadPos:
                    messenger.ShowMessage("Отслеживание головы");
                    break;
                case Mode.TurnHandsMode:
                    messenger.ShowMessage("Включить руки");
                    break;
            }
        }
    }

    private void Tune()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, m_controller))
        {
            switch (currentMode)
            {
                case Mode.SetFloor:
                    roomFloor.position = new Vector3(roomFloor.position.x, pointer.position.y, roomFloor.position.z);
                    break;
                case Mode.SetRotation:
                    if (firstPoint == null)
                    {
                        firstPoint = Instantiate(pointPrefab, pointer.position, pointer.rotation).transform;
                        messenger.SetMessageColor(Color.yellow);
                    }
                    else
                    {
                        secondPoint = Instantiate(pointPrefab, pointer.position, Quaternion.identity).transform;

                        var heading = secondPoint.position - firstPoint.position;
                        var distance = heading.magnitude;
                        var direction = heading / distance;

                        roomFloor.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));

                        messenger.SetMessageColor(messenger.GetDefaultColor());
                        Destroy(firstPoint.gameObject, pointLifeTime);
                        Destroy(secondPoint.gameObject, pointLifeTime);
                    }
                    break;
                //case Mode.SetOffsetX:
                //    RaycastHit hit;
                //    if (Physics.Raycast(pointer.position, pointer.TransformDirection(Vector3.forward), out hit, 5f))
                //    {
                //        Debug.DrawRay(pointer.position, pointer.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                //    }
                //    roomFloor.localPosition = new Vector3(roomFloor.position.x, roomFloor.position.y, pointer.position.z + roomHalfX);
                //    break;
                //case Mode.SetOffsetZ:
                //    roomFloor.localPosition = new Vector3(pointer.position.x - roomHalfZ, roomFloor.position.y, roomFloor.position.z);
                //    break;
                case Mode.TrackHeadPos:
                    Instantiate(pointPrefab, head.position, Quaternion.identity);
                    //if (!trackingIsStarted)
                    //{
                    //    messenger.SetMessageColor(Color.yellow);
                    //    trackingIsStarted = true;
                    //}
                    //else
                    //{
                    //    messenger.SetMessageColor(messenger.GetDefaultColor());
                    //    trackingIsStarted = false;
                    //}
                    break;
                case Mode.TurnHandsMode:
                    leftHand.SetActive(true);
                    rightHand.SetActive(true);
                    leftController.SetActive(false);
                    rightController.SetActive(false);
                    messenger.gameObject.SetActive(false);
                    break;
            }
        }
        //if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, m_controller))
        //{
        //    switch (currentMode)
        //    {
        //        case Mode.SetOffsetX:
        //            int layerMask = 1 << 10;
        //            RaycastHit hit;
        //            if (Physics.Raycast(pointer.position, pointer.TransformDirection(Vector3.forward), out hit, 5f, layerMask))
        //            {
        //                Debug.DrawRay(pointer.position, pointer.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        //            }
        //            break;
        //    }
        //}
        if (OVRInput.GetDown(OVRInput.Button.One, m_controller))
        {
            messenger.SetMessageColor(Color.yellow);
        }
        if (OVRInput.Get(OVRInput.Button.One, m_controller))
        {
            switch (currentMode)
            {
                case Mode.SetOffsetX:
                    roomFloor.Translate(Vector3.right * Time.deltaTime * movementSensitivity, Space.Self);
                    break;
                case Mode.SetOffsetZ:
                    roomFloor.Translate(Vector3.forward * Time.deltaTime * movementSensitivity, Space.Self);
                    break;
            }
        }
        if (OVRInput.GetUp(OVRInput.Button.One, m_controller))
        {
            messenger.SetMessageColor(messenger.GetDefaultColor());
        }
        if (OVRInput.GetDown(OVRInput.Button.Two, m_controller))
        {
            messenger.SetMessageColor(Color.yellow);
        }
        if (OVRInput.Get(OVRInput.Button.Two, m_controller))
        {
            switch (currentMode)
            {
                case Mode.SetOffsetX:
                    roomFloor.Translate(Vector3.left * Time.deltaTime * movementSensitivity, Space.Self);
                    break;
                case Mode.SetOffsetZ:
                    roomFloor.Translate(Vector3.back * Time.deltaTime * movementSensitivity, Space.Self);
                    break;
            }
        }
        if (OVRInput.GetUp(OVRInput.Button.Two, m_controller))
        {
            messenger.SetMessageColor(messenger.GetDefaultColor());
        }
        //if (trackingIsStarted && Time.time >= nextTimeToTrack)
        //{
        //    nextTimeToTrack = Time.time + trackRate;
        //    Instantiate(pointPrefab, head.position, Quaternion.identity);
        //}
        //if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, m_controller).x != 0f && currentMode == Mode.SetOffsetX)
        //{
        //    roomFloor.Translate(Vector3.forward * OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, m_controller).x * Time.deltaTime * movementSensitivity);
        //}
    }
}
