using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectorUI : MonoBehaviour
{
    public Sprite[] characterSprites;
    public Image characterImage;
    public PlayerData playerData;

    private int currentIndex = 0;

    void Start()
    {
        ShowCharacter();
    }

    public void NextCharacter()
    {
        currentIndex = (currentIndex + 1) % characterSprites.Length;
        ShowCharacter();
    }

    public void PrevCharacter()
    {
        currentIndex = (currentIndex - 1 + characterSprites.Length) % characterSprites.Length;
        ShowCharacter();
    }

    void ShowCharacter()
    {
        characterImage.sprite = characterSprites[currentIndex];
    }

    public void ConfirmCharacterSelection()
    {
        if (currentIndex >= 0 && currentIndex < characterSprites.Length)
        {
            playerData.selectedCharacterIndex = currentIndex;
            Debug.Log("선택된 캐릭터 인덱스: " + currentIndex);
        }
        else
        {
            Debug.LogWarning("Invalid character index selected");
        }
    }
}
