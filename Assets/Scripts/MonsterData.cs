using UnityEngine;

[System.Serializable]
public class MonsterData
{
    public string id;
    public string monsterName;
    public string spritePath;

    [System.NonSerialized]
    public Sprite monsterSprite;
}

