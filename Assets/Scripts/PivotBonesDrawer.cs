using System.Collections.Generic;
using UnityEngine;

public class PivotBonesDrawer : MonoBehaviour
{
    [SerializeField]
    private GameObject pivot;

    private OVRCustomSkeleton skeleton;

    private void Start()
    {
        skeleton = GetComponent<OVRCustomSkeleton>();

        var bones = skeleton.CustomBones;

        foreach (var bone in bones)
        {
            var instance = Instantiate(pivot, bone);
            //instance.GetComponent<HintStat>().SetHintText(bone.name);
        }
    }
}
