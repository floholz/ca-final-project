using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalInputController : MonoBehaviour
{
    private GlobalAnimationInfo globalAnimationInfo;
    private bool ERROR = false;

    // Start is called before the first frame update
    void Start()
    {
        globalAnimationInfo = GetComponent<GlobalAnimationInfo>();
        if (globalAnimationInfo == null)
        {
            ERROR = true;
            Debug.LogError("Error in Settup. 'Animation Target' needs to be set!");
            return;
        }
    }

    public void OnAny()
    {
        if (ERROR) return;
        if (
            globalAnimationInfo.animationStatus == eAnimationStatus.ENDED ||
            globalAnimationInfo.animationStatus == eAnimationStatus.STOPPED )
        {
            Application.Quit();
            Debug.Log("Application.Quit()");
        }
    }

}
