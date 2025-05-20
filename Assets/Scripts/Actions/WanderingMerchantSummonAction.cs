using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tools/WanderingMerchantSummonAction")]
public class WanderingMerchantSummonAction : ToolAction
{
    

    public override void OnItemUsed(Item usedItem, ItemContainer inventory)
    {
        // 1. ������ ���� ��ȯ
        WanderingMerchantManager.Instance.SummonMerchant(5f, 300f);
        Debug.Log("������ ������ ���� �ð� �Ŀ� �����մϴ�!");

        // 2. �ֹ��� ������ ����
        if (inventory != null)
        {
            inventory.Remove(usedItem); // S ������ ����
            Debug.Log("�ֹ����� �κ��丮���� ���ŵǾ����ϴ�.");
        }
        else
        {
            Debug.LogWarning("�κ��丮�� null�Դϴ�. �������� ������ �� �����ϴ�.");
        }
    }
}

