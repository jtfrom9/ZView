using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace ZView
{
    public interface IMeshDataUIView
    {
        string Key { get; }

        IObservable<bool> Enabled { get; }
        IObservable<bool> Selected { get; }
        IObservable<Unit> OnModifyPose { get; }
        IObservable<Unit> OnJump { get; }
    }

    public interface IMeshDataListUIView
    {
        IMeshDataUIView Add(string key, string text);
    }

    public class MeshDataSetTag
    {
        public string key;
        public string name;
    }

    public interface IMeshDataCollectionListUIView
    {
        void Add(MeshDataSetTag tag);
        IObservable<MeshDataSetTag> Selected { get; }
    }
}
