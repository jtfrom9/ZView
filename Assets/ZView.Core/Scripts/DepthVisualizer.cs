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
    public interface IPointCloudDataUIView
    {
        string Key { get; }

        IObservable<bool> Enabled { get; }
        IObservable<bool> Selected { get; }
        IObservable<Unit> OnModifyPose { get; }
        IObservable<Unit> OnJump { get; }
    }

    public interface IDepthVisualizer
    {
        void Add(IPointCloudData data, IPointCloudDataUIView uiView);
    }

    public class DepthVisualizer : MonoBehaviour, IDepthVisualizer, IPointCloudDataCollectionListUIView
    {
        [SerializeField] GameObject pointCloudPrefab;

        Subject<PointCloudDataSetTag> selectedSubject = new Subject<PointCloudDataSetTag>();

        void IDepthVisualizer.Add(IPointCloudData data, IPointCloudDataUIView uiView)
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
        void IPointCloudDataCollectionListUIView.Add(PointCloudDataSetTag tag)
        {
            selectedSubject.OnNext(tag);
        }
        IObservable<PointCloudDataSetTag> IPointCloudDataCollectionListUIView.Selected { get => selectedSubject; }

    }
}
