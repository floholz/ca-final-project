using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoAnimationController : AnimationBaseScript
{
    public enum AnimationType
    {
        IDLE, 
        RUN, 
        WALK, 
        SIT, 
        LAY
    }
    
    public Transform animationTarget;

    [Header("Animation Settings")] 
    public List<AnimationType> types;
    public float speed = 1f;

    [Header("Animation Timings")] 
    [InspectorName("Animation Start Time [s]")]
    public float startTime = 0f;

    public bool fixedDuration = false;
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
    private string animatorEndState;
    
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
        animationStatus = eAnimationStatus.WAITING;
        waitedTime = 0;
    }  

    public override void StopAnimation()
    {
        animationStatus = eAnimationStatus.STOPPED;
    }

    public override void EndAnimation()
    {
        animationStatus = eAnimationStatus.ENDED;
        foreach (AnimationBaseScript anim in nextAnimations)
        {
            anim.StartAnimation();
        }
    }



    private void RunAnimation()
    {
        
        switch (types[types.Count - 1])
        {
            case AnimationType.IDLE:
                animatorEndState = "Idle";
                break;
            case AnimationType.WALK:
                animatorEndState = "Walkcycle";
                break;
            case AnimationType.RUN:
                animatorEndState = "Runcycle";
                break;
            case AnimationType.SIT:
                animatorEndState = "Sittingcyle";
                break;
            case AnimationType.LAY:
                animatorEndState = "Layingcycle";
                break;
        }
        animator.ResetTrigger("idle");
        animator.ResetTrigger("walk");
        animator.ResetTrigger("run");
        animator.ResetTrigger("sit");
        animator.ResetTrigger("lay");
        foreach (AnimationType type in types)
        {
            switch (type)
            {
                case AnimationType.IDLE:
                    animator.SetTrigger("idle");
                    break;
                case AnimationType.WALK:
                    animator.SetTrigger("walk");
                    break;
                case AnimationType.RUN:
                    animator.SetTrigger("run");
                    break;
                case AnimationType.SIT:
                    animator.SetTrigger("sit");
                    break;
                case AnimationType.LAY:
                    animator.SetTrigger("lay");
                    break;
            }
        }
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
        if (fixedDuration)
        {
            if (animationRuntime >= duration)
            {
                EndAnimation();
            }
        }
        else
        {
            if (animatorEndState == animator.GetCurrentAnimatorClipInfo(0)[0].clip.name)
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


