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

    // 자식 오브젝트들에 대해 MeshRenderer가 있는 오브젝트에만 Collider 추가
    private void AddColliderToMeshObjects(Transform parent)
    {
        // 부모 객체를 포함하여 모든 자식 오브젝트를 탐색
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        /*
        foreach (Transform child in parent)
        {
            // 자식 오브젝트에 MeshRenderer가 있는지 확인
            if (child.GetComponent<MeshRenderer>() != null)
            {
                // Collider가 없으면 Collider 추가
                if (child.GetComponent<Collider>() == null)
                {
                    BoxCollider boxCollider = child.gameObject.AddComponent<BoxCollider>();

                    // BoxCollider의 크기를 MeshRenderer의 Bounds에 맞게 설정
                    boxCollider.size = child.GetComponent<Renderer>().bounds.size;
                }
            }

            // 자식 오브젝트가 있다면 재귀적으로 탐색
            AddColliderToMeshObjects(child);
        }*/

        foreach (MeshFilter meshFilter in meshFilters)
        {
            // MeshFilter가 있는 오브젝트에 MeshCollider 추가
            GameObject obj = meshFilter.gameObject;

            if (obj.CompareTag("Grass"))
            {
                Collider existingCollider = obj.GetComponent<Collider>();
                if (existingCollider != null)
                {
                    Destroy(existingCollider); // 기존 Collider 제거
                }
                continue; // Collider 생성하지 않음
            }

            if (obj.GetComponent<MeshCollider>() == null) // 이미 MeshCollider가 있으면 건너뜀
            {
                MeshCollider meshCollider = obj.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = meshFilter.sharedMesh; // Mesh를 설정

                // Convex 옵션 설정 (필요 시)
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
