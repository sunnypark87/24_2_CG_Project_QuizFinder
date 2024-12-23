using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject target;
    public Vector3 character;
    public Vector3 spawnCenter = new Vector3(0, 0, 0);
    public float spawnRange = 5f;
    public float fireForce = 500f;
    public float bulletLiftTime = 5f;
    public Vector3 bulletScale = new Vector3(0.2f, 0.2f, 0.2f);
    public float bulletSpawnInterval = 1f;
    private bool isSpawning = false;
    
    void SpawnBullet()
    {
        Vector3 randomPosition = spawnCenter + new Vector3(
            Random.Range(-spawnRange, spawnRange),
            10,
            Random.Range(-spawnRange, spawnRange)
        );

        GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bullet.transform.position = randomPosition;
        bullet.transform.localScale = bulletScale;

        Rigidbody rb = bullet.AddComponent<Rigidbody>();

        Vector3 direction = (character - randomPosition).normalized;
        bullet.transform.forward = direction;

        LineRenderer lineRenderer = bullet.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.positionCount = 2; // �������� ����
        lineRenderer.SetPosition(0, randomPosition); // ������: �Ѿ� ���� ��ġ
        lineRenderer.SetPosition(1, randomPosition + direction * 10f); // ����: ���� ��� ��


        rb.AddForce(direction * fireForce);
        rb.useGravity = false;

        Destroy(bullet, bulletLiftTime);
    }
    IEnumerator SpawnBulletsForDuration(float duration)
    {
        isSpawning = true; // ���� ���� Ȱ��ȭ
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            SpawnBullet(); // �Ѿ� ����
            elapsedTime += bulletSpawnInterval; // ��� �ð� ����
            yield return new WaitForSeconds(bulletSpawnInterval); // �ֱ� ���
        }

        isSpawning = false; // ���� ���� ��Ȱ��ȭ
    }
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Ghost");
        character = target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isSpawning)
        {
            StartCoroutine(SpawnBulletsForDuration(10f));
        }
        character = target.transform.position;
    }
}
