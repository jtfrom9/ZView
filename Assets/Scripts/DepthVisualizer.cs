using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Firebase.Database;

namespace ZView
{
    public class DepthVisualizer : MonoBehaviour
    {
        public FirebaseManager firebaseManager;
        public GameObject depthMeshPrefab;
        public GameObject positionPrefab;

        DatabaseReference root;

        void Start()
        {
            firebaseManager.OnCreated += async () =>
            {
                if (root == null)
                {
                    root = firebaseManager.Database.RootReference.Child("db");

                    root.ChildAdded += (_, args) =>
                    {
                        var data = MeshData.FromJson(args.Snapshot.GetRawJsonValue());
                        Debug.Log(data.Timestamp);
                        var depthMesh = Instantiate(depthMeshPrefab, this.transform).GetComponent<DepthMesh>();
                        depthMesh.SetData(data);

                        {
                            var go = Instantiate(positionPrefab);
                            go.name = $"camera ({data.Timestamp.ToString()})";
                            go.transform.position = data.position;
                        }
                    };

                    root.ChildRemoved += (_, args) =>
                    {
                        var data = MeshData.FromJson(args.Snapshot.GetRawJsonValue());
                        var go = GameObject.Find(data.Timestamp.ToString());
                        if (go != null)
                        {
                            GameObject.Destroy(go);
                            Debug.Log($"removed {go.name}");

                            GameObject.Destroy(GameObject.Find($"camera ({go.name})"));
                        }
                    };
                }

                // var dp = await root.GetValueAsync().AsUniTask();

                // foreach (var c in dp.Children)
                // {
                //     var data = MeshData.FromJson(c.GetRawJsonValue());
                //     Debug.Log(data.Timestamp);

                //     var depthMesh = Instantiate(depthMeshPrefab, this.transform).GetComponent<DepthMesh>();
                //     depthMesh.SetData(data);
                // }
            };
        }
    }
}
