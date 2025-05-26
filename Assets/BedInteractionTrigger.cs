using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedInteractionTrigger : MonoBehaviour
{
    public GameObject sleepChoicePanel;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            sleepChoicePanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
