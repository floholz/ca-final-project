using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class CRSpline
{
	public int numberOfInterpolatedPoints
	{
		get => interpolList.Count;
		set { /* readonly */ }
	}

	private float _splineLength = 0;
	public float splineLength
	{
		get => _splineLength;
		set { /* readonly */ }
	}

	private float _animationRuntime = 0;
	public float animationRuntime
	{
		get => _animationRuntime;
		set { /* readonly */ }
	}

	private bool _splineAnimationIsRunning = false;
	public bool splineAnimationIsRunning
	{
		get => _splineAnimationIsRunning;
		set { /* readonly */ }
	}

	private int id;
	private List<IAnimationEventListener> eventListenerList = new List<IAnimationEventListener>();
	private List<Transform> pointList;
	private List<CRSplinePoint> interpolList;
	private List<GameObject> lineList = new List<GameObject>();
	private float _resolution;
	private float resolution
    {
		get => _resolution;
		set => _resolution = math.clamp(value, 0.0001f, 1f);
    }
	private AlternatingColor altColor;
	private Transform animationTarget = null;
	private Transform pathTransform;
	private Transform cameraTarget = null;
	private float3 animTargPos;
	private quaternion animTargRot;
	private int pointIdx;
	private float duration;
	private float maxDist;
	private bool smoothStart;
	private float cameraStiffness;


	public CRSpline(Transform points, Transform path, Transform cameraTarget, float resolution, float duration, float cameraStiffness, bool smoothStart) 
		: this(points, path, cameraTarget, resolution, duration, cameraStiffness, smoothStart, Color.green, Color.black)
	{ }

	public CRSpline(Transform points, Transform path, Transform cameraTarget, float resolution, float duration, float cameraStiffness, bool smoothStart, Color primary, Color secondary)
	{
		this.pathTransform = path;
		this.cameraTarget = cameraTarget;
		this.resolution = resolution;
		this.duration = duration;
		this.cameraStiffness = cameraStiffness;
		this.smoothStart = smoothStart;

		altColor = new AlternatingColor(primary, secondary);

		// init point list
		InitPointList(points);
	}

	public void SetEventListener(IAnimationEventListener eventListener, int id)
	{
		this.id = id;
		eventListenerList.Add(eventListener);
	}

	public bool CalculateCatmullRomSpline(bool drawPath = false)
    {
		List<float3> ipList = GenerateInterpolatedPoints();
        if (ipList == null) return false;
		interpolList = GenerateSplinePoints(ipList);
		if (drawPath) DrawPath();
		return true;
	}

	public void RunSplineAnimation(Transform animTarget = null)
	{
		Debug.Log("Run " + pathTransform.parent.gameObject.name);
		if (animTarget != null) SetAnimationTarget(animTarget);

		if (animationTarget == null) return; 
		if (interpolList.Count < 2) return;

		_splineAnimationIsRunning = true;
		pointIdx = 1;
		
		animTargPos = interpolList[0].pos;
		float3 direction;
		if (cameraTarget == null)
		{
			direction = interpolList[1].pos - interpolList[0].pos;
		}
		else
		{
			direction = math.float3(cameraTarget.position) - animTargPos;
		}

		animTargRot = animationTarget.rotation;
		if (smoothStart)
		{
			animTargRot = math.slerp(
				animTargRot, 
				quaternion.LookRotationSafe(direction, math.float3(0, 1, 0)), 0.1f
			);
		}
		else
		{
			animTargRot = quaternion.LookRotationSafe(direction, math.float3(0, 1, 0));
		}
		maxDist = math.length(interpolList[1].pos - interpolList[0].pos);

		_animationRuntime = 0;
	}

	public void ResetSplineAnimation()
	{
		_splineAnimationIsRunning = false;
		animTargPos = interpolList[0].pos;
		animTargRot = quaternion.LookRotationSafe(interpolList[1].pos - interpolList[0].pos, math.float3(0, 1, 0));
		animationTarget.SetPositionAndRotation(animTargPos, animTargRot);
	}

	public void FinishedSplineAnimation()
	{
		_splineAnimationIsRunning = false;

		foreach (var listener in eventListenerList)
		{
			listener.AnimationIsDone(id);
		}
	}

	public void SplineAnimationUpdate(float dt)
	{
		if (!_splineAnimationIsRunning) return;  // not yet active

		_animationRuntime += dt;

		float3 move = interpolList[pointIdx].pos - animTargPos;
		float progress = math.length(animTargPos - interpolList[pointIdx - 1].pos);

		if (maxDist < progress) 
		{
			// next point
			pointIdx++;
			if (pointIdx >= interpolList.Count - 1)
			{
				// animation done 
				FinishedSplineAnimation();
				return;
			}
			move = interpolList[pointIdx].pos - animTargPos;
			maxDist = math.length(interpolList[pointIdx].pos - interpolList[pointIdx - 1].pos);
		}

		float stepSpeed = (dt / duration) * _splineLength;
		animTargPos += math.normalize(move) * stepSpeed;
		
		float3 direction;
		if (cameraTarget == null)
		{
			direction = math.normalize(move);
		}
		else
		{
			direction = math.float3(cameraTarget.position) - animTargPos;
		}
		animTargRot = math.slerp(
			animTargRot, 
			quaternion.LookRotationSafe(direction, math.float3(0, 1, 0)), stepSpeed * cameraStiffness
			);

		// set target position and rotation
		animationTarget.SetPositionAndRotation(animTargPos, animTargRot);
	}

	public void SetAnimationTarget(Transform target)
    {
		animationTarget = target;
		animTargPos = target.position;
		animTargRot = target.rotation;
    }

	public void SetColors(Color primary, Color secondary)
	{
		altColor = new AlternatingColor(primary, secondary);
	}


	private void InitPointList(Transform pointsTransfom)
	{
		pointList = new List<Transform>();
		foreach (Transform child in pointsTransfom)
		{
			pointList.Add(child);
		}
	}

	private List<float3> GenerateInterpolatedPoints() 
	{
		List<float3> ipList = new List<float3>();

		if (pointList.Count < 3) return null; // min 3 points to culculate

		int n = pointList.Count - 1;

		float4x4 M = math.float4x4(
				-1, 3, -3, 1,
				 2, -5, 4, -1,
				-1, 0, 1, 0,
				 0, 2, 0, 0
			) * 0.5f;


		// first point
		float3 prevPoint = pointList[0].position;
		float3 P_0;
		{
			P_0 = pointList[0].position + 0.5f * (2.0f * pointList[1].position - pointList[2].position - pointList[0].position);


			float3x4 B = math.float3x4(
					P_0,
					pointList[0].position,
					pointList[1].position,
					pointList[2].position
				);

			float4x3 X = math.transpose(B);

			for (float u = 0; u < 1; u += resolution)
			{
				float4 UT = math.float4(u * u * u, u * u, u, 1);

				float3 P = math.mul(UT, math.mul(M, X));

				ipList.Add(P);
				_splineLength += math.distance(prevPoint, P);
				prevPoint = P;
			}
		}

		// other points (excl. last)
		{
			for (int i = 1; i < n - 1; i++)
			{
				float3x4 B = math.float3x4(
						pointList[i - 1].position,
						pointList[i].position,
						pointList[i + 1].position,
						pointList[i + 2].position);

				float4x3 X = math.transpose(B);

				for (float u = 0; u < 1; u += resolution)
				{
					float4 UT = math.float4(u * u * u, u * u, u, 1);

					float3 P = math.mul(UT, math.mul(M, X));

					ipList.Add(P);
					_splineLength += math.distance(prevPoint, P);
					prevPoint = P;
				}
			}
		}

		// last point
		float3 P_n;
		{
			P_n = pointList[n].position + 0.5f * (2.0f * pointList[n - 1].position - pointList[n - 2].position - pointList[n].position);

			float3x4 B = math.float3x4(
				pointList[n - 2].position,
				pointList[n - 1].position,
				pointList[n].position,
				P_n);

			float4x3 X = math.transpose(B);

			for (float u = 0; u < 1; u += resolution)
			{
				float4 UT = math.float4(u * u * u, u * u, u, 1);

				float3 P = math.mul(UT, math.mul(M, X));

				ipList.Add(P);
				_splineLength += math.distance(prevPoint, P);
				prevPoint = P;
			}

		}

		ipList.Add(pointList[n].position);
		_splineLength += math.distance(prevPoint, pointList[n].position);
		return ipList;
	}

	private List<CRSplinePoint> GenerateSplinePoints(List<float3> interpolatedPoints)
    {
		List<CRSplinePoint> splinePoints = new List<CRSplinePoint>();

		for (int i = 1; i < interpolatedPoints.Count; i++)
		{
			float dist = math.distance(interpolatedPoints[i - 1], interpolatedPoints[i]);
			float time = (dist / _splineLength) * duration;
			splinePoints.Add(new CRSplinePoint(interpolatedPoints[i - 1], dist, time));
		}
		int n = interpolatedPoints.Count - 1;
		float distn = math.distance(interpolatedPoints[n - 1], interpolatedPoints[n]);
		splinePoints.Add(new CRSplinePoint(interpolatedPoints[n], distn, 0));

		return splinePoints;
    }

	private void DrawPath()
	{
		ResetPath();
		DrawLine(
					interpolList[0].pos,
					interpolList[1].pos,
					altColor.next()
				);

		for (int i = 1; i < interpolList.Count; i++)
		{
			Vector3 p1 = interpolList[i - 1].pos;
			Vector3 p2 = interpolList[i].pos;

			DrawLine(p1, p2, altColor.next());
		}
	}

	private void ResetPath()
	{
		foreach (GameObject go in lineList)
		{
            Object.Destroy(go);
		}
		lineList.Clear();
	}

	private void DrawLine(Vector3 start, Vector3 end, Color color)
	{
		GameObject myLine = new GameObject();
		myLine.name = $"Line-{lineList.Count}";
		myLine.layer = LayerMask.NameToLayer("Spline");
		myLine.transform.parent = pathTransform;
		myLine.transform.position = start;
		myLine.AddComponent<LineRenderer>();
		LineRenderer lr = myLine.GetComponent<LineRenderer>();
		lr.material = new Material(Shader.Find("Sprites/Default"));
		lr.startColor = color;
		lr.endColor = color;
		lr.startWidth = 0.1f;
		lr.endWidth = 0.1f;
		lr.SetPosition(0, start);
		lr.SetPosition(1, end);
		lineList.Add(myLine);
	}

}
