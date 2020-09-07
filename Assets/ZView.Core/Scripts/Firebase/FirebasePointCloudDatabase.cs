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
    public class FirebasePointCloudDataSet: IPointCloudDataSet
    {
        DatabaseReference dataRef;

        public string Key { get; private set; }
        public DateTime Timestamp{ get; private set; }

        public FirebasePointCloudDataSet(DatabaseReference dataRef, string key, DateTime timestamp)
        {
            this.dataRef = dataRef;
            this.Key = key;
            this.Timestamp = timestamp;
        }

        void IPointCloudDataSet.NotifySelected(bool selected)
        {
            if (selected)
            {
                dataRef.ChildAdded += onDataAdded;
            }else {
                dataRef.ChildAdded -= onDataAdded;
            }
        }

        ReactiveCollection<IPointCloudData> pcDataCollection = new ReactiveCollection<IPointCloudData>();
        IReadOnlyReactiveCollection<IPointCloudData> IPointCloudDataSet.PointCloudDataCollection { get => pcDataCollection; }

        async void onDataAdded(object sender, ChildChangedEventArgs args)
        {
            var data = await UniTask.Run(() =>
            {
                return SerializablePointCloudData.FromJson(args.Snapshot.GetRawJsonValue());
            });
            pcDataCollection.Add(data);
        }
    }

    public class FirebasePointCloudDatabase : MonoBehaviour, IPointCloudDatabase
    {
        [SerializeField] FirebaseManager firebaseManager;

        DatabaseReference rootRef;
        DatabaseReference sessionsRef;
        DatabaseReference currentSessionRef;

        ReactiveCollection<IPointCloudDataSet> pcDataSetCollection = new ReactiveCollection<IPointCloudDataSet>();
        IReadOnlyReactiveCollection<IPointCloudDataSet> IPointCloudDatabase.PointCloudDataSetCollection { get => pcDataSetCollection; }

        void onSessionAdded(object sneder, ChildChangedEventArgs args)
        {
            var key = args.Snapshot.Key;
            var timestamp = args.Snapshot.Child("timestamp").GetValue(false) as string;
            Debug.Log($"onSessionAdded: {timestamp}");

            var dt = new DateTime();
            System.DateTime.TryParse(timestamp, out dt);
            var dataRef = rootRef.Child("meshes").Child(key);
            pcDataSetCollection.Add(new FirebasePointCloudDataSet(dataRef, key, dt));
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