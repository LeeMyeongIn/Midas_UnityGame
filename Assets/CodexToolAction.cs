using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/ToolAction/Open Codex")]
public class CodexToolAction : ToolAction
{
    public override void OnItemUsed(Item usedItem, ItemContainer inventory)
    {
        // CodexUIManager�� ���� ������ �� ���� UI ����
        if (CodexUIManager.Instance != null)
        {
            CodexUIManager.Instance.OpenCodex();
        }
        else
        {
            Debug.LogWarning("CodexUIManager �ν��Ͻ��� ã�� �� �����ϴ�.");
        }
    }
}
