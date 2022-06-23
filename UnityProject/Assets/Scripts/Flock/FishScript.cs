using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishScript : MonoBehaviour
{
    private FishFlock flock;
    private float speed;

    // Use this for initialization
    void Start ()
    {
        flock = transform.GetComponentInParent<FishFlock>();
        speed = Random.Range(flock.minSpeed, flock.maxSpeed);
		
    }

    // Update is called once per frame
    void Update () {
        transform.Translate(0, 0, Time.deltaTime * speed);
        
        ApplyRules(Time.deltaTime);
        
        if (!flock.swimBounds.Contains(transform.position))
        {
            Ray ray = new Ray(
                transform.position,
                flock.swimBounds.center - transform.position
                );
            RaycastHit hit;
            Physics.Raycast(
                ray, 
                out hit,
                math.length(flock.swimLimits) * 3f
                );
            transform.position = hit.point;
        }
    }
    
    void ApplyRules(float dt)
    {
        float3 centerOfFlock = float3.zero;
        float3 avoid = float3.zero;
        float3 borderAvoid = float3.zero;
        float groupSpeed = 0.01f;
        float distance;
        int groupSize = 0;

        foreach (Transform fish in transform.parent) 
        {
            if(fish != transform)
            {
                distance = math.distance(fish.position,transform.position);
                if(distance <= flock.neighbourDistance)
                {
                    centerOfFlock += math.float3(fish.position);	
                    groupSize++;	
					
                    if(distance < 1.0f)		
                    {
                        avoid += math.float3(transform.position - fish.position);
                    }
					
                    FishScript anotherFlock = fish.GetComponent<FishScript>();
                    groupSpeed += anotherFlock.speed;
                }
            }
        }

        
		
        if(groupSize > 0)
        {
            centerOfFlock /= groupSize;
            speed = groupSpeed/groupSize;
			
            float3 direction = 
                centerOfFlock - math.float3(transform.position)
                + avoid 
                + borderAvoid;
            if (!direction.Equals(float3.zero))
            {
                transform.rotation = math.slerp(
                        transform.rotation, 
                        quaternion.LookRotationSafe(direction, math.float3(0, 1, 0)), 
                        flock.rotationSpeed * dt
                    );
            }
        }
    }
}
