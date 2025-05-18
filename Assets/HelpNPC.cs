using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpNPC : Interactable
{
    public GameObject helpDialoguePanel;
    public Text dialogueUIText;
    [TextArea(3, 10)]
    public string dialogueText;

    void Start()
    {
        if (helpDialoguePanel != null)
            helpDialoguePanel.SetActive(false);
    }

    public override void Interact(Character character)
    {
        if (helpDialoguePanel != null)
        {
            helpDialoguePanel.SetActive(true);
            if (dialogueUIText != null)
                dialogueUIText.text = dialogueText;
        }
    }
}
