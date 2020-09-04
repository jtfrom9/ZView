using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UniRx;

namespace ZView
{
    public interface IMeshData
    {
        string Key { get; }
        DateTime Timestamp { get; }
        List<Vector3> Vertices { get; }
        Vector3 Position { get; }
        Vector3 Rotation { get; }
    }

    public static class IMeshDataExtension {
        public static string ToString(this IMeshData meshData) {
            return $"{meshData.Timestamp.ToString()}, {meshData.Vertices.Count.ToString()} points";
        }
    }

    public interface IMeshDataSet
    {
        string Key { get; }
        DateTime Timestamp { get; }
        void NotifySelected(bool v);

        IReadOnlyReactiveCollection<IMeshData> MeshDataCollection { get; }

    }

    public interface IMeshDatabase
    {
        IReadOnlyReactiveCollection<IMeshDataSet> MeshDataSetCollection { get; }
    }
}
