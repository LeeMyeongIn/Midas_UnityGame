using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ToolsCharacterController : MonoBehaviour
{
    CharacterLevel characterLevel;
    public CharacterController2D characterController2d;
    Character character;
    Rigidbody2D rgbd2d;
    ToolbarController toolbarController;
    Animator animator;
    [SerializeField] float offsetDistance = 1f;
    //[SerializeField] float sizeOfInteractableArea = 1.2f;
    [SerializeField] MarkerManager markerManager;
    [SerializeField] TileMapReadController tileMapReadcontroller;
    [SerializeField] float maxDistance = 1.5f;
    [SerializeField] ToolAction onTilePickUp;
    [SerializeField] IconHighlight iconHighlight;
    AttackController attackController;
    [SerializeField] int weaponEnergyCost = 5;

    Vector3Int selectedTilePosition;
    bool selectable;

    [SerializeField] float toolTimeOut = 1f;
    float timer;

    private void Awake()
    {
        character = GetComponent<Character>();
        characterController2d = GetComponent<CharacterController2D>();
        rgbd2d = GetComponent<Rigidbody2D>();
        toolbarController = GetComponent<ToolbarController>();
        animator = GetComponent<Animator>();
        attackController = GetComponent<AttackController>();
        characterLevel = GetComponent<CharacterLevel>();
    }

    private void Update()
    {
        if(timer > 0f) { timer -= Time.deltaTime; }

        if (Input.GetMouseButtonDown(0))
        {

            WeaponAction();
        }

        SelectTile();
        CanSelectCheck();
        Marker();
        if (Input.GetMouseButtonDown(0))
        {
            if (UseToolWorld() == true)
            {
                return;
            }
            UseToolGrid();
        }
    }

    private void WeaponAction()
    {
        if (timer > 0f) { return; }

        Item item = toolbarController.GetItem;
        if (item == null) { return; }
        if (item.isWeapon == false) { return; }

        EnergyCost(weaponEnergyCost);

        Vector2 position = rgbd2d.position + characterController2d.lastMotionVector * offsetDistance;

        attackController.Attack(item.damage, characterController2d.lastMotionVector);

        timer = toolTimeOut;
    }

    private void EnergyCost(int energyCost)
    {
        character.GetTired(energyCost);
    }

    private void SelectTile()
    {
        selectedTilePosition = tileMapReadcontroller.GetGridPosition(Input.mousePosition, true);
    }

    void CanSelectCheck()
    {
        Vector2 characterPosition = transform.position;
        Vector2 cameraPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        selectable = Vector2.Distance(characterPosition, cameraPosition) < maxDistance;
        markerManager.Show(selectable);
        iconHighlight.CanSelect = selectable;
    }

    private void Marker()
    {
        markerManager.markedCellPosition = selectedTilePosition;
        iconHighlight.cellPosition = selectedTilePosition;
    }

    private bool UseToolWorld()
    {
        if (timer > 0f) { return false; }

        Vector2 position = rgbd2d.position + characterController2d.lastMotionVector * offsetDistance;

        Item item = toolbarController.GetItem;
        if (item == null) { return false; }
        if (item.onAction == null) { return false; }

        EnergyCost(GetEnergyCost(item.onAction));

        if (item.onAction is RemovePlowing rakeTile)
        {
            rakeTile.lastMotionVector = characterController2d.lastMotionVector;
        }

        animator.SetTrigger("act");
        bool complete = item.onAction.OnApply(position);

        if (complete == true)
        {
            // add experience as a reward
            characterLevel.AddExperience(item.onAction.skillType, item.onAction.skillExperienceReward);

            if (item.onItemUsed != null)
            {
                item.onItemUsed.OnItemUsed(item, GameManager.instance.inventoryContainer);
            }
        }

        timer = toolTimeOut;

        return complete;
    }

    private void UseToolGrid()
    {
        if (timer > 0f) { return; }

        if (selectable == true)
        {
            Item item = toolbarController.GetItem;
            if(item == null) {
                PickUpTile();
                return;
            }
            if(item.onTileMapAction == null) { return; }

            EnergyCost(GetEnergyCost(item.onTileMapAction));

            if (item.onTileMapAction is PlowTile plowTile)
            {
                plowTile.lastMotionVector = characterController2d.lastMotionVector;
            }

            if (item.onTileMapAction is RemovePlowing rakeTile)
            {
                rakeTile.lastMotionVector = characterController2d.lastMotionVector;
            }

            animator.SetTrigger("act");
            bool complete = item.onTileMapAction.OnApplyToTileMap(selectedTilePosition, tileMapReadcontroller, item);

            if(complete == true)
            {
                characterLevel.AddExperience(item.onTileMapAction.skillType, item.onTileMapAction.skillExperienceReward);

                if(item.onItemUsed != null)
                {
                    item.onItemUsed.OnItemUsed(item, GameManager.instance.inventoryContainer);
                    characterLevel.AddExperience(item.onItemUsed.skillType, 100);
                }
            }
        }

        timer = toolTimeOut;
    }

    private int GetEnergyCost(ToolAction action)
    {
        int energyCost = action.energyCost;
        energyCost -= characterLevel.GetLevel(action.skillType);

        if(energyCost < 1)
        {
            energyCost = 1;
        }

        return energyCost;
    }

    private void PickUpTile()
    {
        if(onTilePickUp == null) { return; }

        onTilePickUp.OnApplyToTileMap(selectedTilePosition, tileMapReadcontroller, null);
    }

    public Item GetCurrentItem()
    {
        return toolbarController.GetItem;
    }

}
