using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] Vector3 respawnPointPosition;  // �� ��ġ
    [SerializeField] string respawnPointScene;  // �� �� �̸� (HomeScene�� ���÷� ����)

    internal void StartRespawn()
    {
        // ������ ��ġ�� �̵�
        transform.position = respawnPointPosition;

        // ������ �� ī�޶� �̵� ���� ������Ʈ
        UpdateCameraBounds();
    }

    private void UpdateCameraBounds()
    {
        // CameraConfiner�� ã�Ƽ� ī�޶��� �̵� ������ ������Ʈ
        CameraConfiner cameraConfiner = FindObjectOfType<CameraConfiner>();

        if (cameraConfiner != null)
        {
            // �� ��ġ�� �´� Bounding Box�� ���� ���, �̸� ī�޶��� ������ ����
            Collider2D confinerCollider = GameObject.Find("CameraConfinerHouse").GetComponent<Collider2D>();
            cameraConfiner.UpdateBounds(confinerCollider);
        }
    }
}

