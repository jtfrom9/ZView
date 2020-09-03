using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using UniRx;

namespace ZView
{
    public class DepthVisualizer : MonoBehaviour
    {
        public FirebaseManager firebaseManager;
        public GameObject depthMeshPrefab;
        public GameObject positionPrefab;

        public GameObject depthMeshItemPanelPrefab;
        DepthMeshItemListView panelList;

        DatabaseReference root;

        void Start()
        {
            panelList = FindObjectOfType<DepthMeshItemListView>();

            firebaseManager.OnCreated += () =>
            {
                if (root == null)
                {
                    root = firebaseManager.Database.RootReference.Child("db");

                    root.ChildAdded += (_, args) =>
                    {
                        var data = MeshData.FromJson(args.Snapshot.GetRawJsonValue());
                        Debug.Log(data.Timestamp);
                        var depthMesh = Instantiate(depthMeshPrefab, this.transform).GetComponent<DepthMesh>();
                        depthMesh.Initialize(data);
                        // depthMesh.position = data.position;
                        // depthMesh.rotation = data.rotation;

                        // {
                        //     var go = Instantiate(positionPrefab);
                        //     go.name = $"camera ({data.Timestamp.ToString()})";
                        //     go.transform.position = data.position;
                        // }
                        {
                            var item = Instantiate(depthMeshItemPanelPrefab).GetComponent<DepthMeshItemPanelView>();
                            item.transform.SetParent(panelList.transform);
                            item.Initialize(depthMesh.name);
                            item.Enabled.Subscribe(enabled => {
                                depthMesh.gameObject.SetActive(enabled);
                            }).AddTo(this);
                            item.Selected.Subscribe(selected => {
                                depthMesh.OnFocus(selected);
                            }).AddTo(this);
                            item.ModifyPose.Subscribe(__ => depthMesh.ModifyPose()).AddTo(this);
                            item.Jump.Subscribe(__ => {
                                Camera.main.transform.position = data.position;
                                Camera.main.transform.rotation = Quaternion.Euler(data.rotation);
                            }).AddTo(this);
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
