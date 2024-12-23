using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    [SerializeField]
    float Speed = 5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W))
        { move += Vector3.up; }
        if (Input.GetKey(KeyCode.S))
        { move += Vector3.down; }
        if (Input.GetKey(KeyCode.A))
        { move += Vector3.left; }
        if (Input.GetKey(KeyCode.D))
        { move += Vector3.right; }

        if (move != Vector3.zero)
        { this.transform.Translate(move * Speed * Time.deltaTime); }
    }
}