using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndingPanel : MonoBehaviour
{
    public static EndingPanel Instance;

    [Header("UI Elements")]
    public Text questionText;
    public Button yesButton;
    public Button noButton;
    public Button conditionButton;

    private Action onConfirm;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 다음 씬에서도 유지
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        gameObject.SetActive(false);
    }

    public void ShowConfirmation(Action confirmAction)
    {
        gameObject.SetActive(true);
        onConfirm = confirmAction;

        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();
        conditionButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            onConfirm?.Invoke();
        });

        noButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });

        conditionButton.onClick.AddListener(() =>
        {
        });
    }
}
