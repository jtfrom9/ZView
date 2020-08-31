using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Shapes;

namespace ZView
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class DepthMesh : MonoBehaviour
    {
        Material material;

        MeshData meshData;

        Vector3 position;
        Vector3 rotation;
        bool setup = false;

        LineInfo? lineInfo;

        void Start()
        {
            material = GetComponent<MeshRenderer>().material;
        }

        // public MeshData Data
        // {
        //     get => meshData;
        //     set => SetData(value);
        // }

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

            this.position = data.position;
            this.rotation = data.rotation;

            transform.position = data.position;
            transform.rotation = Quaternion.Euler(data.rotation);
            this.setup = true;

            lineInfo = new LineInfo
            {
                startPos = data.position,
                endPos = (data.position + Quaternion.Euler(data.rotation) * Vector3.forward).normalized,
                width = 0.5f,
                fillColor = Color.red,
                endArrow = true
            };
        }

        void Update()
        {
            if (lineInfo != null)
            {
                LineSegment.Draw(lineInfo.Value);
            }
        }

        public void ModifyPose()
        {
            if (this.setup)
            {
                this.transform.position = Vector3.zero;
                this.transform.rotation = Quaternion.identity;
                this.setup = false;
            } else {
                this.transform.position = this.position;
                this.transform.rotation = Quaternion.Euler(this.rotation);
                this.setup = true;
            }
        }

        public void OnFocus(bool focus)
        {
            Debug.Log($"{name} {focus}");
            if (focus)
            {
                this.material.SetColor("_Color", new Color(1, 1, 0, 1));
            }
            else
            {
                this.material.SetColor("_Color", Color.white);
            }
        }
    }
}