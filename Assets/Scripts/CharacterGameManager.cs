using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGameManager : MonoBehaviour
{
    public static CharacterGameManager Instance { get; private set; }

    public PlayerData playerData;

    public PlayerDataForSave loadedSaveData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
