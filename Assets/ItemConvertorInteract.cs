using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemConvertorInteract : Interactable
{
    [SerializeField] Item convertableItem;
    [SerializeField] Item producedItem;
    [SerializeField] int producedItemCount = 1;

    ItemSlot itemSlot;
     
    [SerializeField] float timeToprocess = 5f;
    float timer;

    Animator animator;

    private void Start()
    {
        itemSlot = new ItemSlot();
        animator = GetComponent<Animator>();
    }

    public override void Interact(Character character)
    {
        if (itemSlot.item == null)
        {
            if (GameManager.instance.dragAndDropController.Check(convertableItem))
            {
                StartItemProcessing();
            }
        }
        if(itemSlot.item != null && timer < 0f)
        {
            GameManager.instance.inventoryContainer.Add(itemSlot.item, itemSlot.count);
            itemSlot.Clear();
        }
    }
    private void StartItemProcessing()
    {
        animator.SetBool("Working", true);
        itemSlot.Copy(GameManager.instance.dragAndDropController.itemSlot);
        GameManager.instance.dragAndDropController.RemoveItem();

        timer = timeToprocess;
    }

    private void Update()
    {
        if(itemSlot == null) { return; }
        if (timer > 0f) 
        { 
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                CompleteItemConversion();
            }
        }
    }
    private void CompleteItemConversion()
    {
        animator.SetBool("Working", false);
        itemSlot.Clear();
        itemSlot.Set(producedItem, producedItemCount);
    }
}
