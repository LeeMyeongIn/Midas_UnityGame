using System.Collections.Generic;
using UnityEngine;

public class TriumphPanelManager : MonoBehaviour
{
    [Header("UI ���� ���")]
    public GameObject triumphSlotPrefab;
    public Transform contentParent;

    private void OnEnable()
    {
        if (TriumphManager.Instance != null)
        {
            RefreshUI();
        }
        else
        {
            Debug.LogWarning("TriumphManager.Instance�� ���� �ʱ�ȭ���� �ʾҽ��ϴ�.");
        }
    }

    private void Start()
    {
        if (TriumphManager.Instance != null)
        {
            RefreshUI();
            TriumphManager.Instance.onTriumphUpdated += RefreshUI;
        }
        else
        {
            Debug.LogWarning("Start �������� TriumphManager.Instance�� null�Դϴ�.");
        }
    }

    private void OnDisable()
    {
        if (TriumphManager.Instance != null)
        {
            TriumphManager.Instance.onTriumphUpdated -= RefreshUI;
        }
    }

    public void RefreshUI()
    {
        if (triumphSlotPrefab == null || contentParent == null)
        {
            Debug.LogWarning("TriumphPanelManager: prefab �Ǵ� contentParent�� �������� �ʾҽ��ϴ�.");
            return;
        }

        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var triumphSO in TriumphManager.Instance.triumphList)
        {
            GameObject go = Instantiate(triumphSlotPrefab, contentParent);
            TriumphSlotUI slot = go.GetComponent<TriumphSlotUI>();
            if (slot != null)
            {
                slot.Initialize(triumphSO.data);
            }
            else
            {
                Debug.LogWarning("TriumphSlotUI ������Ʈ�� ã�� �� �����ϴ�.");
            }
        }
    }
}
