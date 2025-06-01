using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public enum TransitionType
{
    Warp,
    Scene
}

public class Transition : MonoBehaviour
{
    [Header("Transition Settings")]
    [SerializeField] private TransitionType transitionType;
    [SerializeField] private string sceneNameToTransition;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private Collider2D confiner;
    [SerializeField] private Transform destination;

    [Header("Optional")]
    [SerializeField] private bool requireConfirmation = false; // true일 때만 EndingPanel 사용

    private CameraConfiner cameraConfiner;

    void Start()
    {
        if (confiner != null)
        {
            cameraConfiner = FindObjectOfType<CameraConfiner>();
        }
    }

    internal void InitiateTransition(Transform toTransition)
    {
        if (requireConfirmation)
        {
            if (EndingPanel.Instance != null)
            {
                EndingPanel.Instance.ShowConfirmation(() =>
                {
                    ProceedTransition(toTransition);
                });
            }
            else
            {
                Debug.LogWarning("[Transition] EndingPanel.Instance가 null입니다. 확인창 없이 이동이 중단됩니다.");
            }
        }
        else
        {
            ProceedTransition(toTransition);
        }
    }

    private void ProceedTransition(Transform toTransition)
    {
        switch (transitionType)
        {
            case TransitionType.Warp:
                var currentCamera = Camera.main?.GetComponent<Cinemachine.CinemachineBrain>();

                if (currentCamera == null)
                {
                    Debug.LogError("[Transition] CinemachineBrain을 찾을 수 없습니다.");
                    return;
                }

                if (destination == null)
                {
                    Debug.LogError("[Transition] destination이 설정되지 않았습니다.");
                    return;
                }

                if (cameraConfiner != null && confiner != null)
                {
                    cameraConfiner.UpdateBounds(confiner);
                }

                currentCamera.ActiveVirtualCamera?.OnTargetObjectWarped(
                    toTransition,
                    destination.position - toTransition.position
                );

                toTransition.position = new Vector3(
                    destination.position.x,
                    destination.position.y,
                    toTransition.position.z
                );
                break;

            case TransitionType.Scene:
                if (string.IsNullOrEmpty(sceneNameToTransition))
                {
                    Debug.LogError("[Transition] Scene 전환 이름이 비어 있습니다.");
                    return;
                }

                GameSceneManager.instance.InitSwitchScene(sceneNameToTransition, targetPosition);
                break;
        }
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (transitionType == TransitionType.Scene)
        {
            Handles.Label(transform.position, "to " + sceneNameToTransition);
        }

        if (transitionType == TransitionType.Warp && destination != null)
        {
            Gizmos.DrawLine(transform.position, destination.position);
        }
#endif
    }
}
