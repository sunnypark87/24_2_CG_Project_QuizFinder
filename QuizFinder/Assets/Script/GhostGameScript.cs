using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sample
{
    public class GhostGameScript : MonoBehaviour
    {
        private Animator Anim;
        private CharacterController Ctrl;
        private Vector3 MoveDirection = Vector3.zero;
       
        // Cache hash values
        private static readonly int IdleState = Animator.StringToHash("Base Layer.idle");
        private static readonly int MoveState = Animator.StringToHash("Base Layer.move");
        private static readonly int SurprisedState = Animator.StringToHash("Base Layer.surprised");
        private static readonly int AttackState = Animator.StringToHash("Base Layer.attack_shift");
        private static readonly int DissolveState = Animator.StringToHash("Base Layer.dissolve");
        private static readonly int AttackTag = Animator.StringToHash("Attack");

        // dissolve
        [SerializeField] private SkinnedMeshRenderer[] MeshR;
        private float Dissolve_value = 1;
        private bool DissolveFlg = false;
        private const int maxHP = 3;
        private int HP = maxHP;
        private Text HP_text;

        // moving speed
        [SerializeField] private float Speed = 4;

        // jump
        [SerializeField] private float jumpForce = 8f; // 점프 힘
        private float ySpeed = 0f; // y축 속도
        private bool isGrounded = false; // 바닥에 있는지 체크
        private bool isJumping = false; // 점프 중인지 체크

        void Start()
        {
            Anim = this.GetComponent<Animator>();
            Ctrl = this.GetComponent<CharacterController>();

            HP_text = GameObject.Find("Canvas/HP").GetComponent<Text>();
            HP_text.text = "HP " + HP.ToString();

            if (GetComponent<Collider>() != null)
            {
                gameObject.AddComponent<Collider>();
            }
        }

        void Update()
        {

            STATUS();
            GRAVITY();

            CheckFall();

            // this character status
            if (!PlayerStatus.ContainsValue(true))
            {
                MOVE();
                PlayerAttack();
                Damage();
            }
            else if (PlayerStatus.ContainsValue(true))
            {
                int status_name = 0;
                foreach (var i in PlayerStatus)
                {
                    if (i.Value == true)
                    {
                        status_name = i.Key;
                        break;
                    }
                }
                if (status_name == Dissolve)
                {
                    PlayerDissolve();
                }
                else if (status_name == Attack)
                {
                    PlayerAttack();
                }
                else if (status_name == Surprised)
                {
                    // nothing method
                }
            }
            // Dissolve
            if (HP <= 0 && !DissolveFlg)
            {
                Anim.CrossFade(DissolveState, 0.1f, 0, 0);
                DissolveFlg = true;
            }
            // processing at respawn
            else if (HP == maxHP && DissolveFlg)
            {
                DissolveFlg = false;
            }
        }

        

        //---------------------------------------------------------------------
        // character status
        //---------------------------------------------------------------------
        private const int Dissolve = 1;
        private const int Attack = 2;
        private const int Surprised = 3;
        private Dictionary<int, bool> PlayerStatus = new Dictionary<int, bool>
    {
        {Dissolve, false },
        {Attack, false },
        {Surprised, false },
    };
        //------------------------------
        private void STATUS()
        {
            // during dissolve
            if (DissolveFlg && HP <= 0)
            {
                PlayerStatus[Dissolve] = true;
            }
            else if (!DissolveFlg)
            {
                PlayerStatus[Dissolve] = false;
            }
            // during attacking
            if (Anim.GetCurrentAnimatorStateInfo(0).tagHash == AttackTag)
            {
                PlayerStatus[Attack] = true;
            }
            else if (Anim.GetCurrentAnimatorStateInfo(0).tagHash != AttackTag)
            {
                PlayerStatus[Attack] = false;
            }
            // during damaging
            if (Anim.GetCurrentAnimatorStateInfo(0).fullPathHash == SurprisedState)
            {
                PlayerStatus[Surprised] = true;
            }
            else if (Anim.GetCurrentAnimatorStateInfo(0).fullPathHash != SurprisedState)
            {
                PlayerStatus[Surprised] = false;
            }
        }
        // dissolve shading
        private void PlayerDissolve()
        {
            Dissolve_value -= Time.deltaTime;
            for (int i = 0; i < MeshR.Length; i++)
            {
                MeshR[i].material.SetFloat("_Dissolve", Dissolve_value);
            }
            if (Dissolve_value <= 0)
            {
                Ctrl.enabled = false;
            }
        }
        // play a animation of Attack
        private void PlayerAttack()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Anim.CrossFade(AttackState, 0.1f, 0, 0);
            }
        }
        //---------------------------------------------------------------------
        // gravity for fall of this character
        //---------------------------------------------------------------------
        private void GRAVITY()
        {
            /*
            if (Ctrl.enabled)
            {
                if (CheckGrounded())
                {
                    if (MoveDirection.y < -0.1f)
                    {
                        MoveDirection.y = -0.1f;
                    }
                }
                MoveDirection.y -= 0.1f;
                Ctrl.Move(MoveDirection * Time.deltaTime);
            }*/
            isGrounded = CheckGrounded();

            if (isGrounded)
            {
                if (ySpeed < 0)
                {
                    ySpeed = 0f; // 바닥에 있으면 ySpeed를 0으로 설정
                }

                // 점프 처리
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ySpeed = jumpForce; // 점프 시작
                    isJumping = true;
                }
            }

            // Apply gravity
            ySpeed -= 9.8f * Time.deltaTime; // 중력

            // Apply movement
            MoveDirection.y = ySpeed;
            Ctrl.Move(MoveDirection * Time.deltaTime);
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
        // for slime moving
        //---------------------------------------------------------------------
        private void MOVE()
        {

            // velocity
            if (Anim.GetCurrentAnimatorStateInfo(0).fullPathHash == MoveState)
            {
                Vector3 velocity = Vector3.zero;
                Vector3 rotation = Vector3.zero;

                // Calculate movement direction based on arrow key inputs
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    velocity += new Vector3(0, 0, -Speed);
                    rotation = new Vector3(0, 180, 0);
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    velocity += new Vector3(0, 0, Speed);
                    rotation = new Vector3(0, 0, 0);
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    velocity += new Vector3(Speed, 0, 0);
                    rotation = new Vector3(0, 90, 0);
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    velocity += new Vector3(-Speed, 0, 0);
                    rotation = new Vector3(0, 270, 0);
                }

                // Normalize velocity for diagonal movement
                if (velocity.magnitude > Speed)
                {
                    velocity = velocity.normalized * Speed;
                }

                MOVE_Velocity(velocity, rotation);
            }
            KEY_DOWN();
            KEY_UP();
        }
        //---------------------------------------------------------------------
        // value for moving
        //---------------------------------------------------------------------
        private void MOVE_Velocity(Vector3 velocity, Vector3 rot)
        {
            MoveDirection = new Vector3(velocity.x, MoveDirection.y, velocity.z);
            if (Ctrl.enabled)
            {
                Ctrl.Move(MoveDirection * Time.deltaTime);
            }
            MoveDirection.x = 0;
            MoveDirection.z = 0;
            this.transform.rotation = Quaternion.Euler(rot);
        }
        //---------------------------------------------------------------------
        // whether arrow key is key down
        //---------------------------------------------------------------------
        private void KEY_DOWN()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Anim.CrossFade(MoveState, 0.1f, 0, 0);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Anim.CrossFade(MoveState, 0.1f, 0, 0);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Anim.CrossFade(MoveState, 0.1f, 0, 0);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Anim.CrossFade(MoveState, 0.1f, 0, 0);
            }
        }
        //---------------------------------------------------------------------
        // whether arrow key is key up
        //---------------------------------------------------------------------
        private void KEY_UP()
        {
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                if (!Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
                {
                    Anim.CrossFade(IdleState, 0.1f, 0, 0);
                }
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
                {
                    Anim.CrossFade(IdleState, 0.1f, 0, 0);
                }
            }
            else if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.RightArrow))
                {
                    Anim.CrossFade(IdleState, 0.1f, 0, 0);
                }
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.LeftArrow))
                {
                    Anim.CrossFade(IdleState, 0.1f, 0, 0);
                }
            }
        }
        //---------------------------------------------------------------------
        // damage
        //---------------------------------------------------------------------
        private void Damage()
        {
            // Damaged by outside field.
            if (Input.GetKeyUp(KeyCode.S))
            {
                Anim.CrossFade(SurprisedState, 0.1f, 0, 0);
                HP--;
                HP_text.text = "HP " + HP.ToString();
            }
        }
        //---------------------------------------------------------------------
        // Check if the object has fallen below a certain Y-coordinate
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