using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEngine;

namespace ZView
{
    public class LoadPoints : MonoBehaviour
    {
        [SerializeField]
        class Data
        {
            [SerializeField]
            public List<Vector3> vertices = new List<Vector3>();
        }

        public string readText(string path)
        {
            string strStream = "";
            try
            {
                using (StreamReader sr = new StreamReader(Application.dataPath + path))
                {
                    strStream = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

            return strStream;
        }

        Mesh genMesh(Data d)
        {
            var mesh = new Mesh();
            mesh.SetVertices(d.vertices);
            mesh.SetIndices(Enumerable.Range(0, d.vertices.Count).Select(x => x).ToArray(),
                MeshTopology.Points,
                0);
            return mesh;
        }

        void Start()
        {
            Debug.Log(Application.dataPath);
            var json = readText("/vertex.json");
            Debug.Log(json);
            var d = JsonUtility.FromJson<Data>(json);
            foreach (var p in d.vertices)
            {
                Debug.Log(p);
            }

            var mf = GetComponent<MeshFilter>();
            mf.sharedMesh = genMesh(d);
        }
    }
}