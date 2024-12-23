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
        bullet.AddComponent<Bullet>();

        Vector3 direction = (character - randomPosition).normalized;
        bullet.transform.forward = direction;

        LineRenderer lineRenderer = bullet.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.positionCount = 2; // 시작점과 끝점
        lineRenderer.SetPosition(0, randomPosition); // 시작점: 총알 생성 위치
        lineRenderer.SetPosition(1, randomPosition + direction * 10f); // 끝점: 예상 경로 끝

        rb.AddForce(direction * fireForce);
        rb.useGravity = false;

        Collider bulletCollider = bullet.GetComponent<Collider>();
        bulletCollider.isTrigger = false;

        Destroy(bullet, bulletLiftTime);
    }
    IEnumerator SpawnBulletsForDuration(float duration)
    {
        isSpawning = true; // 생성 상태 활성화
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            SpawnBullet(); // 총알 생성
            elapsedTime += bulletSpawnInterval; // 경과 시간 증가
            yield return new WaitForSeconds(bulletSpawnInterval); // 주기 대기
        }

        isSpawning = false; // 생성 상태 비활성화
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
public class Bullet : MonoBehaviour
{
    // 총알이 충돌할 때 호출되는 메서드
    void OnCollisionEnter(Collision collision)
    {
        // 충돌한 물체가 "Target"인 경우
        if (collision.gameObject.CompareTag("Map"))
        {
            // 총알이 타겟과 충돌 시 총알을 파괴
            Destroy(gameObject);  // 총알 파괴
            Debug.Log("Bullet hit the target!");
        }
    }
}