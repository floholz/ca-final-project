using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleWipeShaderController : MonoBehaviour
{
    public Shader shader;
    [Range(0, 1)]
    public float radius = 0;

    private Material material;

    // Start is called before the first frame update
    void Start()
    {
        material = new Material(shader);
        float aspactRatio = GetComponentInParent<Camera>().aspect;
        material.SetFloat("_Aspact", aspactRatio);
    }

    private void Update()
    {
        material.SetFloat("_Radius", radius);
    }

    // Update is called once per frame
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, material);
    }

    public void SetRadius(float radius)
    {
        this.radius = radius;
    }
}
