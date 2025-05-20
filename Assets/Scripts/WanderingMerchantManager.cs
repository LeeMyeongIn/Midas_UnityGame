using System.Collections;
using UnityEngine;

public class WanderingMerchantManager : MonoBehaviour
{
    public static WanderingMerchantManager Instance;

    [Header("���� ������Ʈ�� �ð� ����")]
    public GameObject wanderingMerchantObject;
    public float defaultDelay = 5f;
    public float defaultDuration = 300f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Instance�� �ִ� ���� �����Ϸ��� �Ʒ� �ּ� ����
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }

        if (wanderingMerchantObject == null)
        {
            Debug.LogWarning("?? WanderingMerchantManager: wanderingMerchantObject�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }

    public void SummonMerchant(float delay, float duration)
    {
        Debug.Log($" ������ ���� ��ȯ ����: {delay}�� �� ����, {duration}�� ����");
        StartCoroutine(SummonRoutine(delay, duration));
    }

    private IEnumerator SummonRoutine(float delay, float duration)
    {
        yield return new WaitForSeconds(delay);

        if (wanderingMerchantObject != null)
        {
            Debug.Log("������ ���� ����!");
            wanderingMerchantObject.SetActive(true);

            yield return new WaitForSeconds(duration);

            wanderingMerchantObject.SetActive(false);
            Debug.Log("������ ������ �������ϴ�.");
        }
        else
        {
            Debug.LogError("���� ������Ʈ�� null�Դϴ�. ���忡 �����߽��ϴ�.");
        }
    }
}


