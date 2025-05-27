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

            GameObject toolBar = GameObject.Find("ToolBarPanel");
            if (toolBar != null)
            {
                toolBar.SetActive(false);

                Sleep sleep = FindObjectOfType<Sleep>();
                if (sleep != null)
                    sleep.toolBarPanel = toolBar;
            }

            Time.timeScale = 0f;
        }
    }
}
