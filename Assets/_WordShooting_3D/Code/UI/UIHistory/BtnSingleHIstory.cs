using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BtnSingleHIstory : ButtonBase
{
    protected override void OnClick()
    {
        FirebaseManager.Instance.OnDataLoaded += OnDataLoadedHandler;
        FirebaseManager.Instance.LoadSingleHistory();
    }

    private void OnDataLoadedHandler(List<SingleGameHistory> users)
    {
        foreach (Transform child in UIHistorySpawner.Instance.GetHolder())
        {
            GameObject.Destroy(child.gameObject);
        }

        if (users != null)
        {
            foreach (SingleGameHistory user in users)
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
                totalScore.text = "Total score: " + user.GetTotalScore().ToString();
                time.text = "Total Time: " + user.GetTotalTime();

                input.transform.localScale = new Vector3(1, 1, 1);
                input.gameObject.SetActive(true);

            }
        }
        else
        {
            Debug.Log("No data loaded.");
        }

        FirebaseManager.Instance.OnDataLoaded -= OnDataLoadedHandler;
    }
}
