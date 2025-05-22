using System.Collections.Generic;
using UnityEngine;

public class TriumphPanelManager : MonoBehaviour
{
    [Header("UI 구성 요소")]
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
            Debug.LogWarning("TriumphManager.Instance가 아직 초기화되지 않았습니다.");
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
            Debug.LogWarning("Start 시점에도 TriumphManager.Instance가 null입니다.");
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
            Debug.LogWarning("TriumphPanelManager: prefab 또는 contentParent가 설정되지 않았습니다.");
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
                Debug.LogWarning("TriumphSlotUI 컴포넌트를 찾을 수 없습니다.");
            }
        }
    }
}
