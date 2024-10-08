using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;

public class SessionManager : MonoBehaviour
{
    public string SessionName { get; private set; }
    public Transform ListRoom;
    public void RefreshSession(List<SessionInfo> _session)
    {
        foreach (Transform child in ListRoom)
        {
            Destroy(child.gameObject);
        }

        foreach (SessionInfo sessionInfo in _session)
        {
            Debug.Log("sessionInfo: " + sessionInfo.Name);

            if (sessionInfo.IsVisible)
            {
                Transform roomItem = RoomItemSpawner.Instance.Spawn(RoomItemSpawner.inputRoom, Vector3.zero, Quaternion.identity);
                roomItem.gameObject.SetActive(true);
                roomItem.transform.localScale = new Vector3(1, 1, 1);
                roomItem.Find("Background/RoomName").GetComponent<Text>().text = sessionInfo.Name;
                roomItem.Find("Background/PlayerCount").GetComponent<Text>().text = $"{sessionInfo.PlayerCount}/{sessionInfo.MaxPlayers}";
                RoomItemPrefab script = roomItem.GetComponent<RoomItemPrefab>();
                script.roomPassword = sessionInfo.Properties.GetValueOrDefault("sessionKey").PropertyValue as string;
                if (script.roomPassword != "")
                {
                    Transform lockItem = roomItem.Find("Background").Find("Lock");
                    lockItem.gameObject.SetActive(true);
                }
            }
        }
    }

    
}
