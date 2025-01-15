using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;

public class SingleHistoryController : SingletonAbstract<SingleHistoryController>
{
    public event Action<List<SingleGameHistory>> LoadSingleHistories;
    private List<SingleGameHistory> SingleHistories;


    public void SaveSingleGameHistory(SingleGameHistory singleGameHistory)
    {
        string json = JsonUtility.ToJson(singleGameHistory);
        DatabaseReference userArrayRef = ConnectDatabase.Instance.dbRef.Child("users").Child(ConnectDatabase.Instance.userId).Child("SingleGameHistory").Push();
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
        var userCheck =  ConnectDatabase.Instance.dbRef.Child("users").Child(ConnectDatabase.Instance.userId).Child("SingleGameHistory").GetValueAsync();
        yield return new WaitUntil(predicate: () => userCheck.IsCompleted);

        if (userCheck.IsFaulted)
        {
            Debug.LogError("User ID check failed: " + userCheck.Exception);
            LoadSingleHistories?.Invoke(null);
        }
        else if (userCheck.IsCompleted)
        {
            if (userCheck.Result.Exists)
            {
                DataSnapshot snapshot = userCheck.Result;
                SingleHistories = new List<SingleGameHistory>();
                foreach (DataSnapshot childSnapshot in snapshot.Children)
                {
                    string childJson = childSnapshot.GetRawJsonValue();
                    if (!string.IsNullOrEmpty(childJson))
                    {
                        SingleGameHistory history = JsonUtility.FromJson<SingleGameHistory>(childJson);
                        SingleHistories.Add(history);
                    }
                }
                LoadSingleHistories?.Invoke(SingleHistories);
            }
            else
            {
                Debug.Log("User ID does not exist");
                LoadSingleHistories?.Invoke(null);
            }
        }
    }
}
