using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodexUIManager : MonoBehaviour
{
    public static CodexUIManager Instance;

    [Header("Codex UI ��ü �г�")]
    [SerializeField] private GameObject codexRootUI; // CodexUI ��ü �г� (��� ����)

    [Header("�� ���� ������")]
    [SerializeField] private GameObject foodPage;     // �丮 ���� ������
    [SerializeField] private GameObject cropPage;     // �۹� ���� ������
    [SerializeField] private GameObject triumphPage;  // ���� ���� ������

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // ���� ���� �� ����â ���α�
        if (codexRootUI != null)
            codexRootUI.SetActive(false);
    }

    /// <summary>
    /// ���� UI ���� (�⺻�� �丮 ������ ����)
    /// </summary>
    public void OpenCodex()
    {
        if (codexRootUI != null)
            codexRootUI.SetActive(true);

        ShowFoodPage(); // �⺻���� �丮 ��
    }

    /// <summary>
    /// ���� UI �ݱ�
    /// </summary>
    public void CloseCodex()
    {
        if (codexRootUI != null)
            codexRootUI.SetActive(false);
    }

    /// <summary>
    /// �丮 ���� ������ ǥ��
    /// </summary>
    public void ShowFoodPage()
    {
        if (foodPage != null) foodPage.SetActive(true);
        if (cropPage != null) cropPage.SetActive(false);
        if (triumphPage != null) triumphPage.SetActive(false);
    }

    /// <summary>
    /// �۹� ���� ������ ǥ��
    /// </summary>
    public void ShowCropPage()
    {
        if (foodPage != null) foodPage.SetActive(false);
        if (cropPage != null) cropPage.SetActive(true);
        if (triumphPage != null) triumphPage.SetActive(false);
    }

    /// <summary>
    /// ���� ���� ������ ǥ��
    /// </summary>
    public void ShowTriumphPage()
    {
        if (foodPage != null) foodPage.SetActive(false);
        if (cropPage != null) cropPage.SetActive(false);
        if (triumphPage != null) triumphPage.SetActive(true);
    }
}
