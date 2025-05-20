using System.Collections;
using UnityEngine;

public class WanderingMerchantManager : MonoBehaviour
{
    public static WanderingMerchantManager Instance;

    [Header("상인 오브젝트와 시간 설정")]
    public GameObject wanderingMerchantObject;
    public float defaultDelay = 5f;
    public float defaultDuration = 300f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Instance가 있는 동안 유지하려면 아래 주석 해제
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }

        if (wanderingMerchantObject == null)
        {
            Debug.LogWarning("?? WanderingMerchantManager: wanderingMerchantObject가 할당되지 않았습니다.");
        }
    }

    public void SummonMerchant(float delay, float duration)
    {
        Debug.Log($" 떠돌이 상인 소환 예약: {delay}초 후 등장, {duration}초 유지");
        StartCoroutine(SummonRoutine(delay, duration));
    }

    private IEnumerator SummonRoutine(float delay, float duration)
    {
        yield return new WaitForSeconds(delay);

        if (wanderingMerchantObject != null)
        {
            Debug.Log("떠돌이 상인 등장!");
            wanderingMerchantObject.SetActive(true);

            yield return new WaitForSeconds(duration);

            wanderingMerchantObject.SetActive(false);
            Debug.Log("떠돌이 상인이 떠났습니다.");
        }
        else
        {
            Debug.LogError("상인 오브젝트가 null입니다. 등장에 실패했습니다.");
        }
    }
}


