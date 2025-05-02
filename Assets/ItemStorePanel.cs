using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStorePanel : ItemPanel
{
    [SerializeField] Trading trading;

    public override void OnClick(int id)
    {
        SellItem();

        Show();
    }

    private void SellItem()
    {
        trading.SellItem();
    }
}
