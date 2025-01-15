using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BtnSingleHistory : ButtonBase
{
    protected override void OnClick()
    {
        SingleHistoryController.Instance.LoadSingleHistories += OnDataLoadedHandler;
        SingleHistoryController.Instance.LoadSingleHistory();
    }

    private void OnDataLoadedHandler(List<SingleGameHistory> histories)
    {
        foreach (Transform child in UIHistorySpawner.Instance.GetHolder())
        {
            GameObject.Destroy(child.gameObject);
        }

        if (histories != null)
        {
            foreach (SingleGameHistory history in histories)
            {
                Transform input = UIHistorySpawner.Instance.Spawn(UIHistorySpawner.inputSingle, Vector3.zero, Quaternion.identity);
                RectTransform rectTransform = input.GetComponent<RectTransform>();
                Vector3 currentPosition = rectTransform.localPosition;
                currentPosition.z = 0;
                rectTransform.localPosition = currentPosition;

                Vector3 currentRotation = rectTransform.localEulerAngles;
                currentRotation.y = 0;
                rectTransform.localEulerAngles = currentRotation;

                TextMeshProUGUI totalScore = input.transform.Find("TotalScore").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI time = input.transform.Find("TotalTime").GetComponent<TextMeshProUGUI>();
                totalScore.text = "Total score: " + history.GetTotalScore().ToString();
                time.text = "Total Time: " + history.GetTotalTime();

                input.transform.localScale = new Vector3(1, 1, 1);
                input.gameObject.SetActive(true);

            }
        }
        else
        {
            Debug.Log("No data loaded.");
        }

        SingleHistoryController.Instance.LoadSingleHistories -= OnDataLoadedHandler;
    }
}
