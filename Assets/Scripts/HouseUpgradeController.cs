using UnityEngine;

public class HouseUpgradeController : MonoBehaviour
{
    [SerializeField] private GameObject step1House;
    [SerializeField] private GameObject step2House;
    [SerializeField] private GameObject step3House;

    private int currentLevel = 1;

    public void UpgradeToLevel(int level)
    {
        currentLevel = level;
        ApplyLevel();
    }

    private void ApplyLevel()
    {
        step1House?.SetActive(currentLevel == 1);
        step2House?.SetActive(currentLevel == 2);
        step3House?.SetActive(currentLevel == 3);
    }

    private void Start()
    {
        ApplyLevel();
    }
}
