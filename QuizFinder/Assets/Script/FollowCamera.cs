using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public GameObject target;  // ���� Ÿ�� ������Ʈ
    public float height = 10f; // ������ y ����
    public float distance = 10f; // Ÿ�ٰ� ī�޶� ������ �Ÿ�
    public float smoothSpeed = 0.125f; // ī�޶� �̵��� �ε巯�� �ӵ� ����

    private Vector3 offset; // Ÿ�ٿ� ���� �ʱ� ������

    void Start()
    {
        // ī�޶�� Ÿ�� ������ �ʱ� ������ ���� (x, z �Ÿ��� y ���̸� ����)
        offset = new Vector3(0, height, distance);
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // Ÿ���� ��ġ�� ���� ī�޶� ��ġ�� ������Ʈ (y ��ǥ�� ����)
            Vector3 desiredPosition = target.transform.position + offset;
            desiredPosition.y = height; // y ��ǥ�� ������ ������ ����

            // ī�޶� �ε巴�� �̵��ϵ��� ����
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            // ī�޶� �׻� Ÿ���� �ٶ󺸵��� ����
            transform.LookAt(target.transform);
        }
    }
}
