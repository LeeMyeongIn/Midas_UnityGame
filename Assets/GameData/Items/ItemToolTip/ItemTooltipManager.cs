using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemTooltipManager : MonoBehaviour
{
    public static ItemTooltipManager Instance;

    public GameObject tooltipPanel;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI priceText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        HideTooltip();
    }

    /*private void Update()
    {
        if (tooltipPanel.activeSelf)
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 offset = new Vector2(20, -20); // 마우스 기준 오른쪽 위
            tooltipPanel.transform.position = mousePos + offset;
        }
    }*/

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




    public void ShowTooltip(Item item)
    {
        tooltipPanel.SetActive(true);
        itemNameText.text = item.Name;
        descriptionText.text = item.description;
        priceText.text = item.canBeSold
            ? $"Buy:{item.price*1.5f}G\nSell:{item.price*0.5f}G" 
              : "Can't sell";
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
}
