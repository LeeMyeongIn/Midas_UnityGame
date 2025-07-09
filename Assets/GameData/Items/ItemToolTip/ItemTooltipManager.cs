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

            // ĵ���� ���� ���� ũ�� ��������
            RectTransform canvasRect = tooltipPanel.transform.root.GetComponent<Canvas>().GetComponent<RectTransform>();
            RectTransform tooltipRect = tooltipPanel.GetComponent<RectTransform>();

            // ������ ũ��� ��ġ�� ���
            Vector2 size = tooltipRect.sizeDelta * tooltipPanel.transform.lossyScale;
            Vector2 clampedPos = targetPos;

            // ���� ��� Ȯ��
            if (targetPos.x + size.x > Screen.width)
                clampedPos.x = Screen.width - size.x;
            // ���� ��� Ȯ��
            if (targetPos.x < 0)
                clampedPos.x = 0;
            // ��� ��� Ȯ��
            if (targetPos.y > Screen.height)
                clampedPos.y = Screen.height - size.y;
            // �ϴ� ��� Ȯ��
            if (targetPos.y - size.y < 0)
                clampedPos.y = size.y;

            tooltipPanel.transform.position = clampedPos;
        }
        if (!tooltipPanel.activeSelf) //���׼���
        {
            HideTooltip();
        }
    }

    // ���� �޼��� - ���� Ȱ��ȭ�� Trading���� ���� ���� ������ �����ͼ� ���
    public void ShowTooltip(Item item)
    {
        // Trading ������Ʈ���� ���� ���� ���� ��������
        Trading trading = FindObjectOfType<Trading>();
        if (trading != null && trading.IsCurrentlyTrading())
        {
            ShowTooltip(item, trading.GetCurrentSellToPlayerMultip(), trading.GetCurrentBuyFromPlayerMultip());
        }
        else
        {
            ShowTooltip(item, 1.5f, 0.5f); // �⺻ ���� ���
        }
    }

    // ���ο� �޼��� - ���κ� ������ �޾Ƽ� ó��
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
