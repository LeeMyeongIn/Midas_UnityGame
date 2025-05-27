using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SleepChoicePanelManager : MonoBehaviour
{
    public Button sleepButton;
    public Button saveOnlyButton;
    public Button cancelButton;

    private Sleep sleep;

    void Start()
    {
        sleep = FindObjectOfType<Sleep>();

        if (sleep == null)
        {
            Debug.LogWarning("Sleep 스크립트 찾을 수 없음");
            return;
        }

        sleepButton.onClick.AddListener(() =>
        {
            sleep.DoSleep();
            sleep.CloseSleepPanel();
        });

        saveOnlyButton.onClick.AddListener(() =>
        {
            sleep.SaveOnly();
            sleep.CloseSleepPanel();
        });

        cancelButton.onClick.AddListener(() =>
        {
            sleep.CloseSleepPanel();
        });
    }
}
