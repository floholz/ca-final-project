using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEndAnimation : AnimationBaseScript
{
    [InspectorName("Fade Out Time [s]")]
    public float fadeOutTime = 0f;

    private GlobalAnimationInfo globalAnimationInfo;
    private eAnimationStatus animationStatus = eAnimationStatus.IDLE;
    private float animationRuntime = 0;

    // info
    [Header("Spline Info")]
    [InspectorName("Animation State")]
    public string info_animationState;
    [InspectorName("Animation runtime")]
    public float info_animationTime = 0f;


    // Start is called before the first frame update
    void Start()
    {
        globalAnimationInfo = transform.GetComponentInParent<GlobalAnimationInfo>();

        if (globalAnimationInfo == null)
        {
            Debug.LogError("Error in parent. No 'GlobalAnimationInfo' script is set!");
            return;
        }
    }

    private void Update()
    {
        UpdateInfoFields();

        if (animationStatus == eAnimationStatus.IDLE) return;
        if (animationStatus == eAnimationStatus.ENDED) return;
        if (animationStatus == eAnimationStatus.STOPPED) return;
        float dt = Time.deltaTime;

        UpdateAnimation(dt);
    }


    public override void StartAnimation()
    {
        animationStatus = eAnimationStatus.RUNNING;
        animationRuntime = 0;
    }

    public override void EndAnimation()
    {
        animationStatus = eAnimationStatus.ENDED;
        globalAnimationInfo.GlobalEndAnimation();
    }

    private void UpdateAnimation(float dt)
    {
        if (animationStatus != eAnimationStatus.RUNNING) return;

        animationRuntime += dt;
        if (animationRuntime >= fadeOutTime)
        {
            EndAnimation();
        }
    }

    private void UpdateInfoFields()
    {
        info_animationState = animationStatus.ToString("F");
        info_animationTime = animationRuntime;
    }
}
