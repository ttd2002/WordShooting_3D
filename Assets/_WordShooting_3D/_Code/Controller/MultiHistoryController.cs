using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;

public class MultiHistoryController : SingletonAbstract<MultiHistoryController>
{

    public event Action<List<MultiGameHistory>> LoadMultiHistories;
    private List<MultiGameHistory> MultiHistories;
    public void SaveMultiGameHistory(MultiGameHistory multiGameHistory)
    {
        string json = JsonUtility.ToJson(multiGameHistory);
        DatabaseReference userArrayRef = ConnectDatabase.Instance.dbRef.Child("users").Child(ConnectDatabase.Instance.userId).Child("MultiGameHistory").Push();
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

    public void LoadMultiHistory()
    {
        StartCoroutine(LoadMultiGameHistoryEnum());
    }

    private IEnumerator LoadMultiGameHistoryEnum()
    {
        var userCheck = ConnectDatabase.Instance.dbRef.Child("users").Child(ConnectDatabase.Instance.userId).Child("MultiGameHistory").GetValueAsync();
        yield return new WaitUntil(predicate: () => userCheck.IsCompleted);

        if (userCheck.IsFaulted)
        {
            Debug.LogError("Failed to load MultiGameHistory: " + userCheck.Exception);
            LoadMultiHistories?.Invoke(null);
        }
        else if (userCheck.IsCompleted)
        {
            if (userCheck.Result.Exists)
            {
                DataSnapshot snapshot = userCheck.Result;
                MultiHistories = new List<MultiGameHistory>();

                foreach (DataSnapshot childSnapshot in snapshot.Children)
                {
                    string childJson = childSnapshot.GetRawJsonValue();
                    if (!string.IsNullOrEmpty(childJson))
                    {
                        MultiGameHistory history = JsonUtility.FromJson<MultiGameHistory>(childJson);
                        MultiHistories.Add(history);
                    }
                }
                LoadMultiHistories?.Invoke(MultiHistories);
            }
            else
            {
                Debug.Log("No MultiGameHistory data found for this user.");
                LoadMultiHistories?.Invoke(null);

            }
        }

    }
}
