using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UniRx;

namespace ZView
{
    public interface IPointCloudData
    {
        string Key { get; }
        DateTime Timestamp { get; }
        List<Vector3> Vertices { get; }
        Vector3 Position { get; }
        Vector3 Rotation { get; }
    }

    public static class IPointCloudDataExtension
    {
        public static string ToStringAsPointCloud(this IPointCloudData pc)
        {
            return $"{pc.Vertices.Count.ToString()} points. at {pc.Timestamp.ToString()}";
        }
    }

    public interface IPointCloudDataSet
    {
        string Key { get; }
        DateTime Timestamp { get; }
        void NotifySelected(bool v);

        IReadOnlyReactiveCollection<IPointCloudData> PointCloudDataCollection { get; }

    }

    public interface IPointCloudDatabase
    {
        IReadOnlyReactiveCollection<IPointCloudDataSet> PointCloudDataSetCollection { get; }
    }
}
