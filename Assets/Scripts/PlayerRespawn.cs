using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] Vector3 respawnPointPosition;  // 집 위치
    [SerializeField] string respawnPointScene;  // 집 씬 이름 (HomeScene을 예시로 설정)

    internal void StartRespawn()
    {
        // 리스폰 위치로 이동
        transform.position = respawnPointPosition;

        // 리스폰 시 카메라 이동 제한 업데이트
        UpdateCameraBounds();
    }

    private void UpdateCameraBounds()
    {
        // CameraConfiner를 찾아서 카메라의 이동 범위를 업데이트
        CameraConfiner cameraConfiner = FindObjectOfType<CameraConfiner>();

        if (cameraConfiner != null)
        {
            // 집 위치에 맞는 Bounding Box가 있을 경우, 이를 카메라의 범위로 설정
            Collider2D confinerCollider = GameObject.Find("CameraConfinerHouse").GetComponent<Collider2D>();
            cameraConfiner.UpdateBounds(confinerCollider);
        }
    }
}

