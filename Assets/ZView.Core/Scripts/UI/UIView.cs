using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace ZView
{
    public interface IPointCloudDataListUIView
    {
        IPointCloudDataUIView Add(string key, string text);
    }

    public class PointCloudDataSetTag
    {
        public string key;
        public string name;
    }

    public interface IPointCloudDataCollectionListUIView
    {
        void Add(PointCloudDataSetTag tag);
        IObservable<PointCloudDataSetTag> Selected { get; }
    }
}
