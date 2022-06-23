using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutVolumeAnimation : AnimationBaseScript
{
    [InspectorName("Animation Start Time [s]")]
    public float startTime = 0f;
    [InspectorName("Animation Duration [s]")]
    public float duration = 5f;
    [InspectorName("Animation Target")]
    public Transform animationTarget;

    [InspectorName("Next Animations")]
    public List<AnimationBaseScript> nextAnimations;

    // info
    [Header("Spline Info")]
    [InspectorName("Animation State")]
    public string info_animationState;
    [InspectorName("Animation runtime")]
    public float info_animationTime = 0f;
    [InspectorName("Current Volume")]
    public float info_currentVolume = 0f;

    private bool ERROR = false;
    private AudioSource audioPlayer;
    private eAnimationStatus animationStatus = eAnimationStatus.IDLE;
    private float waitedTime = 0;
    private float animationRuntime = 0;
    private float currentVolume;
    private float volumeChange;


    // Start is called before the first frame update
    void Start()
    {
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
        audioPlayer = animationTarget.GetComponent<AudioSource>();
        if (audioPlayer == null)
        {
            ERROR = true;
            Debug.LogError("Error in 'Animation Target'. GameObject does not have 'AudioSource' component!");
            return;
        }
        animationStatus = eAnimationStatus.WAITING;
        waitedTime = 0;
    }

    private void RunAnimation()
    {
        animationStatus = eAnimationStatus.RUNNING;
        animationRuntime = 0;
        currentVolume = audioPlayer.volume;
        volumeChange = (0 - currentVolume) / duration;
    }

    public override void StopAnimation()
    {
        audioPlayer.Stop();
        animationStatus = eAnimationStatus.STOPPED;
    }

    public override void EndAnimation()
    {
        audioPlayer.Stop();
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
        if (animationRuntime > duration)
        {
            EndAnimation();
        }
        currentVolume += volumeChange * dt;
        audioPlayer.volume = currentVolume;
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
        info_currentVolume = currentVolume;
    }
}
