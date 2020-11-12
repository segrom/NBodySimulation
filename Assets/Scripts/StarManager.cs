using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityTemplateProjects;
using Random = UnityEngine.Random;

[Serializable]
public struct Star
{
    public float X;
    public float Y;
    public float Z;
    public float Vx;
    public float Vy;
    public float Vz;
    public float Ax;
    public float Ay;
    public float Az;
    public float Radius;
    public float Mass;
}


public class StarManager : MonoBehaviour
{
    [SerializeField] private List<Material> starClassMats;
    [SerializeField] private Material blackHoleMat;
    [SerializeField]private ComputeShader gravityShader;
    [SerializeField]private StarObject starPref;
    private ComputeBuffer _starsBuffer;
    [SerializeField]private Star[] stars;
    private StarObject[] _starsObjects;
    [SerializeField]private int[] Counter;
    private const int GroupsCount = 3; 
    private const int GroupsSize = 3; 
    private const int KernelCounts = 32; 
    private const int KernelSize = 32;
    private const int centerCount = 200;
    private int _accelerationKernel,_moveKernel;
    

    private void Start(){
        int starCount = GroupsCount * GroupsSize * KernelCounts * KernelSize;
        stars = new Star[starCount];
        _starsObjects = new StarObject[starCount];
        for (int i = 0; i < starCount-centerCount; i++){
            CreateStar(i,100,0.2f);
        }
        for (int i = 0; i < centerCount; i++){
            CreateStar(starCount-centerCount+i,10, 0.002f);
        }

        _accelerationKernel=  gravityShader.FindKernel("k_star_acceleration");

        _starsBuffer = new ComputeBuffer(stars.Length,44,ComputeBufferType.Structured);
        
        gravityShader.SetBuffer(_accelerationKernel,"starsBuffer",_starsBuffer);
        gravityShader.SetInt("_kernel_counts",KernelCounts);
        gravityShader.SetInt("_kernel_size",KernelSize);
        gravityShader.SetInt("_groups_count",GroupsCount);
        gravityShader.SetInt("_groups_size",GroupsSize);
        
        _starsBuffer.SetData(stars);
    }

    private void Update(){
        _starsBuffer.SetData(stars);

        gravityShader.Dispatch(_accelerationKernel,GroupsCount,GroupsSize,1);

        _starsBuffer.GetData(stars);
        Counter = new int[Enum.GetNames(typeof(SpectralClass)).Length];
        for (int i = 0; i < stars.Length; i++){
            var s = stars[i];
            _starsObjects[i].transform.position = new Vector3(s.X,s.Y,s.Z);
            _starsObjects[i].options = stars[i];
            Counter[(int) _starsObjects[i].spectralClass]++;
        }
    }

    ~StarManager(){
        _starsBuffer.Release();
    }

    StarObject CreateStar(int i, float creationRadius, float velocityRange){
        var starObj = Instantiate(starPref,Vector3.zero, Quaternion.identity);
        _starsObjects[i] = starObj;
        starObj.Initialization();
        if (starObj.isBlackHole){
            starObj.name = $"Black Hole ({i})";
            starObj.GetComponent<MeshRenderer>().material = blackHoleMat;
            var d =20;
            starObj.transform.localScale =new Vector3(d,d,d);
            starObj.GetComponent<LODGroup>().enabled = false;
            starObj.lodSprite.gameObject.SetActive(false);
        }
        else{
            starObj.name = $"Star {starObj.spectralClass}{starObj.luminosityClass} ({i})";
            starObj.GetComponent<MeshRenderer>().material = starClassMats[(int) starObj.spectralClass];
            starObj.GetComponent<MeshRenderer>().material.SetFloat("_luminosity",(float)starObj.luminosity);
            var d = starObj.options.Radius * 2*5;
            starObj.transform.localScale =new Vector3(d,d,d);
            starObj.SetLodColor(starObj.GetComponent<MeshRenderer>().material.GetColor("_spector"));

        }
        stars[i] = starObj.options;
        stars[i].X = Random.Range(-creationRadius,creationRadius);
        stars[i].Y = Random.Range(-creationRadius,creationRadius);
        stars[i].Z = Random.Range(-creationRadius,creationRadius);
        stars[i].Vx = Random.Range(-velocityRange,velocityRange);
        stars[i].Vy =  Random.Range(-velocityRange,velocityRange);
        stars[i].Vz =  Random.Range(-velocityRange,velocityRange);
        stars[i].Ax = stars[i].Ay = stars[i].Az = 0;
        
        return starObj;
    }
}
