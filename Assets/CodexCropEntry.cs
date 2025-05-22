using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodexCropEntry : MonoBehaviour
{
    public Image iconImage;

    public void Initialize(Item cropItem, bool hasSeen)
    {
        iconImage.sprite = cropItem.icon;

        iconImage.color = hasSeen
            ? new Color(1f, 1f, 1f, 1f)
            : new Color(0.5f, 0.5f, 0.5f, 1f);
    }
}
