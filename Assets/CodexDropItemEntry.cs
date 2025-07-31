using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CodexDropItemEntry : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private GameObject lockOverlay;

    public void Initialize(Item item, bool isSeen)
    {
        itemIcon.sprite = item.icon;
        itemName.text = item.Name;

        lockOverlay.SetActive(!isSeen);
        itemIcon.color = isSeen ? Color.white : Color.gray;
    }
}
