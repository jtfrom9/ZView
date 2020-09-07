using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using UniRx;

namespace ZView
{
    public interface IDepthVisualizer
    {
        void Add(IPointCloudData data, IMeshDataUIView uiView);
    }

    public class DepthVisualizer : MonoBehaviour, IDepthVisualizer, IMeshDataCollectionListUIView
    {
        [SerializeField] GameObject pointCloudPrefab;

        Subject<MeshDataSetTag> selectedSubject = new Subject<MeshDataSetTag>();

        void IDepthVisualizer.Add(IPointCloudData data, IMeshDataUIView uiView)
        {
            Debug.Log(data.Timestamp);
            var depthMesh = Instantiate(pointCloudPrefab, this.transform).GetComponent<PointCloudView>();
            depthMesh.Initialize(data);

            uiView.Enabled.Subscribe(enabled => {
                depthMesh.gameObject.SetActive(enabled);
            }).AddTo(this);

            uiView.Selected.Subscribe(selected => {
                depthMesh.OnFocus(selected);
            }).AddTo(this);

            uiView.OnModifyPose.Subscribe(_ =>
            {
                depthMesh.ModifyPose();
            }).AddTo(this);

            uiView.OnJump.Subscribe(_ => {
                Camera.main.transform.position = data.Position;
                Camera.main.transform.rotation = Quaternion.Euler(data.Rotation);
            }).AddTo(this);
        }

        // temporary solution
        void IMeshDataCollectionListUIView.Add(MeshDataSetTag tag)
        {
            selectedSubject.OnNext(tag);
        }
        IObservable<MeshDataSetTag> IMeshDataCollectionListUIView.Selected { get => selectedSubject; }

#if false
        async void onMeshDataAdded(object sender, ChildChangedEventArgs args)
        {
            var data = await UniTask.Run(() =>
            {
                return MeshData.FromJson(args.Snapshot.GetRawJsonValue());
            });

            // Model
            Debug.Log(data.Timestamp);
            var depthMesh = Instantiate(depthMeshPrefab, this.transform).GetComponent<DepthMesh>();
            depthMesh.Initialize(data);

            // View
            var item = Instantiate(depthMeshItemPanelPrefab).GetComponent<DepthMeshItemPanelView>();
            item.transform.SetParent(panelList.transform);
            item.Initialize(depthMesh.name);
            item.Enabled.Subscribe(enabled =>
            {
                depthMesh.gameObject.SetActive(enabled);
            }).AddTo(this);
            item.Selected.Subscribe(selected =>
            {
                depthMesh.OnFocus(selected);
            }).AddTo(this);
            item.ModifyPose.Subscribe(__ => depthMesh.ModifyPose()).AddTo(this);
            item.Jump.Subscribe(__ =>
            {
                Camera.main.transform.position = data.position;
                Camera.main.transform.rotation = Quaternion.Euler(data.rotation);
            }).AddTo(this);
        }

        void onMeshDataRemoved(object sender, ChildChangedEventArgs args)
        {
            var data = MeshData.FromJson(args.Snapshot.GetRawJsonValue());
            var go = GameObject.Find(data.Timestamp.ToString());
            if (go != null)
            {
                GameObject.Destroy(go);
                Debug.Log($"removed {go.name}");

                GameObject.Destroy(GameObject.Find($"camera ({go.name})"));
            }
        }

        void onMeshDataSetSelectedFromUI(MeshDataSet mds)
        {
            if(!meshDataSetList.ContainsValue(mds))
                return;
            var x = meshDataSetList.FirstOrDefault(kv => kv.Value == mds);
            currentSessionRef = sessionsRef.Child(x.Key);
            currentSessionRef.ChildAdded += onMeshDataAdded;
            currentSessionRef.ChildRemoved += onMeshDataRemoved;
        }

        void onMeshDataSetDeselectedFromUI(MeshDataSet mds)
        {
            if (!meshDataSetList.ContainsValue(mds))
                return;
            var x = meshDataSetList.FirstOrDefault(kv => kv.Value == mds);
            currentSessionRef = sessionsRef.Child(x.Key);
            currentSessionRef.ChildAdded -= onMeshDataAdded;
            currentSessionRef.ChildRemoved -= onMeshDataRemoved;
        }
#endif

    }
}
