using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Crop", menuName = "Crop")]
public class CropTileSet : ScriptableObject
{
    public string cropName;
    public List<int> growthStageTime; // ���� �ܰ躰 �ð�
    public List<Sprite> sprites;      // �� ���� �ܰ��� ��������Ʈ
    public int timeToGrow;            // ��ü ���� �ð�
    public Item yield;                // ��Ȯ �� ������ ������
    public int count;                 // ��Ȯ �� ������ ����
    public List<Season> seasons;      // �۹��� �ڶ� �� �ִ� ���� ���
}
