using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace ZView
{
    [SerializeField]
    public class MeshData : ISerializationCallbackReceiver
    {
        [SerializeField]
        public List<Vector3> vertices = new List<Vector3>();

        [SerializeField]
        string timestamp;

        [SerializeField]
        public Vector3 position;

        DateTime timestamp_d;

        public DateTime Timestamp { get { return timestamp_d; } }

        public MeshData()
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

        public static MeshData FromJson(string json)
        {
            return JsonUtility.FromJson<MeshData>(json);
        }
    }
}
