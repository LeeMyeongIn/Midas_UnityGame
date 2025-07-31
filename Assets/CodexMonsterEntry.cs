using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CodexMonsterEntry : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject lockOverlay;
    [SerializeField] private Button button;

    private string monsterId;

    public void Initialize(MonsterInfo monsterInfo, bool isSeen, System.Action<string> onClickCallback)
    {
        monsterId = monsterInfo.MonsterId;

        nameText.text = isSeen ? monsterInfo.MonsterName : "???";
        iconImage.sprite = monsterInfo.icon;
        iconImage.color = isSeen ? Color.white : Color.gray;

        if (lockOverlay != null)
            lockOverlay.SetActive(!isSeen);

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClickCallback(monsterId));
        }
    }
}
