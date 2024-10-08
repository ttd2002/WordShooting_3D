using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase.Database;

public class FirebaseManager : SingletonAbstract<FirebaseManager>
{

    protected List<SingleGameHistory> usersList;
    public string userId;
    DatabaseReference dbRef;
    public event Action<List<SingleGameHistory>> OnDataLoaded;
    protected override void Awake()
    {
        base.Awake();
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveSingleGameHistory(SingleGameHistory singleGameHistory)
    {
        string json = JsonUtility.ToJson(singleGameHistory);

        DatabaseReference userArrayRef = dbRef.Child("users").Child(this.userId).Push();
        Debug.Log("json: " + json);
        Debug.Log("singleGameHistory: " + singleGameHistory.GetTimeStamp());

        userArrayRef.SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Data saved successfully.");
            }
            else
            {
                Debug.LogError("Data save failed: " + task.Exception);
            }
        });
    }

    public void LoadSingleHistory()
    {
        StartCoroutine(LoadSingleGameHistoryEnum());
    }

    private IEnumerator LoadSingleGameHistoryEnum()
    {
        var userCheck = dbRef.Child("users").Child(userId).GetValueAsync();
        yield return new WaitUntil(predicate: () => userCheck.IsCompleted);

        if (userCheck.IsFaulted)
        {
            Debug.LogError("User ID check failed: " + userCheck.Exception);
            OnDataLoaded?.Invoke(null);
        }
        else if (userCheck.IsCompleted)
        {
            if (userCheck.Result.Exists)
            {
                DataSnapshot snapshot = userCheck.Result;
                usersList = new List<SingleGameHistory>();
                foreach (DataSnapshot childSnapshot in snapshot.Children)
                {
                    string childJson = childSnapshot.GetRawJsonValue();
                    if (!string.IsNullOrEmpty(childJson))
                    {
                        SingleGameHistory user = JsonUtility.FromJson<SingleGameHistory>(childJson);
                        usersList.Add(user);
                    }
                }
                OnDataLoaded?.Invoke(usersList);
            }
            else
            {
                Debug.Log("User ID does not exist");
                OnDataLoaded?.Invoke(null);
            }
        }
    }

}
