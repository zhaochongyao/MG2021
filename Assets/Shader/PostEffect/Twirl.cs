using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Twirl : PostEffectBase
{
    public float RotSpeed = 10;
    public float AlphaSpeed = 0.08f;
    public float Time01 = 0;

    void Start() {
        Time01 = 0;
    }
    void Update() {
        Time01 += Time.deltaTime;
    }
    void OnRenderImage(RenderTexture src, RenderTexture dest) {
        if(_Material)
        {
            _Material.SetFloat("_RotSpeed",RotSpeed);
            _Material.SetFloat("_AlphaSpeed",AlphaSpeed);
            _Material.SetFloat("_Time01",Time01);
            Graphics.Blit(src,dest,_Material);
        }
        else
        {
            Graphics.Blit(src,dest);
        }

        
    }
}
