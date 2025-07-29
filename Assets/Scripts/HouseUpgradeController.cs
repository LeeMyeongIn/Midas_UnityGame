using UnityEngine;

public class HouseUpgradeController : MonoBehaviour
{
    public static HouseUpgradeController Instance;

    [SerializeField] private GameObject step1House;
    [SerializeField] private GameObject step2House;
    [SerializeField] private GameObject step3House;

    private int currentLevel = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void UpgradeToLevel(int level)
    {
        if (level <= 0)
        {
            Debug.LogWarning($"[경고] 잘못된 집 레벨 {level}, 무시됨");
            return;
        }

        currentLevel = level;
        ApplyLevel();
    }

    private void ApplyLevel()
    {
        Debug.Log($"[집 레벨 적용] 현재 레벨: {currentLevel}");

        step1House?.SetActive(currentLevel == 1);
        step2House?.SetActive(currentLevel == 2);
        step3House?.SetActive(currentLevel == 3);
    }

    private void Start()
    {
        ApplyLevel();
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }
}
