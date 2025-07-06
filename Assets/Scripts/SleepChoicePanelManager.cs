using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SleepChoicePanelManager : MonoBehaviour
{
    public Button sleepButton;
    public Button saveOnlyButton;
    public Button saveExitButton;
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

        saveExitButton.onClick.AddListener(() =>
        {
            sleep.SaveOnly();  // 저장
            sleep.CloseSleepPanel();  // 패널 닫기


#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();  // 종료
#endif
        });
    }
}
