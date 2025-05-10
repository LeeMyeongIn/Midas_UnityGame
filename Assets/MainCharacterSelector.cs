using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterSelector : MonoBehaviour
{
    [SerializeField] private Sprite[] characterSprites;
    [SerializeField] private RuntimeAnimatorController[] characterAnimators;
    [SerializeField] private PlayerData playerData;

    private Animator animator;

    private void OnEnable()
    {
        int index = playerData.selectedCharacterIndex;

        // 스프라이트 설정
        if (index >= 0 && index < characterSprites.Length)
        {
            GetComponent<SpriteRenderer>().sprite = characterSprites[index];
        }
        else
        {
            Debug.LogWarning("Invalid character index in PlayerData: " + index);
        }

        // 애니메이터 설정
        animator = GetComponent<Animator>();

        animator.runtimeAnimatorController = null;

        if (index >= 0 && index < characterAnimators.Length)
        {
            animator.runtimeAnimatorController = characterAnimators[index];
        }
        else
        {
            Debug.LogWarning("Invalid animator index in PlayerData: " + index);
        }

        Debug.Log("Animator set to: " + characterAnimators[index].name);
    }
}
