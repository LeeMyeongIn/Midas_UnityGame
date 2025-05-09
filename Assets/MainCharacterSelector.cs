using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterSelector : MonoBehaviour
{
    [SerializeField] private Sprite[] characterSprites;
    [SerializeField] private PlayerData playerData;

    private void Start()
    {
        int index = playerData.selectedCharacterIndex;

        if (index >= 0 && index < characterSprites.Length)
        {
            GetComponent<SpriteRenderer>().sprite = characterSprites[index];
        }
        else
        {
            Debug.LogWarning("Invalid character index in PlayerData: " + index);
        }
    }
}
