using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] AttackController attackController;
    Vector2 lastDir = Vector2.right;

    void Update()
    {
        Vector2 move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (move != Vector2.zero)
            lastDir = move.normalized;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Item currentItem = ToolbarController.Instance.GetItem;

            if (currentItem != null && currentItem.isWeapon)
            {
                attackController.Attack(currentItem.damage, lastDir);
                Debug.Log($"[공격] {currentItem.Name} 으로 {currentItem.damage} 데미지 공격");
            }
            else
            {
                Debug.Log("[공격 실패] 무기가 선택되지 않았습니다.");
            }
        }
    }
}