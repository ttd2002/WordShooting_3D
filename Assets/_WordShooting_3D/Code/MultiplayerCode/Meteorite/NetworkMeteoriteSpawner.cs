using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class NetworkMeteoriteSpawner : SingletonNetworkSpawner<NetworkMeteoriteSpawner>
{
     public static string networkMeteorite = "NetworkMeteorite";

     [SerializeField]
     private List<NetworkObject> commonTextObjects = new List<NetworkObject>();
     public List<NetworkObject> GetActiveTextObjects()
     {
          commonTextObjects.RemoveAll(obj => obj == null || !obj.gameObject.activeInHierarchy);
          return commonTextObjects;
     }
     public NetworkObject GetFirstActiveObject()
     {
          GetActiveTextObjects();
          if (commonTextObjects.Count == 0)
          {
               return null;
          }
          return commonTextObjects[0];
     }
     public void MarkObjectAsInactive(NetworkObject obj)
     {
          if (obj == null) return;

          if (commonTextObjects.Contains(obj))
          {
               commonTextObjects.Remove(obj);
          }
     }
     public void MarkObjectAsActive(NetworkObject obj)
     {
          if (obj == null) return;
          if (!commonTextObjects.Contains(obj))
          {
               commonTextObjects.Add(obj);
          }
     }
}
