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
        IPointCloudDatabase pcDatabase;
        IDepthVisualizer depthVisualizer;
        IMeshDataCollectionListUIView dataListCollectionUIView;
        IMeshDataListUIView dataListUIView;

        IDisposable currentDataSetModelSubscriber;
        IPointCloudDataSet currentDataSet;

        CompositeDisposable compositeDisposable = new CompositeDisposable();
        // CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        // ctor inject
        public DepthVisualizerUIPresenter(IPointCloudDatabase database,
            IDepthVisualizer depthVisualizer,
            IMeshDataCollectionListUIView collectionListUIView,
            IMeshDataListUIView dataListUIView)
        {
            this.pcDatabase = database;
            this.depthVisualizer = depthVisualizer;
            this.dataListCollectionUIView = collectionListUIView;
            this.dataListUIView = dataListUIView;
        }

        void IInitializable.Initialize() // IInitializable
        {
            // Model -> View
            pcDatabase.PointCloudDataSetCollection.ObserveAdd().Subscribe(e => {
                var set = e.Value;
                Debug.Log($"[Presenter] DataSet Added: {set.Key}, {set.Timestamp.ToString()}");
                dataListCollectionUIView.Add(new MeshDataSetTag
                {
                    key = set.Key,
                    name = set.Timestamp.ToString()
                });
            }).AddTo(compositeDisposable);

            // Model <- View
            dataListCollectionUIView.Selected.Subscribe(tag =>
            {
                IPointCloudDataSet dataSet;
                Debug.Log($"[Presenter] DataSet Selected: {tag.key}, {tag.name}");
                if(tag==null) {
                    dataSet = null;
                } else
                {
                    dataSet = pcDatabase.PointCloudDataSetCollection.FirstOrDefault(set => set.Key == tag.key);
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
                        currentDataSetModelSubscriber = dataSet.PointCloudDataCollection.ObserveAdd().Subscribe(e =>
                        {
                            var pCloud = e.Value;
                            Debug.Log($"[Presenter] data added: {pCloud.ToStringAsPointCloud()}");

                            // add to list UI
                            var uiView = dataListUIView.Add(pCloud.Key, pCloud.Timestamp.ToString());

                            // add to Visualizer
                            depthVisualizer.Add(pCloud, uiView);
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
