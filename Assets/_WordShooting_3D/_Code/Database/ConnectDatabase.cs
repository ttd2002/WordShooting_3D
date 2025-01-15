using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase.Database;
using System.Threading.Tasks;

public class ConnectDatabase : SingletonAbstract<ConnectDatabase>
{
    public string userName;
    public string userId;
    public DatabaseReference dbRef;


    protected override void Awake()
    {
        base.Awake();
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        DontDestroyOnLoad(gameObject);
    }
    
    
    
}


