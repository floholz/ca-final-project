using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimationController : AnimationBaseScript
{
    public enum TriggerState
    {
        OPEN, CLOSE
    }
    
    public Transform animationTarget;

    [InspectorName("Animation Start Time [s]")]
    public float startTime = 0f;
    [InspectorName("Trigger open [open/close]")]
    public TriggerState action;

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
    }

    // Update is called once per frame
    void Update()
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

    private void RunAnimation()
    {
        animator.ResetTrigger("open");
        animator.ResetTrigger("close");
        switch (action)
        {
            case TriggerState.OPEN:
                animator.SetTrigger("open");
                break;
            case TriggerState.CLOSE:
                animator.SetTrigger("close");
                break;
        }
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


    private void UpdateAnimation(float dt)
    {
        if (animationStatus != eAnimationStatus.RUNNING) return;

        animationRuntime += dt;
        switch (action)
        {
            case TriggerState.OPEN:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("idle-open"))
                {
                    EndAnimation();
                }
                break;
            case TriggerState.CLOSE:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
                {
                    EndAnimation();
                }
                break;
        }
    }


    private void UpdateInfoFields()
    {
        info_animationState = animationStatus.ToString("F");
        info_animationTime = animationRuntime;
    }


}
