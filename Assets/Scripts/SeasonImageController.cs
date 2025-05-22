using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonImageController : MonoBehaviour
{
    [SerializeField] GameObject springImage;
    [SerializeField] GameObject summerImage;
    [SerializeField] GameObject fallImage;
    [SerializeField] GameObject winterImage;

    public void UpdateSeasonImage(Season currentSeason)
    {
        springImage.SetActive(false);
        summerImage.SetActive(false);
        fallImage.SetActive(false);
        winterImage.SetActive(false);

        switch (currentSeason)
        {
            case Season.Spring:
                springImage.SetActive(true);
                break;
            case Season.Summer:
                summerImage.SetActive(true);
                break;
            case Season.Fall:
                fallImage.SetActive(true);
                break;
            case Season.Winter:
                winterImage.SetActive(true);
                break;
        }
    }
}
