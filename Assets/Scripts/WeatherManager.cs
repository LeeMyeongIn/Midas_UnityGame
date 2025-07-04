using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeatherChances
{
    public float clear = 1f;
    public float rain = 0f;
    public float heavyRain = 0f;
    public float rainAndThunder = 0f;
    public float snow = 0f;
    public float petals = 0f;
    public float leaves = 0f;

    public WeatherChances(float clear, float rain, float heavyRain, float rainAndThunder, float snow, float petals, float leaves)
    {
        this.clear = clear;
        this.rain = rain;
        this.heavyRain = heavyRain;
        this.rainAndThunder = rainAndThunder;
        this.snow = snow;
        this.petals = petals;
        this.leaves = leaves;
    }
}

public enum WeatherStates
{
    Clear,
    Rain,
    HeavyRain,
    RainAndThunder,
    Snow,
    Petals,
    Leaves
}

public class WeatherManager : MonoBehaviour
{
    [Header("Particle Systems")]
    [SerializeField] ParticleSystem rainObject;
    [SerializeField] ParticleSystem heavyRainObject;
    [SerializeField] ParticleSystem rainAndThunder;
    [SerializeField] ParticleSystem snowObject;
    [SerializeField] ParticleSystem petalsObject;
    [SerializeField] ParticleSystem leavesObject;

    [Header("Season Weather Chances")]
    public WeatherChances springChances = new WeatherChances(0.5f, 0.2f, 0f, 0f, 0f, 0.3f, 0f);
    public WeatherChances summerChances = new WeatherChances(0.4f, 0.3f, 0.2f, 0.1f, 0f, 0f, 0f);
    public WeatherChances fallChances = new WeatherChances(0.6f, 0.1f, 0f, 0f, 0f, 0f, 0.3f);
    public WeatherChances winterChances = new WeatherChances(0.5f, 0f, 0f, 0f, 0.5f, 0f, 0f);

    private WeatherStates currentWeatherState;
    public WeatherStates CurrentWeather => currentWeatherState;

    public string CurrentWeatherText => currentWeatherState.ToString();


    private void Start()      //test
    {
        //ApplyWeather(WeatherStates.Snow);
        GenerateDailyWeather(Season.Spring);
    }

    public void GenerateDailyWeather(Season currentSeason)
    {
        WeatherStates generatedWeather = GenerateWeatherBasedOnSeason(currentSeason);
        ApplyWeather(generatedWeather);
    }

    private WeatherStates GenerateWeatherBasedOnSeason(Season season)
    {
        WeatherChances chances = season switch
        {
            Season.Spring => springChances,
            Season.Summer => summerChances,
            Season.Fall => fallChances,
            Season.Winter => winterChances,
            _ => throw new ArgumentOutOfRangeException()
        };

        float total = chances.clear + chances.rain + chances.heavyRain +
                      chances.rainAndThunder + chances.snow +
                      chances.petals + chances.leaves;

        float r = UnityEngine.Random.value * total;

        if (r < chances.clear) return WeatherStates.Clear;
        r -= chances.clear;

        if (r < chances.rain) return WeatherStates.Rain;
        r -= chances.rain;

        if (r < chances.heavyRain) return WeatherStates.HeavyRain;
        r -= chances.heavyRain;

        if (r < chances.rainAndThunder) return WeatherStates.RainAndThunder;
        r -= chances.rainAndThunder;

        if (r < chances.snow) return WeatherStates.Snow;
        r -= chances.snow;

        if (r < chances.petals) return WeatherStates.Petals;
        r -= chances.petals;

        return WeatherStates.Leaves;
    }

    public event Action<WeatherStates> OnWeatherChanged;
    private void ApplyWeather(WeatherStates state)
    {
        currentWeatherState = state;
        OnWeatherChanged?.Invoke(state); // 이벤트 호출

        rainObject?.gameObject.SetActive(state == WeatherStates.Rain);
        heavyRainObject?.gameObject.SetActive(state == WeatherStates.HeavyRain);
        rainAndThunder?.gameObject.SetActive(state == WeatherStates.RainAndThunder);
        snowObject?.gameObject.SetActive(state == WeatherStates.Snow);
        petalsObject?.gameObject.SetActive(state == WeatherStates.Petals);
        leavesObject?.gameObject.SetActive(state == WeatherStates.Leaves);
    }

    public bool IsRaining =>
        currentWeatherState == WeatherStates.Rain ||
        currentWeatherState == WeatherStates.HeavyRain ||
        currentWeatherState == WeatherStates.RainAndThunder;
}
