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
        if (timer > 0f) { timer -= Time.deltaTime; }

        if (Input.GetMouseButtonDown(0))
        {
            WeaponAction();
        }

        SelectTile();
        CanSelectCheck();
        Marker();

        if (Input.GetMouseButtonDown(0))
        {
            if (UseToolWorld()) return;
            UseToolGrid();
        }
    }

    private void WeaponAction()
    {
        if (timer > 0f) { return; }

        Item item = toolbarController.GetItem;
        if (item == null || item.isWeapon == false) return;

        EnergyCost(weaponEnergyCost);

        if (item.id == 10) offsetDistance = 1.3f;       // SwordL1
        else if (item.id == 11) offsetDistance = 1.5f;  // SwordL2
        else if (item.id == 12) offsetDistance = 1.7f;  // SwordL3
        else offsetDistance = 1.3f;

        animator.SetTrigger("act");

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
        if (timer > 0f)
        {
            Debug.Log("[DEBUG] UseToolWorld: Timer remains, skipping.");
            return false;
        }

        Vector2 position = rgbd2d.position + characterController2d.lastMotionVector * offsetDistance;

        Item item = toolbarController.GetItem;
        if (item == null)
        {
            Debug.Log("[DEBUG] UseToolWorld: Item is null, skipping.");
            return false;
        }

        if (item.onTileMapAction != null)
        {
            Debug.Log($"[DEBUG] UseToolWorld: TileMapAction Item ({item.name})이므로 false 반환. UseToolGrid()로 이동.");
            return false;
        }

        bool complete = false;

        if (item.onAction != null)
        {
            Debug.Log($"[DEBUG] UseToolWorld: WorldAction({item.onAction.name}) 실행 시도.");
            EnergyCost(GetEnergyCost(item.onAction));

            if (item.onAction is RemovePlowing rakeTile)
                rakeTile.lastMotionVector = characterController2d.lastMotionVector;

            animator.SetTrigger("act");
            complete = item.onAction.OnApply(position);

            if (complete && item.onItemUsed != null)
                item.onItemUsed.OnItemUsed(item, GameManager.instance.inventoryContainer);
        }
        else if (item.onItemUsed != null)
        {
            // 도감 아이템 전용 처리 (TileMapAction이 없는 경우에만 실행)
            Debug.Log($"[DEBUG] UseToolWorld: ItemUsed({item.onItemUsed.name}) 실행 시도.");
            animator.SetTrigger("act");
            item.onItemUsed.OnItemUsed(item, GameManager.instance.inventoryContainer);
            complete = true;
        }

        if (complete && item.onItemUsed != null)
        {
            characterLevel.AddExperience(item.onItemUsed.skillType, 100);
        }

        Debug.Log($"[DEBUG] UseToolWorld 최종 반환: {complete}. UseToolGrid() 호출 여부: {!complete}");

        timer = toolTimeOut;
        return complete;
    }

    private void UseToolGrid()
    {
        if (timer > 0f) return;

        if (selectable == true)
        {
            Item item = toolbarController.GetItem;
            if (item == null)
            {
                PickUpTile();
                return;
            }

            if (item.onTileMapAction == null) return;

            EnergyCost(GetEnergyCost(item.onTileMapAction));

            if (item.onTileMapAction is PlowTile plowTile)
                plowTile.lastMotionVector = characterController2d.lastMotionVector;

            if (item.onTileMapAction is RemovePlowing rakeTile)
                rakeTile.lastMotionVector = characterController2d.lastMotionVector;

            animator.SetTrigger("act");
            bool complete = item.onTileMapAction.OnApplyToTileMap(selectedTilePosition, tileMapReadcontroller, item);

            if (complete)
            {
                characterLevel.AddExperience(item.onTileMapAction.skillType, item.onTileMapAction.skillExperienceReward);

                if (item.onItemUsed != null)
                {
                    item.onItemUsed.OnItemUsed(item, GameManager.instance.inventoryContainer);
                    characterLevel.AddExperience(item.onItemUsed.skillType, 100);
                }
            }

            timer = toolTimeOut;
        }
    }

    private int GetEnergyCost(ToolAction action)
    {
        int energyCost = action.energyCost;
        energyCost -= characterLevel.GetLevel(action.skillType);
        return Mathf.Max(1, energyCost);
    }

    private void PickUpTile()
    {
        if (onTilePickUp == null) return;
        onTilePickUp.OnApplyToTileMap(selectedTilePosition, tileMapReadcontroller, null);
    }

    public Item GetCurrentItem()
    {
        return toolbarController.GetItem;
    }
}
