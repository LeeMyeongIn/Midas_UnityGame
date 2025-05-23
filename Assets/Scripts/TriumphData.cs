using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TriumphData
{
    public string id;
    public string name;
    [TextArea]
    public string description;

    public TriumphType type;

    public int currentCount;
    public int targetCount;
    public bool isCompleted;
    public bool isRewardClaimed;

    public List<Item> rewardItems;
}
