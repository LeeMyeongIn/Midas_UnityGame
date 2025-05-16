using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/ToolAction/Open Codex")]
public class CodexToolAction : ToolAction
{
    public override void OnItemUsed(Item usedItem, ItemContainer inventory)
    {
        // CodexUIManager가 씬에 존재할 때 도감 UI 열기
        if (CodexUIManager.Instance != null)
        {
            CodexUIManager.Instance.OpenCodex();
        }
        else
        {
            Debug.LogWarning("CodexUIManager 인스턴스를 찾을 수 없습니다.");
        }
    }
}
