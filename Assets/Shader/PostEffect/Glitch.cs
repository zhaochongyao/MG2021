using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glitch : PostEffectBase
{
    public float Distance = 0.03f ;
    public float RGBSpeed = 50 ;
    public float ScanInt = 0.02f ;
    public float ScanDis = 0.3f ;
    public float u_scan = 1 ;
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
            _Material.SetFloat("_Distance",Distance);
            _Material.SetFloat("_RGBSpeed",RGBSpeed);
            _Material.SetFloat("_ScanInt",ScanInt);
            _Material.SetFloat("_ScanDis",ScanDis);
            _Material.SetFloat("_u_sca",u_scan);
            _Material.SetFloat("_Time01",Time01);
            Graphics.Blit(src,dest,_Material);
        }
        else
        {
            Graphics.Blit(src,dest);
        }

        
    }
}
