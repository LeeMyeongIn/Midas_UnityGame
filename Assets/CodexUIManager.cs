using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodexUIManager : MonoBehaviour
{
    public static CodexUIManager Instance;

    [Header("Codex UI 전체 패널")]
    [SerializeField] private GameObject codexRootUI; // CodexUI 전체 패널 (배경 포함)

    [Header("각 도감 페이지")]
    [SerializeField] private GameObject foodPage;     // 요리 도감 페이지
    [SerializeField] private GameObject cropPage;     // 작물 도감 페이지
    [SerializeField] private GameObject triumphPage;  // 업적 도감 페이지

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // 게임 시작 시 도감창 꺼두기
        if (codexRootUI != null)
            codexRootUI.SetActive(false);
    }

    /// <summary>
    /// 도감 UI 열기 (기본은 요리 탭으로 시작)
    /// </summary>
    public void OpenCodex()
    {
        if (codexRootUI != null)
            codexRootUI.SetActive(true);

        ShowFoodPage(); // 기본으로 요리 탭
    }

    /// <summary>
    /// 도감 UI 닫기
    /// </summary>
    public void CloseCodex()
    {
        if (codexRootUI != null)
            codexRootUI.SetActive(false);
    }

    /// <summary>
    /// 요리 도감 페이지 표시
    /// </summary>
    public void ShowFoodPage()
    {
        if (foodPage != null) foodPage.SetActive(true);
        if (cropPage != null) cropPage.SetActive(false);
        if (triumphPage != null) triumphPage.SetActive(false);
    }

    /// <summary>
    /// 작물 도감 페이지 표시
    /// </summary>
    public void ShowCropPage()
    {
        if (foodPage != null) foodPage.SetActive(false);
        if (cropPage != null) cropPage.SetActive(true);
        if (triumphPage != null) triumphPage.SetActive(false);
    }

    /// <summary>
    /// 업적 도감 페이지 표시
    /// </summary>
    public void ShowTriumphPage()
    {
        if (foodPage != null) foodPage.SetActive(false);
        if (cropPage != null) cropPage.SetActive(false);
        if (triumphPage != null) triumphPage.SetActive(true);
    }
}
