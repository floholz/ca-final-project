using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutmullRomSplineScript : AnimationBaseScript, IAnimationEventListener
{
    [Header("Spline Settings")]
    public bool useDifferentColorScheme = false;
    [Range(0.0001f, 1f)]
    [InspectorName("Spline Resolution [0..1]")]
    public float resolution = 0.075f;
    public bool smoothStart = false;
    [InspectorName("Animation Start Time [s]")]
    public float startTime = 0f;
    [InspectorName("Animation Duration [s]")]
    public float duration = 30f;
    public bool drawPath = true;
    [Range(0.01f, 3f)]
    public float cameraStiffness = 1f;
    [InspectorName("Animation Target")]
    public Transform animationTarget;
    [InspectorName("Use the 'CameraTarget' as focus point")]
    public bool useCameraTarget = false;
    public Transform externalCameraTarget;
    [InspectorName("Paralell Animations")]
    public List<AnimationBaseScript> paralellAnimations;
    [InspectorName("Next Animations")]
    public List<AnimationBaseScript> nextAnimations;

    // info
    [Header("Spline Info")]
    [InspectorName("Number of interpolated points")]
    public int info_numberOfInterpolatedPoints;
    [InspectorName("Animation State")]
    public string info_animationState;
    [InspectorName("Animation runtime")]
    public float info_animationTime = 0f;

    private bool ERROR = false;
    private eAnimationStatus animationStatus = eAnimationStatus.IDLE;
    private CRSpline spline;
    private Transform pointsTransfom;
    private Transform pathTransfom;
    private Transform cameraTargetTransform;
    private float waitedTime = 0;
    private int id;

    // Start is called before the first frame update
    void Start()
    {
        pointsTransfom = transform.Find("Points");
        pathTransfom = transform.Find("Path");
        cameraTargetTransform = transform.Find("CameraTarget");

        if (pointsTransfom == null)
        {
            ERROR = true;
            Debug.LogError("Error in prefab. 'Points' GameObject does not exist!");
            return;
        }
        if (pathTransfom == null)
        {
            ERROR = true;
            Debug.LogError("Error in prefab. 'Path' GameObject does not exist!");
            return;
        }
        if (animationTarget == null)
        {
            ERROR = true;
            Debug.LogError("Error in Settup. 'Animation Target' needs to be set!");
            return;
        }
        if (cameraTargetTransform == null)
        {
            ERROR = true;
            Debug.LogError("Error in prefab. 'CameraTarget' GameObject does not exist!");
            return;
        }
        if (useCameraTarget)
        {
            cameraTargetTransform.gameObject.SetActive(true);
            if (externalCameraTarget != null)
            {
                cameraTargetTransform = externalCameraTarget;
            }
        }
        else
        {
            cameraTargetTransform.gameObject.SetActive(false);
            cameraTargetTransform = null;
        }
        
        spline = new CRSpline(pointsTransfom, pathTransfom, cameraTargetTransform, resolution, duration, cameraStiffness, smoothStart);
        if (useDifferentColorScheme) spline.SetColors(Color.blue, Color.red);
        spline.CalculateCatmullRomSpline(drawPath);
        spline.SetAnimationTarget(animationTarget);
        spline.SetEventListener(this , 0);
        
    }

    private void FixedUpdate()
    {
        if (ERROR) return;
        float dt = Time.fixedDeltaTime;

        UpdateInfoFields();
    }

    // Update is called once per frame
    void Update()
    {
        if (ERROR) return;
        float dt = Time.deltaTime;

        WaitForAnimationToStart(dt);
        spline.SplineAnimationUpdate(dt);
    }


    public override void StartAnimation()
    {
        animationStatus = eAnimationStatus.WAITING;
        waitedTime = 0;
        Debug.Log("Started " + transform.gameObject.name);
    }

    public override void EndAnimation()
    {
        animationStatus = eAnimationStatus.ENDED;
        Debug.Log("Ended " + transform.gameObject.name);
    }

    public override void StopAnimation()
    {
        animationStatus = eAnimationStatus.STOPPED;
        Debug.Log("Stopped " + transform.gameObject.name);
    }
    
    void IAnimationEventListener.AnimationIsDone(int id)
    {
        animationStatus = eAnimationStatus.ENDED;

        foreach (AnimationBaseScript anim in paralellAnimations)
        {
            anim.StopAnimation();
        }
        foreach (AnimationBaseScript item in nextAnimations)
        {
            item.StartAnimation();
        }

    }


    private void UpdateInfoFields()
    {
        info_numberOfInterpolatedPoints = spline.numberOfInterpolatedPoints;
        info_animationState = animationStatus.ToString("F");
        info_animationTime = spline.animationRuntime;
    }

    private void WaitForAnimationToStart(float dt)
    {
        if (animationStatus != eAnimationStatus.WAITING) return;

        waitedTime += dt;
        if (waitedTime >= startTime)
        {
            spline.RunSplineAnimation();
            animationStatus = eAnimationStatus.RUNNING;
            foreach (AnimationBaseScript anim in paralellAnimations)
            {
                anim.StartAnimation();
            }
        }
    }
}


