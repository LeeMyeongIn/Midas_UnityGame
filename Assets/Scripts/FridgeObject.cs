using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FridgeObject : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            int mask = LayerMask.GetMask("PlaceableObject");
            Collider2D hit = Physics2D.OverlapPoint(mousePos, mask);

            if (hit != null)
            {
                if (hit.gameObject == this.gameObject)
                {
                    CookingPanelManager.Instance.OpenPanel();
                }
            }
        }
    }

}
