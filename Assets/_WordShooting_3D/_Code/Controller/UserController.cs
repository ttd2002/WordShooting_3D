using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UserController : SingletonAbstract<UserController>
{
    public async Task SaveUserDataAsync(User playerIn)
    {
        string userKey = playerIn.email.Replace("@", "_").Replace(".", "_");
        string userPath = "users/" + userKey;
        string json = JsonUtility.ToJson(playerIn);

        try
        {
            var snapshot = await ConnectDatabase.Instance.dbRef.Child(userPath).GetValueAsync();
            if (!snapshot.Exists)
            {
                await ConnectDatabase.Instance.dbRef.Child(userPath).SetRawJsonValueAsync(json);
            }
            Debug.Log("Login successful.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error saving user data: " + ex.Message);
        }
    }
}
