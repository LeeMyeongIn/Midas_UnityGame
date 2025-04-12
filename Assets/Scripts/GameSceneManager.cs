using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager instance;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] ScreenTint screenTint;
    [SerializeField] CameraConfiner cameraConfiner;
    string currentScene;
    AsyncOperation unload;
    AsyncOperation load;

    // Start is called before the first frame update
    void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
    }

    public void InitSwitchScene(string to, Vector3 targetPosition)
    {
        StartCoroutine(Transition(to, targetPosition));
    }

    IEnumerator Transition(string to, Vector3 targetPosition)
    {
        screenTint.Tint();

        yield return new WaitForSeconds(1f / screenTint.speed + 0.1f);  // 1초를 틴팅 속도로 나눈 값 + 약간의 시간 오프셋 (틴트 애니메이션 재생 속도)
        SwitchScene(to, targetPosition);

        while(load != null & unload != null)
        {
            if (load.isDone) { load = null; }
            if (unload.isDone) { unload = null; }
            yield return new WaitForSeconds(0.1f);
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(currentScene));

        cameraConfiner.UpdateBounds();
        screenTint.UnTint();
    }

    public void SwitchScene(string to, Vector3 targetPosition)
    {
        load = SceneManager.LoadSceneAsync(to, LoadSceneMode.Additive);
        unload = SceneManager.UnloadSceneAsync(currentScene);
        currentScene = to;

        Transform playerTransform = GameManager.instance.player.transform;

        CinemachineBrain currentCamera = Camera.main.GetComponent<CinemachineBrain>();

        currentCamera.ActiveVirtualCamera.OnTargetObjectWarped(
            playerTransform,
            targetPosition - playerTransform.position
            );

        playerTransform.position = new Vector3(
            targetPosition.x,
            targetPosition.y,
            playerTransform.position.z
            );
    }
}
