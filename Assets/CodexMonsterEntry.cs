using UnityEngine;
using UnityEngine.UI;
using System;

public class CodexMonsterEntry : MonoBehaviour
{
    [SerializeField] private Image monsterImage;
    private string monsterId;
    private Action<string> onClick;

    public void Initialize(MonsterInfo info, bool isSeen, Action<string> onClickAction)
    {
        monsterId = info.MonsterId;
        onClick = onClickAction;


        if (monsterImage != null)
        {
            monsterImage.sprite = info.icon;
            monsterImage.color = isSeen
                ? Color.white
                : new Color(1f, 1f, 1f, 0.3f);
        }
    }
}
