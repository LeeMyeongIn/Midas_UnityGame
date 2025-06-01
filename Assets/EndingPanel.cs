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
    public GameObject panelObject; // EndingPanel ��ü�� ���� (�Ǵ� this.gameObject ���)
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
        // ���� �ٲ����, ���� ���� ó�� �ε�� ���� �ٸ��� ����
        if (scene.name != currentSceneName)
        {
            Debug.Log("[EndingPanel] �� �̵� ������ �� �ڵ� ���ŵ�");
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
            Debug.Log("���� Ȯ�� ��ư Ŭ���� (���� ��� �߰� ����)");
        });
    }


}
