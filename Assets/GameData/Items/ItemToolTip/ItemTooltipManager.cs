using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltipManager : MonoBehaviour
{
    public static ItemTooltipManager Instance;
    public GameObject tooltipPanel;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI priceText;
    [Header("Item Icon")]
    public Image itemIconImage;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        HideTooltip();
    }

    void Update()
    {
        if (tooltipPanel.activeSelf)
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 offset = new Vector2(10, -10);
            Vector2 targetPos = mousePos + offset;

            // 캔버스 기준 툴팁 크기 가져오기
            RectTransform canvasRect = tooltipPanel.transform.root.GetComponent<Canvas>().GetComponent<RectTransform>();
            RectTransform tooltipRect = tooltipPanel.GetComponent<RectTransform>();

            // 툴팁의 크기와 위치를 계산
            Vector2 size = tooltipRect.sizeDelta * tooltipPanel.transform.lossyScale;
            Vector2 clampedPos = targetPos;

            // 우측 경계 확인
            if (targetPos.x + size.x > Screen.width)
                clampedPos.x = Screen.width - size.x;
            // 좌측 경계 확인
            if (targetPos.x < 0)
                clampedPos.x = 0;
            // 상단 경계 확인
            if (targetPos.y > Screen.height)
                clampedPos.y = Screen.height - size.y;
            // 하단 경계 확인
            if (targetPos.y - size.y < 0)
                clampedPos.y = size.y;

            tooltipPanel.transform.position = clampedPos;
        }
        if (!tooltipPanel.activeSelf) //버그수정
        {
            HideTooltip();
        }
    }

    // 기존 메서드 - 현재 활성화된 Trading에서 상인 배율 정보를 가져와서 사용
    public void ShowTooltip(Item item)
    {
        // Trading 컴포넌트에서 현재 상인 정보 가져오기
        Trading trading = FindObjectOfType<Trading>();
        if (trading != null && trading.IsCurrentlyTrading())
        {
            ShowTooltip(item, trading.GetCurrentSellToPlayerMultip(), trading.GetCurrentBuyFromPlayerMultip());
        }
        else
        {
            ShowTooltip(item, 1.5f, 0.5f); // 기본 배율 사용
        }
    }

    // 새로운 메서드 - 상인별 배율을 받아서 처리
    public void ShowTooltip(Item item, float sellToPlayerMultip, float buyFromPlayerMultip)
    {
        tooltipPanel.SetActive(true);

        if (itemNameText != null)
            itemNameText.text = item.name;
        if (descriptionText != null)
            descriptionText.text = item.description;
        if (priceText != null)
            priceText.text = item.canBeSold
                ? $"Buy:{item.price * sellToPlayerMultip}  <sprite name=Coin2> \nSell:{item.price * buyFromPlayerMultip}  <sprite name=Coin2>"
                  : "Can't sell";

        UpdateItemIcon(item);
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
        if (itemIconImage != null)
        {
            itemIconImage.sprite = null;
            itemIconImage.enabled = false;
        }
    }

    private void UpdateItemIcon(Item item)
    {
        if (itemIconImage != null && item != null)
        {
            if (item.icon != null)
            {
                itemIconImage.sprite = item.icon;
                itemIconImage.enabled = true;
            }
        }
    }
}
