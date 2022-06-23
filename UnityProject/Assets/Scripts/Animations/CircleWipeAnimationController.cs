using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleWipeAnimationController : AnimationBaseScript
{
    [InspectorName("Animation Start Time [s]")]
    public float startTime = 0f;
    [InspectorName("Animation Duration [s]")]
    public float duration = 5f;
    [InspectorName("Animation Target")]
    public Transform animationTarget;

    [Header("Animation Settings")]
    [Range(0, 1)]
    public float startRadius = 0;
    [Range(0, 1)]
    public float endRadius = 1;

    [InspectorName("Next Animations")]
    public List<AnimationBaseScript> nextAnimations;

    // info
    [Header("Spline Info")]
    [InspectorName("Animation State")]
    public string info_animationState;
    [InspectorName("Animation runtime")]
    public float info_animationTime = 0f;
    [Range(0, 1)]
    [InspectorName("Current Radius")]
    public float info_currentRadius = 0f;

    private bool ERROR = false;
    private CircleWipeShaderController wipeController;
    private eAnimationStatus animationStatus = eAnimationStatus.IDLE;
    private float waitedTime = 0;
    private float animationRuntime = 0;
    private float currentRadius;
    private float radiusChange;

    // Start is called before the first frame update
    void Start()
    {
        if (animationTarget == null)
        {
            ERROR = true;
            Debug.LogError("Error in Settup. 'Animation Target' needs to be set!");
            return;
        }

        wipeController = animationTarget.GetComponent<CircleWipeShaderController>();
        if (wipeController == null)
        {
            ERROR = true;
            Debug.LogError("Error in 'Animation Target'. GameObject does not have 'CircleWipeShaderController' component!");
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

    public override void StopAnimation()
    {
        animationStatus = eAnimationStatus.STOPPED;
        FinishAnimation();
    }

    public override void EndAnimation()
    {
        animationStatus = eAnimationStatus.ENDED;
        FinishAnimation();
    }

    private void RunAnimation()
    {
        animationStatus = eAnimationStatus.RUNNING;
        animationRuntime = 0;
        currentRadius = startRadius;
        wipeController.SetRadius(startRadius);
        radiusChange = (endRadius - startRadius) / duration;
    }

    private void FinishAnimation()
    {
        foreach (AnimationBaseScript anim in nextAnimations)
        {
            anim.StartAnimation();
        }
    }

    private void UpdateAnimation(float dt)
    {
        if (animationStatus != eAnimationStatus.RUNNING) return;

        animationRuntime += dt;
        if (animationRuntime > duration)
        {
            EndAnimation();
        }
        currentRadius += radiusChange * dt;
        wipeController.SetRadius(currentRadius);
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

    private void UpdateInfoFields()
    {
        info_animationState = animationStatus.ToString("F");
        info_animationTime = animationRuntime;
        info_currentRadius = currentRadius;
    }
}
