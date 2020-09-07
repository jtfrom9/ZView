using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using UniRx;

namespace ZView
{
    public class FirebasePointCloudDataSet: IPointCloudDataSet, IDisposable
    {
        DatabaseReference dataRef;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public string Key { get; private set; }
        public DateTime Timestamp{ get; private set; }

        public FirebasePointCloudDataSet(DatabaseReference dataRef, string key, DateTime timestamp)
        {
            this.dataRef = dataRef;
            this.Key = key;
            this.Timestamp = timestamp;
        }

        void setupCallback(bool v)
        {
            if (v)
            {
                dataRef.ChildAdded += onDataAdded;
            }
            else
            {
                dataRef.ChildAdded -= onDataAdded;
            }
        }

        void IPointCloudDataSet.NotifySelected(bool selected)
        {
            setupCallback(selected);
        }

        ReactiveCollection<IPointCloudData> pcDataCollection = new ReactiveCollection<IPointCloudData>();
        IReadOnlyReactiveCollection<IPointCloudData> IPointCloudDataSet.PointCloudDataCollection { get => pcDataCollection; }

        async void onDataAdded(object sender, ChildChangedEventArgs args)
        {
            var data = await UniTask.Run(() =>
            {
                return SerializablePointCloudData.FromJson(args.Snapshot.GetRawJsonValue());
            },
                configureAwait: true, this.cancellationTokenSource.Token);
            pcDataCollection.Add(data);
        }

        public void Dispose()
        {
            if(this.cancellationTokenSource==null)
                return;
            setupCallback(false);
            this.cancellationTokenSource.Cancel();
            this.cancellationTokenSource = null;
            GC.SuppressFinalize(this);
        }
    }

    public class FirebasePointCloudDatabase : MonoBehaviour, IPointCloudDatabase
    {
        [SerializeField] FirebaseManager firebaseManager;

        DatabaseReference rootRef;
        DatabaseReference sessionsRef;

        ReactiveCollection<IPointCloudDataSet> pcDataSetCollection = new ReactiveCollection<IPointCloudDataSet>();
        IReadOnlyReactiveCollection<IPointCloudDataSet> IPointCloudDatabase.PointCloudDataSetCollection { get => pcDataSetCollection; }

        void onSessionAdded(object sneder, ChildChangedEventArgs args)
        {
            var key = args.Snapshot.Key;
            var timestamp = args.Snapshot.Child("timestamp").GetValue(false) as string;
            var path = args.Snapshot.Child("path").GetValue(false) as string;
            Debug.Log($"onSessionAdded: {timestamp}");

            var dt = new DateTime();
            System.DateTime.TryParse(timestamp, out dt);

            // path fallback
            if(string.IsNullOrEmpty(path))
                path = "meshes";
            var dataRef = rootRef.Child(path).Child(key);
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

        void clearCallback()
        {
            sessionsRef.ChildAdded -= onSessionAdded;
            sessionsRef.ChildRemoved -= onSessionRemoved;
        }

        public void Close()
        {
            if(rootRef==null)
                return;
            clearCallback();
            sessionsRef = null;
            foreach (var set in pcDataSetCollection)
            {
                (set as IDisposable)?.Dispose();
            }
            rootRef = null;
        }

        void OnApplicationQuit()
        {
            Debug.Log("OnApplicationQuit");
            Close();
        }
    }
}