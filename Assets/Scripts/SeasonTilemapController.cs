using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
public class SeasonTilemapController : MonoBehaviour
{
    [Header("Season Tilemap")]
    [SerializeField] Tilemap springTilemap;
    [SerializeField] Tilemap summerTilemap;
    [SerializeField] Tilemap fallTilemap;
    [SerializeField] Tilemap winterTilemap;

    [Header("Season House")]
    [SerializeField] GameObject SpringSummerFall;
    [SerializeField] GameObject Winter;

    public void UpdateSeason(Season season)
    {
        springTilemap.gameObject.SetActive(season == Season.Spring);
        summerTilemap.gameObject.SetActive(season == Season.Summer);
        fallTilemap.gameObject.SetActive(season == Season.Fall);
        winterTilemap.gameObject.SetActive(season == Season.Winter);

        bool isWinter = (season == Season.Winter);
        SpringSummerFall.SetActive(!isWinter);
        Winter.SetActive(isWinter);
    }
}
