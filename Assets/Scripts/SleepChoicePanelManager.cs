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
            Debug.LogWarning("Sleep ��ũ��Ʈ ã�� �� ����");
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
            sleep.SaveOnly();  // ����
            sleep.CloseSleepPanel();  // �г� �ݱ�


#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();  // ����
#endif
        });
    }
}
