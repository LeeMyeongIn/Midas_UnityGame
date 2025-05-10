using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
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
    const float phaseLenght = 900f;  // 15분 단위
    const float phasesInDay = 96f;  // secondsInDay divided by phaseLength

    [SerializeField] Color nightLightColor;
    [SerializeField] AnimationCurve nightTimeCurve;
    [SerializeField] Color dayLightColor = Color.white;

    float time;
    [SerializeField] float timeScale = 60f;
    [SerializeField] float startAtTime = 21601f;
    [SerializeField] float morningTime = 2160f;

    [SerializeField] ScreenTint screenTint;
    [SerializeField] SeasonTilemapController seasonTilemapController;

    DayOfWeek dayOfWeek;

    [SerializeField] TMPro.TextMeshProUGUI text;
    //[SerializeField] TMPro.TextMeshProUGUI dayOfTheWeekText;
    [SerializeField] TMPro.TextMeshProUGUI seasonText;
    [SerializeField] TMPro.TextMeshProUGUI dateText;
    [SerializeField] Light2D globalLight;
    public int days;
    public int years;
    bool isDayChanging = false;

    Season currentSeason;
    const int seasonLength = 28;    //one month = 28 days

    List<TimeAgent> agents;

    private void Awake()
    {
        agents = new List<TimeAgent>();
    }

    private void Start()
    {
        if (seasonTilemapController == null)
        {
            seasonTilemapController = FindObjectOfType<SeasonTilemapController>();
        }

        time = startAtTime;
        UpdateSeasonText();
        UpdateDateText();
        seasonTilemapController?.UpdateSeason(currentSeason);
    }

    public void Subscribe(TimeAgent timeAgent)
    {
        agents.Add(timeAgent);
    }

    public void Unsubscribe(TimeAgent timeAgent)
    {
        agents.Remove(timeAgent);
    }

    float Hours
    {
        get { return time / 3600f; }
    }

    float Minutes
    {
        get { return time % 3600f / 60f; }
    }

    private void Update()
    {
        if (isDayChanging)
            return;

        time += Time.deltaTime * timeScale;

        TimeValueCalculation();
        DayLight();

        if (!isDayChanging && Hours >= 2f && time >= (morningTime + 72000f)) // 6시부터 8시간 = 2시
        {
            isDayChanging = true;
            StartCoroutine(NextDayRoutine());
            return;
        }

        TimeAgents();
    }

    private IEnumerator NextDayRoutine()
    {
        screenTint.Tint();
        yield return new WaitForSeconds(1.5f);

        time = morningTime;

        days += 1;
        int dayNum = ((int)dayOfWeek + 1) % 7;
        dayOfWeek = (DayOfWeek)dayNum;

        UpdateDateText();

        if (days >= seasonLength)
        {
            NextSeason();
        }

        screenTint.UnTint();
        yield return new WaitForSeconds(1f);
        isDayChanging = false;
    }
    private void TimeValueCalculation()
    {
        int totalHours = (int)Hours % 24;
        int mm = ((int)Minutes / 10) * 10;

        string period = totalHours >= 12 ? "PM" : "AM";

        int hh12 = totalHours % 12;
        if (hh12 == 0) hh12 = 12;

        text.text = $"{hh12:00}:{mm:00} {period}";
    }

    private void DayLight()
    {
        float v = nightTimeCurve.Evaluate(Hours);
        Color c = Color.Lerp(dayLightColor, nightLightColor, v);
        globalLight.color = c;
    }

    int oldPhase = -1;

    private void TimeAgents()
    {
        if (oldPhase == -1)
        {
            oldPhase = CalculatePhase();
        }

        int currentPhase = CalculatePhase();

        while (oldPhase < currentPhase)
        {
            oldPhase += 1;
            for (int i = 0; i < agents.Count; i++)
            {
                agents[i].Invoke(this);
            }
        }
    }

    private int CalculatePhase()
    {
        return (int)(time / phaseLenght) + (int)(days * phasesInDay);
    }

    /*private void NextDay()
    {
        time -= secondsInDay;
        days += 1;

        int dayNum = (int)dayOfWeek;
        dayNum += 1;
        if(dayNum >= 7)
        {
            dayNum = 0;
        }
        dayOfWeek = (DayOfWeek)dayNum;
        //UpdateDayText();
        UpdateDateText();

        if (days >= seasonLength)
        {
            NextSeason();
        }
    }*/

    private void NextSeason()
    {
        days = 0;
        int seasonNum = (int)currentSeason;
        seasonNum += 1;

        if (seasonNum >= 4)
        {
            seasonNum = 0;
            years += 1;
        }

        currentSeason = (Season)seasonNum;
        UpdateSeasonText();
        UpdateDateText();
        seasonTilemapController.UpdateSeason(currentSeason);
    }

    private void UpdateSeasonText()
    {
        string seasonName = currentSeason.ToString().ToUpper();
        seasonText.text = $"{GetOrdinal(years + 1)} YEAR of {seasonName}";
        //seasonText.text = currentSeason.ToString();
    }

    public Season CurrentSeason
    {
        get { return currentSeason; }
    }

    private string GetOrdinal(int number)
    {
        if (number % 100 >= 11 && number % 100 <= 13)
            return number + "th";

        switch (number % 10)
        {
            case 1: return number + "st";
            case 2: return number + "nd";
            case 3: return number + "rd";
            default: return number + "th";
        }
    }

    /*private void UpdateDayText()
    {
        dayOfTheWeekText.text = dayOfWeek.ToString();
    }*/

    private void UpdateDateText()
    {
        int displayDay = days + 1;
        dateText.text = $"Day: {displayDay}, {dayOfWeek}";
    }

    public void SkipTime(float seconds = 0, float minute = 0, float hours = 0)
    {
        float timeToSkip = seconds;
        timeToSkip += minute * 60f;
        timeToSkip += hours * 3600f;

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
}
