using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public GameObject target;  // 따라갈 타겟 오브젝트
    public float height = 10f; // 고정된 y 높이
    public float distance = 10f; // 타겟과 카메라 사이의 거리
    public float smoothSpeed = 0.125f; // 카메라 이동의 부드러운 속도 조정

    private Vector3 offset; // 타겟에 대한 초기 오프셋

    void Start()
    {
        // 카메라와 타겟 사이의 초기 오프셋 설정 (x, z 거리와 y 높이를 고정)
        offset = new Vector3(0, height, distance);
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // 타겟의 위치에 따라 카메라 위치를 업데이트 (y 좌표는 고정)
            Vector3 desiredPosition = target.transform.position + offset;
            desiredPosition.y = height; // y 좌표를 고정된 값으로 설정

            // 카메라가 부드럽게 이동하도록 보간
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            // 카메라가 항상 타겟을 바라보도록 설정
            transform.LookAt(target.transform);
        }
    }
}
