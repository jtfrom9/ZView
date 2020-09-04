using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using Zenject;

namespace ZView
{
    public class DepthVisualizerUIPresenter: IInitializable
    {
        IMeshDatabase meshDatabase;
        IDepthVisualizer depthVisualizer;
        IMeshDataCollectionListUIView collectionListUIView;
        IMeshDataListUIView dataListUIView;

        IDisposable currentDataSetModelSubscriber;
        IMeshDataSet currentDataSet;

        CompositeDisposable compositeDisposable = new CompositeDisposable();
        // CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        // ctor inject
        public DepthVisualizerUIPresenter(IMeshDatabase database,
            IDepthVisualizer depthVisualizer,
            IMeshDataCollectionListUIView collectionListUIView,
            IMeshDataListUIView dataListUIView)
        {
            this.meshDatabase = database;
            this.depthVisualizer = depthVisualizer;
            this.collectionListUIView = collectionListUIView;
            this.dataListUIView = dataListUIView;
        }

        void IInitializable.Initialize() // IInitializable
        {
            // Model -> View
            meshDatabase.MeshDataSetCollection.ObserveAdd().Subscribe(e => {
                var set = e.Value;
                Debug.Log($"[Presenter] DataSet Added: {set.Key}, {set.Timestamp.ToString()}");
                collectionListUIView.Add(new MeshDataSetTag
                {
                    key = set.Key,
                    name = set.Timestamp.ToString()
                });
            }).AddTo(compositeDisposable);

            // Model <- View
            collectionListUIView.Selected.Subscribe(tag =>
            {
                IMeshDataSet dataSet;
                Debug.Log($"[Presenter] DataSet Selected: {tag.key}, {tag.name}");
                if(tag==null) {
                    dataSet = null;
                } else
                {
                    dataSet = meshDatabase.MeshDataSetCollection.FirstOrDefault(set => set.Key == tag.key);
                }
                if (currentDataSet != dataSet)
                {
                    if (currentDataSetModelSubscriber != null)
                    {
                        currentDataSetModelSubscriber.Dispose();
                        currentDataSetModelSubscriber = null;
                    }
                    Debug.Log($"[Presenter] dataSet: {dataSet}");
                    if (dataSet != null)
                    {
                        currentDataSetModelSubscriber = dataSet.MeshDataCollection.ObserveAdd().Subscribe(e =>
                        {
                            var meshData = e.Value;
                            Debug.Log($"[Presenter] data added: {meshData.ToString()}");

                            // add to list UI
                            var uiView = dataListUIView.Add(meshData.Key, meshData.Timestamp.ToString());

                            // add to Visualizer
                            depthVisualizer.Add(meshData, uiView);
                        });
                    }
                    currentDataSet?.NotifySelected(false);
                    dataSet?.NotifySelected(true);
                    currentDataSet = dataSet;
                }
            }).AddTo(compositeDisposable);
        }

        void Dispose() // Disposable
        {
            // cancellationTokenSource.Cancel();
            // cancellationTokenSource.Dispose();
            compositeDisposable.Clear();
        }
    }
}
