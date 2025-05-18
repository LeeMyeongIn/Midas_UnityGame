using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Crop", menuName = "Crop")]
public class CropTileSet : ScriptableObject
{
    public string cropName;
    public List<int> growthStageTime; // 성장 단계별 시간
    public List<Sprite> sprites;      // 각 성장 단계의 스프라이트
    public int timeToGrow;            // 전체 성장 시간
    public Item yield;                // 수확 시 나오는 아이템
    public int count;                 // 수확 시 나오는 수량
    public List<Season> seasons;      // 작물이 자랄 수 있는 계절 목록
}
