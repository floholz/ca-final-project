using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BoidScript : MonoBehaviour
{
    [FormerlySerializedAs("movement")] 
    public float3 _movement;
    [FormerlySerializedAs("separation")] 
    public float3 _separation;
    [FormerlySerializedAs("alignment")] 
    public float3 _alignment;
    [FormerlySerializedAs("cohesion")] 
    public float3 _cohesion;
    [FormerlySerializedAs("collide")] 
    public float3 _collide;
    

    [FormerlySerializedAs("velocity")] 
    public float3 _velocity;
    
    private FishFlock flock;
    
    // Start is called before the first frame update
    void Start()
    {
        flock = transform.GetComponentInParent<FishFlock>();
        _velocity = float3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;

        _movement = Movement();
        _alignment = Alignment();
        _cohesion = Cohesion();
        _separation = Separation();
        _collide = Collision();

        _velocity += _movement + _alignment + _cohesion + _separation + _collide;
        //Collide();
        _velocity *= dt;
        
        float3 pos = math.float3(transform.position) + _velocity;
        quaternion rot  = math.slerp(
                transform.rotation, 
                quaternion.LookRotationSafe(_velocity, math.float3(0, 1, 0)), 
                flock.rotationSpeed * dt
            );
        
        transform.SetPositionAndRotation(pos, rot);
        // transform.Translate(0, 0, Random.Range(flock.minSpeed, flock.maxSpeed));
    }

    private float3 Movement()
    {
        if (!flock.useMovement) return float3.zero;
        return 
            math.normalize(transform.forward) 
            * flock.weightMovement 
            * Random.Range(flock.minSpeed, flock.maxSpeed);
    }

    private float3 Separation()
    {
        if (!flock.useSeparation) return float3.zero;
        float3 separate = float3.zero;
        
        foreach (Transform fish in transform.parent)
        {
            if (fish != transform)
            {
                float distance = math.distance(fish.position, transform.position);
                if (distance < flock.separationDist)
                {
                    separate += math.float3(transform.position - fish.position);
                }
            }
        }
        return separate * flock.weightSeparation;
    } 
    
    private float3 Alignment()
    {
        if (!flock.useAlignment) return float3.zero;
        float3 alignment = float3.zero;

        foreach (Transform fish in transform.parent)
        {
            if (fish != transform)
            {
                alignment += fish.GetComponent<BoidScript>()._velocity;
            }
        }

        alignment /= flock.numberOfFish - 1;
        
        return (alignment - _velocity) * flock.weightCohesion;
    } 
    
    private float3 Cohesion()
    {
        if (!flock.useCohesion) return float3.zero;
        float3 cohesion = float3.zero;

        foreach (Transform fish in transform.parent)
        {
            if (fish != transform)
            {
                cohesion += math.float3(fish.position);
            }
        }

        cohesion /= flock.numberOfFish - 1;
        
        return (cohesion - math.float3(transform.position)) * flock.weightCohesion;
    }

    private float3 Collision()
    {
        if (!flock.useCollision) return float3.zero;
        float3 avoid = float3.zero;
        if (!flock.swimBounds.Contains(transform.position))
        {
            _velocity = (flock.swimBounds.center - transform.position);
        }
        return 
            avoid
            * flock.weightCollision
            * flock.boundaryAvoid;
    }

    private void Collide()
    {
        if (!flock.useCollision) return;
        if (!flock.swimBounds.Contains(transform.position))
        {
            _velocity = math.reflect(_velocity, flock.swimBounds.center - transform.position);
        }
    }
}
