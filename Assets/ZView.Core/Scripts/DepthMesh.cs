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

        IMeshData meshData;

        Vector3 position;
        Vector3 rotation;
        bool setup = false;

        bool initialized = false;
        LineInfo lineInfo;

        Color selectedArrowColor = new Color(1, 0.5f, 0, 1);
        Color normalArrowColor = new Color(0, 0.5f, 1, 1);

        void Start()
        {
            material = GetComponent<MeshRenderer>().material;
        }

        // public MeshData Data
        // {
        //     get => meshData;
        //     set => SetData(value);
        // }

        public void Initialize(IMeshData data)
        {
            var mesh = new Mesh();
            gameObject.name = data.Timestamp.ToString();
            mesh.SetVertices(data.Vertices);
            mesh.SetIndices(Enumerable.Range(0, data.Vertices.Count).Select(x => x).ToArray(),
                MeshTopology.Points,
                0);

            var mf = GetComponent<MeshFilter>();
            mf.sharedMesh = mesh;

            this.position = data.Position;
            this.rotation = data.Rotation;

            // transform.position = data.position;
            // transform.rotation = Quaternion.Euler(data.rotation);
            // this.setup = true;

            lineInfo = new LineInfo
            {
                startPos = data.Position,
                endPos = (data.Position + Quaternion.Euler(data.Rotation) * Vector3.forward).normalized,
                width = 0.05f,
                fillColor = normalArrowColor,
                forward = Vector3.up,
                endArrow = true,
                arrowLength = 0.1f,
                arrowWidth = 0.2f
            };
            initialized = true;
        }

        void Update()
        {
            if (initialized)
            {
                LineSegment.Draw(lineInfo);
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
                lineInfo.fillColor = selectedArrowColor;
            }
            else
            {
                this.material.SetColor("_Color", Color.white);
                lineInfo.fillColor = normalArrowColor;
            }
        }
    }
}