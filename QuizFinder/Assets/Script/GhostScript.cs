using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Sample {
public class GhostScript : MonoBehaviour
{
    private Animator Anim;
    private CharacterController Ctrl;
    private Vector3 MoveDirection = Vector3.zero;
    private NavMeshAgent Agent; // NavMeshAgent 추가
    private bool useNavMesh = false; // NavMesh 모드 활성화 여부

    // Cache hash values
    private static readonly int IdleState = Animator.StringToHash("Base Layer.idle");
    private static readonly int MoveState = Animator.StringToHash("Base Layer.move");

    // moving speed
    [SerializeField] private float Speed = 4;

    void Start()
    {
        Anim = this.GetComponent<Animator>();
        Ctrl = this.GetComponent<CharacterController>();
        Agent = this.GetComponent<NavMeshAgent>(); // NavMeshAgent 초기화
        Agent.enabled = false; // 기본은 비활성화

    }

    void Update()
    {
        HandleInput();

        if (useNavMesh)
        {
            if (Agent.remainingDistance <= Agent.stoppingDistance && !Agent.pathPending)
            {
                Anim.CrossFade(IdleState, 0.1f, 0, 0);
            }
            return;
        }

        GRAVITY();
        CheckFall();
    }

    private void HandleInput()
    {
        // NavMesh 이동 활성화
        if (Input.GetMouseButtonDown(1)) // 우클릭
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // 클릭한 지점의 좌표 출력
                Debug.Log($"우클릭 위치: {hit.point}");

                Agent.enabled = true;
                useNavMesh = true;
                Agent.SetDestination(hit.point); // 이동 목표 설정
                Anim.CrossFade(MoveState, 0.1f, 0, 0); // 걷기 애니메이션
            }

        }

        // NavMesh 이동 해제
        if (Input.GetKeyDown(KeyCode.M))
        {
            useNavMesh = false;
            Agent.enabled = false;
        }
    }

    //---------------------------------------------------------------------
    // gravity for fall of this character
    //---------------------------------------------------------------------
    private void GRAVITY ()
    {
        if(!useNavMesh && Ctrl.enabled)
        {
            if(CheckGrounded())
            {
                if(MoveDirection.y < -0.1f)
                {
                    MoveDirection.y = -0.1f;
                }
            }
            MoveDirection.y -= 0.1f;
            Ctrl.Move(MoveDirection * Time.deltaTime);
        }
    }
    //---------------------------------------------------------------------
    // whether it is grounded
    //---------------------------------------------------------------------
    private bool CheckGrounded()
    {
        if (Ctrl.isGrounded && Ctrl.enabled)
        {
            return true;
        }
        Ray ray = new Ray(this.transform.position + Vector3.up * 0.1f, Vector3.down);
        float range = 0.2f;
        return Physics.Raycast(ray, range);
    }

    //---------------------------------------------------------------------
    // Define the event that occurs when falling
    //---------------------------------------------------------------------
    private void CheckFall()
    {
        if (transform.position.y < -20f)
        {
            OnFallEvent();
        }
    }

    //---------------------------------------------------------------------
    // Define the event that occurs when falling
    //---------------------------------------------------------------------
    private void OnFallEvent()
    {
        Debug.Log("Object has fallen below the threshold!");

        // Example: Reset position to a safe location
        transform.position = new Vector3(0f, 5f, -25f);

    }
    }
}