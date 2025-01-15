using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BtnMultiHistory : ButtonBase
{
    protected override void OnClick()
    {
        MultiHistoryController.Instance.LoadMultiHistories += OnDataLoadedHandler;
        MultiHistoryController.Instance.LoadMultiHistory();
    }

    private void OnDataLoadedHandler(List<MultiGameHistory> histories)
    {
        foreach (Transform child in UIHistorySpawner.Instance.GetHolder())
        {
            GameObject.Destroy(child.gameObject);
        }

        if (histories != null)
        {
            foreach (MultiGameHistory history in histories)
            {
                Transform input = UIMultiHistorySpawner.Instance.Spawn(UIMultiHistorySpawner.inputMulti, Vector3.zero, Quaternion.identity);
                RectTransform rectTransform = input.GetComponent<RectTransform>();
                Vector3 currentPosition = rectTransform.localPosition;
                currentPosition.z = 0;
                rectTransform.localPosition = currentPosition;

                Vector3 currentRotation = rectTransform.localEulerAngles;
                currentRotation.y = 0;
                rectTransform.localEulerAngles = currentRotation;

                TextMeshProUGUI leaderBoard = input.transform.Find("Leaderboard").GetComponent<TextMeshProUGUI>();
                leaderBoard.text = history.GetLeaderBoard();

                input.transform.localScale = new Vector3(1, 1, 1);
                input.gameObject.SetActive(true);

            }
        }
        else
        {
            Debug.Log("No data loaded.");
        }

        MultiHistoryController.Instance.LoadMultiHistories -= OnDataLoadedHandler;
    }
}
