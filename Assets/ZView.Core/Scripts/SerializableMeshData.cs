using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZView
{
    [SerializeField]
    public class SerializableMeshData : ISerializationCallbackReceiver, IMeshData
    {
        [SerializeField]
        public List<Vector3> vertices = new List<Vector3>();

        [SerializeField]
        string timestamp;

        [SerializeField]
        public Vector3 position;

        [SerializeField]
        public Vector3 rotation;

        DateTime timestamp_d;

        string key = "";
        public string Key {
            get => key;
            set => key = value;
        }

        public DateTime Timestamp { get { return timestamp_d; } }

        List<Vector3> IMeshData.Vertices { get => vertices; }
        Vector3 IMeshData.Position { get => position; }
        Vector3 IMeshData.Rotation { get => rotation; }

        public SerializableMeshData()
        {
            this.timestamp_d = DateTime.Now;
            this.timestamp = timestamp_d.ToString();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (!string.IsNullOrEmpty(timestamp))
            {
                timestamp_d = new DateTime();
                System.DateTime.TryParse(timestamp, out timestamp_d);
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        public string ToJson(bool pretty = false)
        {
            return JsonUtility.ToJson(this, pretty);
        }

        public static SerializableMeshData FromJson(string json)
        {
            return JsonUtility.FromJson<SerializableMeshData>(json);
        }
    }
}