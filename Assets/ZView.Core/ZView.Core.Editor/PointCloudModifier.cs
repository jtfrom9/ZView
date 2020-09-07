using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ZView.Editor
{
    [CustomEditor(typeof(PointCloudView))]
    public class PointCloudModifier : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("ModifyPose"))
            {
                var depthMesh = target as PointCloudView;
                if (depthMesh != null)
                {
                    depthMesh.ModifyPose();
                }
            }
        }
    }
}
