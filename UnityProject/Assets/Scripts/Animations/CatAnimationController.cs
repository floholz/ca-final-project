using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatAnimationController : AnimationBaseScript
{
    public Transform animationTarget;
 
    [Header("Animation Type")]
    public bool walking = false;
    public bool running = false;
    public float speed = 1f;

    [Header("Animation Timings")]
    public bool looping = true;
    [InspectorName("Animation Start Time [s]")]
    public float startTime = 0f;
    [InspectorName("Animation Duration [s]")]
    public float duration = 5f;

    [Header("After Animation")]
    [InspectorName("Next Animations")]
    public List<AnimationBaseScript> nextAnimations;

    // info
    [Header("Spline Info")]
    [InspectorName("Animation State")]
    public string info_animationState;
    [InspectorName("Animation runtime")]
    public float info_animationTime = 0f;



    private bool ERROR = false;
    private Animator animator;
    private eAnimationStatus animationStatus = eAnimationStatus.IDLE;
    private float waitedTime = 0;
    private float animationRuntime = 0;


    // Start is called before the first frame update
    void Start()
    {
        animator = animationTarget.GetComponentInChildren<Animator>();

        if (animator == null)
        {
            ERROR = true;
            Debug.LogError("Error in 'Animation Target'. No Animator in GameObject or Children!");
            return;
        }

        if (walking)
        {
            running = false;
        }

    }

    private void Update()
    {
        if (ERROR) return;
        UpdateInfoFields();

        if (animationStatus == eAnimationStatus.IDLE) return;
        if (animationStatus == eAnimationStatus.ENDED) return;
        if (animationStatus == eAnimationStatus.STOPPED) return;
        float dt = Time.deltaTime;

        WaitForAnimationToStart(dt);
        UpdateAnimation(dt);
    }



    public override void StartAnimation()
    {
        if (looping)
        {
            RunAnimation();
        }
        else
        {
            animationStatus = eAnimationStatus.WAITING;
            waitedTime = 0;
        }
    }  

    public override void StopAnimation()
    {
        animationStatus = eAnimationStatus.STOPPED;
        animator.SetBool("isMoveing", false);
        animator.SetBool("isRunning", false);
        animator.SetFloat("speed", 0);
    }

    public override void EndAnimation()
    {
        animationStatus = eAnimationStatus.ENDED;
        animator.SetBool("isMoveing", false);
        animator.SetBool("isRunning", false);
        animator.SetFloat("speed", 0);
        foreach (AnimationBaseScript anim in nextAnimations)
        {
            anim.StartAnimation();
        }
    }



    private void RunAnimation()
    {
        animator.SetBool("isMoveing", walking);
        animator.SetBool("isRunning", running);
        animator.SetFloat("speed", speed);
        animationStatus = eAnimationStatus.RUNNING;
        animationRuntime = 0;
    }

    private void WaitForAnimationToStart(float dt)
    {
        if (animationStatus == eAnimationStatus.WAITING)
        {
            waitedTime += dt;
            if (waitedTime >= startTime)
            {
                RunAnimation();
            }
        }        
    }

    private void UpdateAnimation(float dt)
    {
        if (animationStatus != eAnimationStatus.RUNNING) return;

        animationRuntime += dt;

        if (!looping)
        {
            if (animationRuntime >= duration)
            {
                EndAnimation();
            }
        }
    }

    private void UpdateInfoFields()
    {
        info_animationState = animationStatus.ToString("F");
        info_animationTime = animationRuntime;
    }
}


