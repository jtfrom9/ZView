using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace ZView
{
    public class FirebaseTest : MonoBehaviour
    {
        public FirebaseManager firebaseManager;

        void Start()
        {
            firebaseManager.OnCreated += async () =>
            {
                Debug.Log("fb ok");
                var root = firebaseManager.Database.RootReference;
                await root.Child("depth-meth/data").SetValueAsync("hoge").AsUniTask();
                Debug.Log("fg write ok");

                var key = root.Child("db").Push().Key;
                var data = new SerializableMeshData();
                data.vertices.Add(Vector3.zero);
                data.vertices.Add(new Vector3(1, 2, 3));
                data.vertices.Add(new Vector3(4, 5, 6));
            // await root.Child("db").Child(key).UpdateChildrenAsync(new Dictionary<string, object>()
            // {
            //     {"timestamp", DateTime.Now},
            //     {"vertices", new List<Vector3>() {
            //         new Vector3(1,2,3),
            //         new Vector3(3,4,5)
            //     }}
            // });
            await root.Child("db").Child(key).SetRawJsonValueAsync(data.ToJson());
            };
        }
    }
}
