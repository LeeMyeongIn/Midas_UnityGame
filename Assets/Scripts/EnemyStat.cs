using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStat : MonoBehaviour, IDamageable
{
    [SerializeField] int maxHp = 10;
    int currHp;

    [Header("드랍 관련")]
    [SerializeField] DropTable dropTable;

    private void Awake()
    {
        currHp = maxHp;
    }

    public void CalculateDamage(ref int damage) { }

    public void ApplyDamage(int damage)
    {
        currHp -= damage;
    }

    public void CheckState()
    {
        if (currHp <= 0)
        {
            MonsterInfo info = GetComponent<MonsterInfo>();
            if (info != null)
            {
                MonsterUnlockManager.Instance.RegisterMonster(info.MonsterId);
                Debug.Log($"[Codex] 몬스터 도감 등록: {info.MonsterId}");
            }

            DropItem();
            Destroy(gameObject);
        }
    }

    private void DropItem()
    {
        if (dropTable == null) return;

        GameObject drop = dropTable.GetOneGuaranteedDrop();
        if (drop != null)
        {
            Instantiate(drop, transform.position, Quaternion.identity);
        }
    }
}
