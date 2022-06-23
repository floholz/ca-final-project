using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalAnimationInfo : MonoBehaviour
{
    [InspectorName("Start Animations")]
    public List<AnimationBaseScript> startAnimations;

    [InspectorName("Force Stop Animations")]
    public List<AnimationBaseScript> stopAnimations;

    [Header("Info")]
    [InspectorName("Animation State")]
    public string info_animationState;
    [InspectorName("Animation runtime")]
    public float info_animationTime = 0f;

    [HideInInspector()]
    public eAnimationStatus animationStatus = eAnimationStatus.IDLE;
    private float animationRuntime = 0;


    void Start()
    {
        GlobalStartAnimation();
    
        foreach (AnimationBaseScript anim in startAnimations)
        {
            anim.StartAnimation();
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (animationStatus == eAnimationStatus.RUNNING)
        {
            animationRuntime += Time.deltaTime;
        }

        UpdateInfoFields();
    }
    
    public void GlobalEndAnimation()
    {
        animationStatus = eAnimationStatus.ENDED;
        foreach (AnimationBaseScript anim in stopAnimations)
        {
            anim.StopAnimation();
        }
        // Application.Quit();
    }

    private void GlobalStartAnimation()
    {
        animationRuntime = 0;
        animationStatus = eAnimationStatus.RUNNING;
    }



    private void UpdateInfoFields()
    {
        info_animationState = animationStatus.ToString("F");
        info_animationTime = animationRuntime;
    }
}
