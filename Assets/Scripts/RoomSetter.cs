using UnityEngine;

public class RoomSetter : MonoBehaviour
{
    public enum Mode { SetFloor, SetRotation }

    [SerializeField]
    private OVRInput.Controller m_controller = OVRInput.Controller.None;
    [SerializeField]
    private Messenger messenger;
    [SerializeField]
    private GameObject dotPrefab;

    [SerializeField]
    private Transform roomFloor;

    private Mode currentMode = Mode.SetFloor;
    private int modesCount;

    private Transform firstDot;
    private Transform secondDot;

    private void Start()
    {
        modesCount = System.Enum.GetValues(typeof(Mode)).Length;
        messenger.ShowMessage("Установка пола");
    }

    private void Update()
    {
        PrintDot();
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
            }
        }
    }

    private void PrintDot()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, m_controller))
        {
            switch (currentMode)
            {
                case Mode.SetFloor:
                    roomFloor.position = new Vector3(roomFloor.position.x, transform.position.y, roomFloor.position.z);
                    break;
                case Mode.SetRotation:
                    firstDot = Instantiate(dotPrefab, transform.position, transform.rotation).transform;
                    RotateRoom(-firstDot.rotation.eulerAngles.y);
                    Destroy(firstDot.gameObject, 3f);
                    break;
            }
        }
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
