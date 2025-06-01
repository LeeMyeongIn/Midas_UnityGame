using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class EndingPanel : MonoBehaviour
{
    public static EndingPanel Instance;

    [Header("UI Elements")]
    public GameObject panelObject; // EndingPanel 자체를 참조 (또는 this.gameObject 사용)
    public Text questionText;
    public Button yesButton;
    public Button noButton;
    public Button conditionButton;

    private Action onConfirm;
    private string currentSceneName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }

        panelObject.SetActive(false);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 바뀌었고, 현재 씬이 처음 로드된 씬과 다르면 제거
        if (scene.name != currentSceneName)
        {
            Debug.Log("[EndingPanel] 씬 이동 감지됨 → 자동 제거됨");
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
        }
    }

    public void ShowConfirmation(Action confirmAction)
    {
        panelObject.SetActive(true);
        onConfirm = confirmAction;

        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();
        conditionButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(() =>
        {
            panelObject.SetActive(false);
            onConfirm?.Invoke();
        });

        noButton.onClick.AddListener(() =>
        {
            panelObject.SetActive(false);
        });

        conditionButton.onClick.AddListener(() =>
        {
            Debug.Log("조건 확인 버튼 클릭됨 (추후 기능 추가 가능)");
        });
    }


}
