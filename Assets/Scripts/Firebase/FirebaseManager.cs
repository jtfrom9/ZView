using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;

public class FirebaseManager : MonoBehaviour
{
    public FirebaseApp App { get; private set; }
    public FirebaseDatabase Database { get; private set; }
    public event Action OnCreated;

    public LogLevel LogLevel = LogLevel.Info;

    void Awake()
    {
        FirebaseApp.LogLevel = LogLevel;
    }

    void initialize()
    {
        this.App = FirebaseApp.Create();
        var opt = App.Options;
        Debug.Log($"$Firebase: Config = {{ ApiKey: {opt.ApiKey}, AppId: {opt.AppId}, DatabaseUrl: {opt.DatabaseUrl}, MessageSenderId: {opt.MessageSenderId}, ProjectId: {opt.ProjectId}, StorageBucket: {opt.StorageBucket} }}");
        this.Database = FirebaseDatabase.GetInstance(this.App);
        // this.Database.LogLevel = LogLevel;

        OnCreated?.Invoke();
    }

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log($"failed CheckAndFixDependenciesAsync: {task.Exception.InnerException.Message}");
            }
            else if (task.Result != DependencyStatus.Available)
            {
                Debug.Log($"Could not resolve all Firebase dependencies: {task.Result}");
            }
            else
            {
                initialize();
                Debug.Log("initialize done");
            }
        });
    }
}
