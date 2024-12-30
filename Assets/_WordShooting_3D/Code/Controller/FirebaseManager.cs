using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase.Database;
using System.Threading.Tasks;

public class FirebaseManager : SingletonAbstract<FirebaseManager>
{

    
    public string userName;
    public string userId;
    public event Action<List<SingleGameHistory>> LoadSingleHistories;
    public event Action<List<MultiGameHistory>> LoadMultiHistories;
    private List<SingleGameHistory> SingleHistories;
    private List<MultiGameHistory> MultiHistories;
    private DatabaseReference _dbRef;

    protected override void Awake()
    {
        base.Awake();
        _dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        DontDestroyOnLoad(gameObject);
    }
    public async Task SaveUserDataAsync(User playerIn)
    {
        string userKey = playerIn.email.Replace("@", "_").Replace(".", "_");
        string userPath = "users/" + userKey;
        string json = JsonUtility.ToJson(playerIn);

        try
        {
            var snapshot = await _dbRef.Child(userPath).GetValueAsync();
            if (!snapshot.Exists)
            {
                await _dbRef.Child(userPath).SetRawJsonValueAsync(json);
            }
            Debug.Log("Login successful.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error saving user data: " + ex.Message);
        }
    }
    public void SaveSingleGameHistory(SingleGameHistory singleGameHistory)
    {
        string json = JsonUtility.ToJson(singleGameHistory);
        DatabaseReference userArrayRef = _dbRef.Child("users").Child(this.userId).Child("SingleGameHistory").Push();
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
    public void SaveMultiGameHistory(MultiGameHistory multiGameHistory)
    {
        string json = JsonUtility.ToJson(multiGameHistory);
        DatabaseReference userArrayRef = _dbRef.Child("users").Child(this.userId).Child("MultiGameHistory").Push();
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
        var userCheck = _dbRef.Child("users").Child(userId).Child("SingleGameHistory").GetValueAsync();
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
    public void LoadMultiHistory()
    {
        StartCoroutine(LoadMultiGameHistoryEnum());
    }

    private IEnumerator LoadMultiGameHistoryEnum()
    {
        var userCheck = _dbRef.Child("users").Child(userId).Child("MultiGameHistory").GetValueAsync();
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


