using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public ToolsCharacterController toolsCharacterController;

    public GameObject player;
    public ItemContainer inventoryContainer;
    public ItemDragAndDropController dragAndDropController;
    public DayTimeController timeController;
    public DialogueSystem dialogueSystem;
    public PlaceableObjectsReferenceManager placebleObjects;
    public ItemList itemDB;
    public OnScreenMessageSystem messageSystem;
    public ScreenTint screenTint;
    public GridLayout gridLayout;
    public TileMapReadController tileMapReadController;

    private void Awake()
    {
        instance = this;
    }
}
