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

    }
}
