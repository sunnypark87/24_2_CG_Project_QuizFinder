using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject target;
    private Vector3 character;
    public float gameTime = 60f;
    public Vector3 spawnCenter = new Vector3(0, 0, 0);
    public float minDistance = 5f;
    public float spawnRange = 10f;
    public float fireForce = 500f;
    public float bulletLiftTime = 5f;
    public Vector3 bulletScale = new Vector3(0.2f, 0.2f, 0.2f);
    public float bulletSpawnInterval = 1f;
    private bool isSpawning = false;

    public Mesh customBulletMesh; // Inspector에서 할당
    public Material customBulletMaterial; // Inspector에서 할당
    public GameObject bulletPrefab; // Prefab을 인스펙터에서 할당

    private LayerMask playerLayer;

    void SpawnBullet()
    {
        float distance = Random.Range(minDistance, spawnRange); // 최소~최대 거리 사이의 값
        float angleY = Random.Range(0f, Mathf.PI * 2); // 0~360도 (라디안)
        float angleXZ = Random.Range(Mathf.PI / 18, Mathf.PI / 3); // 수직 방향 각도 (-45도~45도)

        Vector3 offset = new Vector3(
            distance * Mathf.Cos(angleY) * Mathf.Cos(angleXZ), // X축
            distance * Mathf.Sin(angleXZ), // Y축
            distance * Mathf.Sin(angleY) * Mathf.Cos(angleXZ) // Z축
        );

        Vector3 randomPosition = character + offset;


        GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bullet.transform.position = randomPosition;
        bullet.transform.localScale = bulletScale;

        int bulletLayer = LayerMask.NameToLayer("BulletLayer");
        if (bulletLayer != -1)
        {
            bullet.layer = bulletLayer; // 생성된 Bullet에 Layer 적용
        }

        Rigidbody rb = bullet.AddComponent<Rigidbody>();
        SphereCollider collider = bullet.AddComponent<SphereCollider>();
        collider.isTrigger = true; // 트리거 모드로 설정 (OnTriggerEnter 사용)
        bullet.AddComponent<BulletCollision>();


        Vector3 targetPosition = character + new Vector3(
            Random.Range(-2f, 2f),
            Random.Range(0.5f, 1f),
            Random.Range(-2f, 2f)
        );

        // 방향 벡터 계산
        Vector3 direction = (targetPosition - randomPosition).normalized;


        bullet.transform.forward = direction;
        /*
        LineRenderer lineRenderer = bullet.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.positionCount = 2; // 시작점과 끝점
        lineRenderer.SetPosition(0, randomPosition); // 시작점: 총알 생성 위치
        lineRenderer.SetPosition(1, randomPosition + direction * 10f); // 끝점: 예상 경로 끝
        */
        rb.AddForce(direction * fireForce);
        rb.useGravity = false;

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
        playerLayer = LayerMask.GetMask("PlayerLayer");
        StartCoroutine(SpawnBulletsForDuration(gameTime));
    }

    // Update is called once per frame
    void Update()
    {
        character = target.transform.position;
    }
}

public class BulletCollision : MonoBehaviour
{
    private LayerMask playerLayer;
    void Start()
    {
        playerLayer = LayerMask.GetMask("PlayerLayer");
    }
    // 충돌 시 처리할 행동을 정의
    private void OnTriggerEnter(Collider other)
    {
        // playerLayer와 충돌한 경우 처리
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            // 플레이어와 충돌 시 행동
            Debug.Log("Bullet hit player!");

            // 예시로 플레이어에게 데미지를 주거나, 이벤트를 발생시키는 코드 추가
            // PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            // if (playerHealth != null)
            // {
            //     playerHealth.TakeDamage(10);
            // }

            // 충돌 후 총알을 제거하거나 다른 처리를 할 수 있음
            Destroy(gameObject);

            GameData.deathgameResult = false;
            GameData.deathgameCompleted = true;

            //SceneController.Instance.UnloadDeathgame();
        }
    }
}