using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Currency : MonoBehaviour
{
    [SerializeField] int amount;
    [SerializeField] TMPro.TextMeshProUGUI text;

    public void Add(int moneyGain)
    {
        amount += moneyGain;
        UpdateText();
    }

    private void Start()
    {
        amount = 1000;
        UpdateText();
    }

    private void UpdateText()
    {
        text.text = amount.ToString();
    }

}
