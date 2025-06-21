using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum DayOfWeek
{
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
    Saturday,
    Sunday
}
public enum Season
{
    Spring,
    Summer,
    Fall,
    Winter
}

public class DayTimeController : MonoBehaviour
{
    const float secondsInDay = 86400f;
    const float phaseLenght = 900f;
    const float phasesInDay = 96f;

    [SerializeField] Color nightLightColor;
    [SerializeField] AnimationCurve nightTimeCurve;
    [SerializeField] Color dayLightColor = Color.white;

    float time;
    [SerializeField] float timeScale = 60f;
    [SerializeField] float startAtTime = 21601f;
    [SerializeField] float morningTime = 2160f;

    [SerializeField] ScreenTint screenTint;
    [SerializeField] SeasonTilemapController seasonTilemapController;
    public WeatherManager weatherManager;
    [SerializeField] WeatherImageController weatherImageController;
    [SerializeField] SeasonImageController seasonImageController;

    DayOfWeek dayOfWeek;

    [SerializeField] TMPro.TextMeshProUGUI timeText;
    [SerializeField] TMPro.TextMeshProUGUI yearText;
    [SerializeField] TMPro.TextMeshProUGUI seasonText;
    [SerializeField] TMPro.TextMeshProUGUI dateText;
    [SerializeField] TMPro.TextMeshProUGUI weatherText;
    [SerializeField] Light2D globalLight;

    public int days;
    public int years;
    public int totalDays;
    bool isDayChanging = false;

    Season currentSeason;
    public Season CurrentSeason => currentSeason;

    const int seasonLength = 2;

    List<TimeAgent> agents;
    int oldPhase = -1;

    private void Awake()
    {
        agents = new List<TimeAgent>();
        Sleep sleep = GetComponent<Sleep>();
    }

    private void Start()
    {
        if (time == 0f)
            time = startAtTime;

        if (seasonTilemapController == null)
            seasonTilemapController = FindObjectOfType<SeasonTilemapController>();

        UpdateSeasonText();
        UpdateDateText();
        UpdateYearText();
        seasonTilemapController?.UpdateSeason(currentSeason);
        seasonImageController?.UpdateSeasonImage(currentSeason);

        if (weatherManager != null)
        {
            weatherManager.GenerateDailyWeather(currentSeason);
            UpdateWeatherText();
            weatherImageController.UpdateWeatherImages();
        }
    }

    public void Subscribe(TimeAgent timeAgent) => agents.Add(timeAgent);
    public void Unsubscribe(TimeAgent timeAgent) => agents.Remove(timeAgent);

    float Hours => time / 3600f;
    float Minutes => time % 3600f / 60f;

    private void Update()
    {
        if (isDayChanging) return;

        time += Time.deltaTime * timeScale;

        TimeValueCalculation();
        DayLight();

        if (!isDayChanging && Hours >= 2f && time >= (morningTime + 72000f))
        {
            isDayChanging = true;

            Sleep sleep = FindObjectOfType<Sleep>();
            if (sleep != null)
                sleep.DoSleep();

            StartCoroutine(NextDayRoutine());
            return;
        }

        TimeAgents();
    }

    private IEnumerator NextDayRoutine()
    {
        Sleep sleep = FindObjectOfType<Sleep>();
        if (sleep != null)
            sleep.DoSleep();

        yield return new WaitForSeconds(1.5f);

        time = morningTime;
        days += 1;
        totalDays += 1;
        oldPhase = -1;

        int dayNum = ((int)dayOfWeek + 1) % 7;
        dayOfWeek = (DayOfWeek)dayNum;

        UpdateDateText();

        if (weatherManager != null)
        {
            weatherManager.GenerateDailyWeather(currentSeason);
            UpdateWeatherText();
        }

        if (days >= seasonLength)
        {
            NextSeason();
        }

        yield return new WaitForSeconds(1f);

        CheckEndingCondition();

        isDayChanging = false;
    }

