using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct Gesture
{
    public string name;
    public List<Vector3> fingerData;
    public UnityEvent onRecognized;
}

public class GestureDetector : MonoBehaviour
{
    [SerializeField]
    private float threshold = 0.1f;
    [SerializeField]
    private float recognitionRate = 2f;
    [SerializeField]
    private OVRSkeleton skeleton;
    [SerializeField]
    private bool debugMode = true;
    public List<Gesture> gestures;
    private List<OVRBone> fingerBones;
    private Gesture previousGesture;

    private bool isBonesSet = false;
    private float nextTimeToRecognize = 0f;

    private void Start()
    {
        previousGesture = new Gesture();
    }

    private void Update()
    {
        if (skeleton.gameObject.activeSelf)
        {
            if (!isBonesSet && skeleton.Bones.Count > 0)
            {
                fingerBones = new List<OVRBone>(skeleton.Bones);
                isBonesSet = true;
            }

            if (isBonesSet)
            {
                if (debugMode && Input.GetKeyDown(KeyCode.Space))
                {
                    Save();
                }

                Gesture currentGesture = Recognize();
                bool hasRecognized = !currentGesture.Equals(new Gesture());

                if (hasRecognized && Time.time >= nextTimeToRecognize)// && !currentGesture.Equals(previousGesture))
                {
                    nextTimeToRecognize = Time.time + recognitionRate;
                    Debug.Log($"New Gesture Found : {currentGesture.name}");
                    previousGesture = currentGesture;
                    currentGesture.onRecognized.Invoke();
                }
            }
        }
    }

    // Сохранить текущий жест на руке
    private void Save()
    {
        Gesture g = new Gesture();
        g.name = "New Gesture";
        List<Vector3> data = new List<Vector3>();
        foreach (var bone in fingerBones)
        {
            // Позиция пальца относительно корня
            data.Add(skeleton.transform.InverseTransformPoint(bone.Transform.position));
        }

        g.fingerData = data;
        gestures.Add(g);
    }

    // Распознать текущий жест
    private Gesture Recognize()
    {
        Gesture currentGesture = new Gesture();
        float currentMin = Mathf.Infinity;

        foreach (var gesture in gestures)
        {
            float sumDistance = 0f;
            bool isDiscarded = false;
            for (int i = 0; i < fingerBones.Count; i++)
            {
                Vector3 currentData = skeleton.transform.InverseTransformPoint(fingerBones[i].Transform.position);
                float distance = Vector3.Distance(currentData, gesture.fingerData[i]);
                if (distance > threshold)
                {
                    isDiscarded = true;
                    break;
                }

                sumDistance += distance;
            }

            if (!isDiscarded && sumDistance < currentMin)
            {
                currentMin = sumDistance;
                currentGesture = gesture;
            }
        }

        return currentGesture;
    }
}
