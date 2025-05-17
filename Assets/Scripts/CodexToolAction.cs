using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodexToolAction : ToolAction
{
    public override void OnItemUsed(Item usedItem, ItemContainer inventory)
    {
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