    private void CheckEndingCondition()
    {
        // 4³âÂ÷ º½ 1ÀÏ
        if (years == 3 && currentSeason == Season.Spring && days == 0)
        {
            bool conditionMet = CheckMyGameClearCondition();

            if (conditionMet)
            {
                SceneManager.LoadScene("HappyEndingScene");
            }
            else
            {
                SceneManager.LoadScene("BadEndingScene");
            }
        }
    }

    private bool CheckMyGameClearCondition()
    {
        return EndingConditionChecker.Instance != null && EndingConditionChecker.Instance.IsEndingAvailable();
    }

    private void UpdateWeatherText()
    {
        if (weatherManager != null && weatherText != null)
        {
            weatherText.text = $"{weatherManager.CurrentWeatherText}";
        }
    }

    private void TimeValueCalculation()
    {
        int totalHours = (int)Hours % 24;
        int mm = ((int)Minutes / 10) * 10;

        string period = totalHours >= 12 ? "PM" : "AM";

        int hh12 = totalHours % 12;
        if (hh12 == 0) hh12 = 12;

        timeText.text = $"{hh12:00}:{mm:00} {period}";
    }

    private void DayLight()
    {
        float v = nightTimeCurve.Evaluate(Hours);
        Color c = Color.Lerp(dayLightColor, nightLightColor, v);
        globalLight.color = c;
    }

    private void TimeAgents()
    {
        if (oldPhase == -1)
            oldPhase = CalculatePhase();

        int currentPhase = CalculatePhase();

        while (oldPhase < currentPhase)
        {
            oldPhase += 1;
            foreach (var agent in agents)
            {
                agent.Invoke(this);
            }
        }
    }

    private int CalculatePhase()
    {
        return (int)(time / phaseLenght) + (int)(days * phasesInDay);
    }

    private void NextSeason()
    {
        days = 0;
        int seasonNum = (int)currentSeason + 1;

        if (seasonNum >= 4)
        {
            seasonNum = 0;
            years += 1;
            UpdateYearText();
        }

        currentSeason = (Season)seasonNum;
        UpdateSeasonText();
        UpdateDateText();
        seasonTilemapController?.UpdateSeason(currentSeason);
        seasonImageController?.UpdateSeasonImage(currentSeason);
    }

    private void UpdateSeasonText()
    {
        string seasonName = currentSeason.ToString().ToUpper();
        seasonText.text = $"{seasonName}";
    }

    private void UpdateDateText()
    {
        int displayDay = days + 1;
        dateText.text = $"{displayDay}, {dayOfWeek}";
    }

    private void UpdateYearText()
    {
        if (yearText != null)
        {
            yearText.text = $"Year: {years + 1}";
        }
    }

    public void SkipTime(float seconds = 0, float minute = 0, float hours = 0)
    {
        float timeToSkip = seconds + minute * 60f + hours * 3600f;
        time += timeToSkip;
    }

    public void SkipToMorning()
    {
        float secondsToSkip = 0f;
        if (time > morningTime)
        {
            secondsToSkip += secondsInDay - time + morningTime;
        }
        else
        {
            secondsToSkip += morningTime - time;
        }

        SkipTime(secondsToSkip);
    }

    public void SetSeason(int seasonIndex)
    {
        currentSeason = (Season)seasonIndex;
        seasonTilemapController?.UpdateSeason(currentSeason);
        seasonImageController?.UpdateSeasonImage(currentSeason);
        UpdateSeasonText();
        UpdateDateText();
    }

    public void SetTime(float timeValue)
    {
        time = timeValue;
        TimeValueCalculation();
        DayLight();
    }

    public void SetDate(int savedYear, int savedDay)
    {
        years = savedYear;
        days = savedDay;
        UpdateDateText();
        UpdateYearText();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "HappyEndingScene" || scene.name == "BadEndingScene")
        {
            Destroy(gameObject);
        }
    }
}
