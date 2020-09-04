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
    public class MeshDataSet: IMeshDataSet
    {
        DatabaseReference dataRef;

        public string Key { get; private set; }
        public DateTime Timestamp{ get; private set; }

        public MeshDataSet(DatabaseReference dataRef, string key, DateTime timestamp)
        {
            this.dataRef = dataRef;
            this.Key = key;
            this.Timestamp = timestamp;
        }

        void IMeshDataSet.NotifySelected(bool selected)
        {
            if (selected)
            {
                dataRef.ChildAdded += onMeshDataAdded;
            }else {
                dataRef.ChildAdded -= onMeshDataAdded;
            }
        }

        ReactiveCollection<IMeshData> meshDataCollection = new ReactiveCollection<IMeshData>();
        IReadOnlyReactiveCollection<IMeshData> IMeshDataSet.MeshDataCollection { get => meshDataCollection; }

        async void onMeshDataAdded(object sender, ChildChangedEventArgs args)
        {
            var data = await UniTask.Run(() =>
            {
                return SerializableMeshData.FromJson(args.Snapshot.GetRawJsonValue());
            });
            meshDataCollection.Add(data);
        }
    }

    public class FirebaseMeshDatabase : MonoBehaviour, IMeshDatabase
    {
        [SerializeField] FirebaseManager firebaseManager;

        DatabaseReference rootRef;
        DatabaseReference sessionsRef;
        DatabaseReference currentSessionRef;

        ReactiveCollection<IMeshDataSet> meshDataCollection = new ReactiveCollection<IMeshDataSet>();
        IReadOnlyReactiveCollection<IMeshDataSet> IMeshDatabase.MeshDataSetCollection { get => meshDataCollection; }

        void onSessionAdded(object sneder, ChildChangedEventArgs args)
        {
            var key = args.Snapshot.Key;
            var timestamp = args.Snapshot.Child("timestamp").GetValue(false) as string;
            Debug.Log($"onSessionAdded: {timestamp}");

            var dt = new DateTime();
            System.DateTime.TryParse(timestamp, out dt);
            var dataRef = rootRef.Child("meshes").Child(key);
            meshDataCollection.Add(new MeshDataSet(dataRef, key, dt));
        }

        void onSessionRemoved(object sneder, ChildChangedEventArgs args)
        {
        }

        void Start()
        {
            firebaseManager.OnCreated += () =>
            {
                rootRef = firebaseManager.Database.RootReference.Child("db");
                sessionsRef = rootRef.Child("sessions");
                sessionsRef.ChildAdded += onSessionAdded;
                sessionsRef.ChildRemoved += onSessionRemoved;
            };
        }
    }
}