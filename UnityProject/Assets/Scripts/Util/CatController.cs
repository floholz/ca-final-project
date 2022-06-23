using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class CatController : MonoBehaviour
{
    private const float MOVE_SPEED = 7.5f;

    private float movement = 0f;
    private Vector3 direction = new Vector3(1, 0, 0);
    

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * movement * MOVE_SPEED * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);
        animator.SetBool("isMoveing", movement > 0);
        animator.SetBool("isRunning", movement > 0.93f);
        animator.SetFloat("speed", movement);
    }

    public void OnMove(InputValue input)
    {
        Vector2 inputVec = input.Get<Vector2>();
        Vector3 value = new Vector3(inputVec.x, 0, inputVec.y);
        movement = Vector3.Magnitude(value);
        if (movement > 0)
        {
            direction = Vector3.Normalize(value);
        }        
    }

}
