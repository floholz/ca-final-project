using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAnimationScript : AnimationBaseScript
{
    [InspectorName("Animation Start Time [s]")]
    public float startTime = 0f;
    public AudioClip clip;
    [Range(0, 1)]
    public float volume = 1f;
    [InspectorName("Pitch / Speed")]
    [Range(0.1f, 2f)]
    public float pitch = 1f;

    [InspectorName("Next Animations")]
    public List<AnimationBaseScript> nextAnimations;

    // info
    [Header("Spline Info")]
    [InspectorName("Animation State")]
    public string info_animationState;
    [InspectorName("Animation runtime")]
    public float info_animationTime = 0f;


    private bool ERROR = false;
    private AudioSource audioPlayer;
    private eAnimationStatus animationStatus = eAnimationStatus.IDLE;
    private float waitedTime = 0;
    private float animationRuntime = 0;
    private float duration = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (clip == null)
        {
            ERROR = true;
            Debug.LogError("Error in 'Prefab'. No 'Clip' was set!");
            return;
        }

        duration = clip.length;

        audioPlayer = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        //audioPlayer = GetComponent<AudioSource>();
        audioPlayer.clip = clip;
        audioPlayer.volume = volume;
        audioPlayer.pitch = pitch;
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
        animationStatus = eAnimationStatus.RUNNING;
        animationRuntime = 0;
        audioPlayer.Play();
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
    }
}
