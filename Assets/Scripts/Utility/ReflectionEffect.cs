using System;
using System.Collections;
using UnityEngine;

public class ReflectionEffect : MonoBehaviour
{
    public Texture2D reflectTex;


    private void Start()
    {
        Shader.SetGlobalTexture("_ReflectionTex", reflectTex);
    }

    private void OnValidate()
    {
        Shader.SetGlobalTexture("_ReflectionTex", reflectTex);
    }

    private void OnEnable()
    {
        Shader.EnableKeyword("cubeMap_off");
    }

    private void OnDisable()
    {
        
        Shader.DisableKeyword("cubeMap_off");
    }
}