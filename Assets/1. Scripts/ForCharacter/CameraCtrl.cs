using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    private Transform playerTransform;
    private Vector3 target;

    private Transform cameraTransform;

    [SerializeField]
    private bool followOnStart = false;

    bool isFollowing;

    void Start()
    {
        if (followOnStart)
        {
            OnStartFollowing();
        }
    }

    void LateUpdate()
    {
        if(cameraTransform == null && isFollowing)
        {
            OnStartFollowing();
        }

        if (isFollowing)
        {
            Apply();
        }
    }

    public void OnStartFollowing()
    {
        cameraTransform = Camera.main.transform;
        playerTransform = GetComponent<Transform>();

        target.z = cameraTransform.position.z;

        isFollowing = true;

        Apply();
    }

    void Apply()
    {
        /*
         * 카메라 이동
         * 카메라가 맵안을 보여주고 있다면 카메라 이동 가능
         * 카메라가 비추는 범위가 맵밖을 포함하지 않도록 함 -> 굳이 필요한가?
         */
        if(playerTransform.position.y < 7 && playerTransform.position.y > -7)
        {
            target.y = playerTransform.position.y;
        }
        else if(playerTransform.position.y > 7)
        {
            target.y = 7;
        }
        else if (playerTransform.position.y < -7)
        {
            target.y = -7;
        }

        if (playerTransform.position.x < 8 && playerTransform.position.x > -8)
        {
            target.x = playerTransform.position.x;            
        }
        else if (playerTransform.position.x > 8)
        {
            target.x = 8;
        }
        else if (playerTransform.position.x < -8)
        {
            target.x = -8;
        }

        cameraTransform.position = target;
    }
}
