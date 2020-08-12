using UnityEngine;

public class InitVR : MonoBehaviour
{
    private void Awake()
    {
        OVRManager.fixedFoveatedRenderingLevel = OVRManager.FixedFoveatedRenderingLevel.High;
        OVRManager.useDynamicFixedFoveatedRendering = true;
        Application.targetFrameRate = 72;
    }
}
