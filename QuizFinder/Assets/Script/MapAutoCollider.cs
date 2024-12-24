using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapAutoCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AddColliderToMeshObjects(transform);
    }

    // �ڽ� ������Ʈ�鿡 ���� MeshRenderer�� �ִ� ������Ʈ���� Collider �߰�
    private void AddColliderToMeshObjects(Transform parent)
    {
        // �θ� ��ü�� �����Ͽ� ��� �ڽ� ������Ʈ�� Ž��
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        /*
        foreach (Transform child in parent)
        {
            // �ڽ� ������Ʈ�� MeshRenderer�� �ִ��� Ȯ��
            if (child.GetComponent<MeshRenderer>() != null)
            {
                // Collider�� ������ Collider �߰�
                if (child.GetComponent<Collider>() == null)
                {
                    BoxCollider boxCollider = child.gameObject.AddComponent<BoxCollider>();

                    // BoxCollider�� ũ�⸦ MeshRenderer�� Bounds�� �°� ����
                    boxCollider.size = child.GetComponent<Renderer>().bounds.size;
                }
            }

            // �ڽ� ������Ʈ�� �ִٸ� ��������� Ž��
            AddColliderToMeshObjects(child);
        }*/

        foreach (MeshFilter meshFilter in meshFilters)
        {
            // MeshFilter�� �ִ� ������Ʈ�� MeshCollider �߰�
            GameObject obj = meshFilter.gameObject;

            if (obj.CompareTag("Grass"))
            {
                Collider existingCollider = obj.GetComponent<Collider>();
                if (existingCollider != null)
                {
                    Destroy(existingCollider); // ���� Collider ����
                }
                continue; // Collider �������� ����
            }

            if (obj.GetComponent<MeshCollider>() == null) // �̹� MeshCollider�� ������ �ǳʶ�
            {
                MeshCollider meshCollider = obj.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = meshFilter.sharedMesh; // Mesh�� ����

                // Convex �ɼ� ���� (�ʿ� ��)
                meshCollider.convex = false; // or true for simpler collisions
                meshCollider.isTrigger = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
