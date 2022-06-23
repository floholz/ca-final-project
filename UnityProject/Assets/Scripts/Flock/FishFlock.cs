using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class FishFlock : MonoBehaviour
{
    public int numberOfFish = 10;
    public GameObject fishGO;
    public Transform aquarium;
    
    [Range(1.0f, 10.0f)]
    public float neighbourDistance;
    [Range(1.0f, 5.0f)]
    public float separationDist;
    [Range(1.0f, 5.0f)]
    public float boundaryAvoid;
    [Range(0.0f, 5.0f)]
    public float rotationSpeed;
    
    [Header("Fish Settings")]
    [Range(0.0f, 5.0f)]
    public float minSpeed;
    [Range(0.0f, 5.0f)]
    public float maxSpeed;
    [Header("Padding")]
    public float top = 0;
    public float right = 0;
    public float left = 0;
    public float bottom = 0;
    public float front = 0;
    public float back = 0;

    [Header("Boid Forces")] public bool useMovement = false;
    public bool useSeparation = false;
    public bool useAlignment = false;
    public bool useCohesion = false;
    public bool useCollision = false;

    [Range(0, 1f)]
    public float weightMovement;
    [Range(0, 1f)]
    public float weightSeparation;
    [Range(0, 1f)]
    public float weightAlignment;
    [Range(0, 1f)]
    public float weightCohesion;
    [Range(0, 1f)]
    public float weightCollision;
    
    
    public GameObject[] allFish;
    public Bounds boundaries;
    public Bounds swimBounds;
    public float3 swimLimits;

    private Transform boidsTransform;
    private Transform centerTransform;
    
    // Start is called before the first frame update
    void Start()
    {        
        boidsTransform = transform.Find("Boids");
        centerTransform = transform.Find("Center");
        
        boundaries = aquarium.GetComponent<Renderer>().bounds;

        Vector3 swimSize = (boundaries.max - boundaries.min) * 0.9f;
        swimSize.x -= (swimSize.x * left + swimSize.x * right);
        swimSize.y -= (swimSize.y * top + swimSize.y * bottom);
        swimSize.z -= (swimSize.z * front + swimSize.z * back);
        Vector3 swimCenter = boundaries.center
            + Vector3.down * boundaries.size.x * top / 2
            + Vector3.up * boundaries.size.x * bottom / 2
            + Vector3.right * boundaries.size.x * left / 2
            + Vector3.left * boundaries.size.x * right / 2
            + Vector3.back * boundaries.size.x * front / 2
            + Vector3.forward * boundaries.size.x * back / 2;
        swimBounds = new Bounds(swimCenter, swimSize);

        swimLimits = (swimBounds.max - swimBounds.min) * 0.45f;

        allFish = new GameObject[numberOfFish];
        for (int i = 0; i < numberOfFish; i++)
        {
            float3 fishPos = math.float3(
                Random.Range(-swimLimits.x, swimLimits.x),
                Random.Range(-swimLimits.y, swimLimits.y),
                Random.Range(-swimLimits.z, swimLimits.z)
            ) + math.float3(boundaries.center);
            
            var fish = Instantiate(fishGO);
            fish.transform.position = fishPos;
            fish.transform.SetParent(boidsTransform, true);
            allFish[i] = fishGO;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = Vector3.zero;
        foreach (Transform fish in boidsTransform)
        {
            pos += fish.position;
        }
        centerTransform.SetPositionAndRotation(pos / allFish.Length, Quaternion.identity);
    }
}
