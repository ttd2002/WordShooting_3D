using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnJoin : ButtonBase
{
    protected override void OnClick()
    {
        string sessionName = UIManagerMainMenu.Instance.Rooms.roomName;
        string sessionKey = UIManagerMainMenu.Instance.Rooms.roomPassword;
        string sessionPassword = transform.parent.Find("InputFieldPwd").Find("TextPwdJoin").GetComponent<Text>().text;

        Debug.Log("sessionName" + sessionName);
        Debug.Log("pass" + sessionKey);
        if (sessionKey == sessionPassword)
        {
            NetworkManager.Instance.ConnectToSession(sessionName);

        }
        else
        {
            Debug.Log("Wrong password!");
        }
    }
}
