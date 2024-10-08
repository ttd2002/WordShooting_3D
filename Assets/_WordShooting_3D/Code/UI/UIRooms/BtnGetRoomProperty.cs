using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnGetRoomProperty : ButtonBase
{
    protected override void OnClick()
    {
        string sessionName = transform.Find("Background").Find("RoomName").GetComponent<Text>().text;
        string roomKey = transform.GetComponent<RoomItemPrefab>().roomPassword;
        UIManagerMainMenu.Instance.Rooms.roomName = sessionName;
        UIManagerMainMenu.Instance.Rooms.roomPassword = roomKey;
        if (roomKey != "")
        {
            UIManagerMainMenu.Instance.Rooms.OpenTextFieldPwdJoin();
        }
        else
        {
            NetworkManager.Instance.ConnectToSession(sessionName);
        }
    }
}
