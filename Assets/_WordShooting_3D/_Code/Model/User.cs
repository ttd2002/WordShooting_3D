using System;
using System.Collections.Generic;

[Serializable]
public class User 
{
    public string email;
    public string name;
    public List<SingleGameHistory> singleHistory;
    public List<MultiGameHistory> multiHistory;
}
