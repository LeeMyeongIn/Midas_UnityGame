using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingPanelController : MonoBehaviour
{
    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}
