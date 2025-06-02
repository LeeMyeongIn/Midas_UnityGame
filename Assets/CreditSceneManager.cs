using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditSceneManager : MonoBehaviour
{
    [SerializeField] float creditDuration = 5f;  // 크레딧 보여줄 시간

    void Start()
    {
        StartCoroutine(GoToMainMenu());
    }

    IEnumerator GoToMainMenu()
    {
        yield return new WaitForSeconds(creditDuration);
        SceneManager.LoadScene("MainMenuScene");
    }
}
