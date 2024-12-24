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

    public Mesh customBulletMesh; // Inspector���� �Ҵ�
    public Material customBulletMaterial; // Inspector���� �Ҵ�
    public GameObject bulletPrefab; // Prefab�� �ν����Ϳ��� �Ҵ�

    private LayerMask playerLayer;

    void SpawnBullet()
    {
        float distance = Random.Range(minDistance, spawnRange); // �ּ�~�ִ� �Ÿ� ������ ��
        float angleY = Random.Range(0f, Mathf.PI * 2); // 0~360�� (����)
        float angleXZ = Random.Range(Mathf.PI / 18, Mathf.PI / 3); // ���� ���� ���� (-45��~45��)

        Vector3 offset = new Vector3(
            distance * Mathf.Cos(angleY) * Mathf.Cos(angleXZ), // X��
            distance * Mathf.Sin(angleXZ), // Y��
            distance * Mathf.Sin(angleY) * Mathf.Cos(angleXZ) // Z��
        );

        Vector3 randomPosition = character + offset;


        GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bullet.transform.position = randomPosition;
        bullet.transform.localScale = bulletScale;

        int bulletLayer = LayerMask.NameToLayer("BulletLayer");
        if (bulletLayer != -1)
        {
            bullet.layer = bulletLayer; // ������ Bullet�� Layer ����
        }

        Rigidbody rb = bullet.AddComponent<Rigidbody>();
        SphereCollider collider = bullet.AddComponent<SphereCollider>();
        collider.isTrigger = true; // Ʈ���� ���� ���� (OnTriggerEnter ���)
        bullet.AddComponent<BulletCollision>();


        Vector3 targetPosition = character + new Vector3(
            Random.Range(-2f, 2f),
            Random.Range(0.5f, 1f),
            Random.Range(-2f, 2f)
        );

        // ���� ���� ���
        Vector3 direction = (targetPosition - randomPosition).normalized;


        bullet.transform.forward = direction;
        /*
        LineRenderer lineRenderer = bullet.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.positionCount = 2; // �������� ����
        lineRenderer.SetPosition(0, randomPosition); // ������: �Ѿ� ���� ��ġ
        lineRenderer.SetPosition(1, randomPosition + direction * 10f); // ����: ���� ��� ��
        */
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
    // �浹 �� ó���� �ൿ�� ����
    private void OnTriggerEnter(Collider other)
    {
        // playerLayer�� �浹�� ��� ó��
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            // �÷��̾�� �浹 �� �ൿ
            Debug.Log("Bullet hit player!");

            // ���÷� �÷��̾�� �������� �ְų�, �̺�Ʈ�� �߻���Ű�� �ڵ� �߰�
            // PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            // if (playerHealth != null)
            // {
            //     playerHealth.TakeDamage(10);
            // }

            // �浹 �� �Ѿ��� �����ϰų� �ٸ� ó���� �� �� ����
            Destroy(gameObject);

            GameData.deathgameResult = false;
            GameData.deathgameCompleted = true;

            //SceneController.Instance.UnloadDeathgame();
        }
    }
}