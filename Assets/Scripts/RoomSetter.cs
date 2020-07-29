using System;
using System.Collections;
using UnityEngine;

public class RoomSetter : MonoBehaviour
{
    public enum Mode { SetFloor, SetRotation, SetOffsetX, SetOffsetZ, TrackHeadPos }

    [SerializeField]
    private OVRInput.Controller m_controller = OVRInput.Controller.None;
    [SerializeField]
    private Messenger messenger;
    [SerializeField]
    private GameObject dotPrefab;

    [SerializeField]
    private Transform roomFloor;
    [SerializeField]
    private Transform head;

    [SerializeField]
    private float movementSensitivity;
    [SerializeField]
    private float rotationSensitivity;

    [SerializeField]
    private float trackRate;

    private Mode currentMode = Mode.SetFloor;
    private int modesCount;

    private Transform firstDot;
    private Transform secondDot;

    private bool trackingIsStarted = false;

    private float nextTimeToTrack = 0f;

    //private Coroutine trackingCoroutine;

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
                    roomFloor.position = new Vector3(roomFloor.position.x, transform.position.y, roomFloor.position.z);
                    break;
                case Mode.SetRotation:
                    if (firstDot == null)
                    {
                        firstDot = Instantiate(dotPrefab, transform.position, transform.rotation).transform;
                        messenger.SetMessageColor(Color.yellow);
                    }
                    else
                    {
                        secondDot = Instantiate(dotPrefab, transform.position, Quaternion.identity).transform;

                        var A = new Vector2(firstDot.position.x, firstDot.position.z);
                        var B = new Vector2(secondDot.position.x, secondDot.position.z);

                        //var C = new Vector2(firstDot.position.x, secondDot.position.z);
                        var C = new Vector2(secondDot.position.x, firstDot.position.z);

                        var a = C - A;
                        var b = B - A;

                        var multi = a.x * b.x + a.y * b.y;

                        var cos = multi / (a.magnitude * b.magnitude);

                        var acos = Mathf.Acos(cos);
                        var ang = (180.0f / Mathf.PI) * acos;

                        //RotateRoom(ang + 90.0f);
                        RotateRoom(ang);
                        messenger.SetMessageColor(messenger.GetDefaultColor());
                        Destroy(firstDot.gameObject, 3f);
                        Destroy(secondDot.gameObject, 3f);
                    }
                    break;
                case Mode.TrackHeadPos:
                    Instantiate(dotPrefab, head.position, Quaternion.identity);
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
            }
        }
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
        //    Instantiate(dotPrefab, head.position, Quaternion.identity);
        //}
        //if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, m_controller).x != 0f && currentMode == Mode.SetOffsetX)
        //{
        //    roomFloor.Translate(Vector3.forward * OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, m_controller).x * Time.deltaTime * movementSensitivity);
        //}
        //if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, m_controller).y != 0f)
        //{
        //    switch (currentMode)
        //    {
        //        case Mode.SetOffset:
        //            roomFloor.Translate(Vector3.right * OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, m_controller).y * Time.deltaTime * movementSensitivity);
        //            break;
        //        case Mode.SetRotation:
        //            roomFloor.Rotate(Vector3.up * OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, m_controller).y * Time.deltaTime * rotationSensitivity);
        //            break;
        //    }
        //}
    }

    //private IEnumerator SetHeadPosition()
    //{
    //    Instantiate(dotPrefab, head.position, Quaternion.identity);
    //    yield return new WaitForSeconds(2f);
    //}

    private void SetRoomRotationWithController()
    {
        firstDot = Instantiate(dotPrefab, transform.position, transform.rotation).transform;
        RotateRoom(firstDot.rotation.eulerAngles.y);
        Destroy(firstDot.gameObject, 3f);
    }

    private void SetRoomRotation()
    {
        if (firstDot == null)
        {
            firstDot = Instantiate(dotPrefab, transform.position, Quaternion.identity).transform;
        }
        else
        {
            secondDot = Instantiate(dotPrefab, transform.position, Quaternion.identity).transform;
            var heading = secondDot.position - firstDot.position;
            var distance = heading.magnitude;
            var direction = heading / distance;
            RotateRoom(direction.y * 360.0f);
            Destroy(firstDot.gameObject, 3f);
            Destroy(secondDot.gameObject, 3f);
        }
    }

    private void RotateRoom(float value)
    {
        roomFloor.rotation = Quaternion.Euler(roomFloor.rotation.eulerAngles.x, value, roomFloor.rotation.eulerAngles.z);
    }
}
