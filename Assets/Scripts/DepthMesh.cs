using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZView
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class DepthMesh : MonoBehaviour
    {
        MeshData meshData;

        public MeshData Data
        {
            get => meshData;
            set => SetData(value);
        }

        public void SetData(MeshData data)
        {
            var mesh = new Mesh();
            gameObject.name = data.Timestamp.ToString();
            mesh.SetVertices(data.vertices);
            mesh.SetIndices(Enumerable.Range(0, data.vertices.Count).Select(x => x).ToArray(),
                MeshTopology.Points,
                0);

            var mf = GetComponent<MeshFilter>();
            mf.sharedMesh = mesh;
        }
    }
}