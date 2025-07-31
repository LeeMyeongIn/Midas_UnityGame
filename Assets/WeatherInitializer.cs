using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherInitializer : MonoBehaviour
{
    void Start()
    {
        if (PlayerPrefs.HasKey("LoadedWeatherState"))
        {
            int weatherInt = PlayerPrefs.GetInt("LoadedWeatherState");
            WeatherStates loadedWeather = (WeatherStates)weatherInt;

            Debug.Log($"[WeatherInitializer] 날씨 불러오기 시작: {loadedWeather}");

            StartCoroutine(SetWeatherWhenReady(loadedWeather));
            PlayerPrefs.DeleteKey("LoadedWeatherState");
        }
    }

    IEnumerator SetWeatherWhenReady(WeatherStates state)
    {
        while (WeatherManager.Instance == null)
        {
            Debug.Log("[WeatherInitializer] WeatherManager 인스턴스를 기다리는 중...");
            yield return null;
        }

        Debug.Log($"[WeatherInitializer] 날씨 적용: {state}");
        WeatherManager.Instance.SetWeather(state);

        yield return null;

        var imageController = FindObjectOfType<WeatherImageController>();
        if (imageController != null)
        {
            Debug.Log("[WeatherInitializer] 이미지 갱신");
            imageController.UpdateWeatherImages();
        }
    }

}