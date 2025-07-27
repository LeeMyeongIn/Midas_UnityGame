using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ItemPickup : MonoBehaviour
{
    public Item item;

    private void Awake()
    {
        if (item != null)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.sprite = item.icon;
        }
    }

    public void PickUp()
    {
        InventoryController.Instance.AddItem(item);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PickUp();
        }
    }
}
