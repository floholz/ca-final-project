using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class QuaternionScript : MonoBehaviour
{
    public Transform target;

    private float3 targetPos;
    private quaternion targetRot;

    private float3 myPos;
    private quaternion myRot;


    // Start is called before the first frame update
    void Start()
    {
        getTargetPosAndRot();
        getMyPosAndRot();
        updateLookAt();
        transform.SetPositionAndRotation(myPos, myRot);
    }

    // Update is called once per frame
    void Update()
    {
        getTargetPosAndRot();
        getMyPosAndRot();

        updateLookAt();
        transform.SetPositionAndRotation(myPos, myRot);        
    }

    private void updateLookAt()
    {
        float3 direction = targetPos - myPos;
        float3 up = math.float3(0, 1, 0);
        myRot = quaternion.LookRotationSafe(direction, up);

        Debug.DrawLine(myPos, myPos + direction, Color.cyan);
    }

    private void getTargetPosAndRot()
    {
        targetPos = target.position;
        targetRot = target.rotation;
    }
    private void getMyPosAndRot()
    {
        myPos = transform.position;
        myRot = transform.rotation;
    }
}
