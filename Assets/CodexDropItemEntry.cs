using UnityEngine;
using UnityEngine.UI;

public class CodexDropItemEntry : MonoBehaviour
{
    [SerializeField] private Image itemIcon;

    public void Initialize(Item item, bool isSeen)
    {
        if (itemIcon != null)
        {
            itemIcon.sprite = item.icon;
            itemIcon.color = isSeen
                ? Color.white
                : new Color(1f, 1f, 1f, 0.3f);

            itemIcon.rectTransform.sizeDelta = new Vector2(64f, 64f);
        }
    }
}
