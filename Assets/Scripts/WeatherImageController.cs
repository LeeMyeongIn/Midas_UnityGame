using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherImageController : MonoBehaviour
{
    [SerializeField] WeatherManager weatherManager;

    [SerializeField] GameObject clearImage;
    [SerializeField] GameObject rainImage;
    [SerializeField] GameObject heavyRainImage;
    [SerializeField] GameObject rainAndThunderImage;
    [SerializeField] GameObject petalsImage;
    [SerializeField] GameObject leavesImage;
    [SerializeField] GameObject snowImage;

    private void Awake()
    {
        if (weatherManager == null)
            weatherManager = FindObjectOfType<WeatherManager>();
    }

    public void UpdateWeatherImages()
    {
        if (weatherManager == null) return;

        clearImage.SetActive(false);
        rainImage.SetActive(false);
        heavyRainImage.SetActive(false);
        rainAndThunderImage.SetActive(false);
        petalsImage.SetActive(false);
        leavesImage.SetActive(false);
        snowImage.SetActive(false);

        switch (weatherManager.CurrentWeather)
        {
            case WeatherStates.Clear: clearImage.SetActive(true); break;
            case WeatherStates.Rain: rainImage.SetActive(true); break;
            case WeatherStates.HeavyRain: heavyRainImage.SetActive(true); break;
            case WeatherStates.RainAndThunder: rainAndThunderImage.SetActive(true); break;
            case WeatherStates.Petals: petalsImage.SetActive(true); break;
            case WeatherStates.Leaves: leavesImage.SetActive(true); break;
            case WeatherStates.Snow: snowImage.SetActive(true); break;
        }
    }
}
