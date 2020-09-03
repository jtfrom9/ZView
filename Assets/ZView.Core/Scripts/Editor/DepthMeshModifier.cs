using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ZView.Editor
{
    [CustomEditor(typeof(DepthMesh))]
    public class DepthMeshModifier : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("ModifyPose"))
            {
                var depthMesh = target as DepthMesh;
                if (depthMesh != null)
                {
                    depthMesh.ModifyPose();
                }
            }
        }
    }
}
