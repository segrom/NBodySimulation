using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityTemplateProjects
{
    
    public enum SpectralClass //Basic (Harvard) Spectral Classification
    {
        W=0,O=1,B=2,A=3,F=4,G=5,K=6,M=7,L=8,T=9,Y=10
    }

    public enum LuminosityClass
    {
        O=0,Iap=1,Ia=2,Ib=3,II=4,III=5,IV=6,V=7,VI=8,VII=9
    }

    public class StarObject : MonoBehaviour
    {
        private const double sigma= 5.67e-8d;//Stefan-Boltzmann Constant
        public Star options;
        public SpriteRenderer lodSprite;
        public double temperature;
        public double  luminosity;
        public bool isBlackHole = false;
        public SpectralClass spectralClass{ get; private set; }
        public LuminosityClass luminosityClass{ get; private set; }

        
        public void Initialization(){
            if (Random.Range(0f, 100f) > 99.9999f){
                isBlackHole = true;
                luminosity = 0;
                temperature = 0;
                options.Radius = 120;
                options.Mass = 4000000;
                return;
            }
            if (Random.Range(0, 100) > 10){
                luminosityClass = (LuminosityClass)Random.Range(7,9);
                int sc = Random.Range(0, 10) < 2 ? Random.Range(2, 9) : Random.Range(4, 7);
                spectralClass = (SpectralClass)sc;
            }
            else{
                if (Random.Range(0, 100) > 10){
                    luminosityClass = (LuminosityClass)Random.Range(4,6);
                    int sc = Random.Range(0, 10) < 2 ? Random.Range(2, 9) : Random.Range(5, 9);
                    spectralClass = (SpectralClass)sc;
                }
                else if (Random.Range(0, 100) > 30){
                    luminosityClass = (LuminosityClass)Random.Range(2,4);
                    spectralClass = (SpectralClass) Random.Range(1, 9);
                }
                else if (Random.Range(0, 100) > 20){
                    luminosityClass = (LuminosityClass)Random.Range(7,10);
                    if(luminosityClass==LuminosityClass.VII) spectralClass = (SpectralClass) Random.Range(1, 5);
                    else spectralClass = (SpectralClass) Random.Range(7, 11);
                }
                else{
                    luminosityClass = (LuminosityClass)Random.Range(0,2);
                    spectralClass = (SpectralClass) Random.Range(2, 8);
                }
            }
            luminosity = SetLuminosityByClass();
            temperature = SetTemperatureByClass();
            options.Radius =(float) Math.Sqrt(luminosity / (4 * Math.PI * sigma * Math.Pow(temperature, 4)));
            options.Mass = GetMassFromLum();
        }

        private float GetMassFromLum(){
            if (luminosity < 0.0330145) return (float)Math.Pow(luminosity / 0.23d, 0.44444d);
            else if (luminosity <16) return (float)Math.Pow(luminosity , 0.25d);
            else if (luminosity < 54000) return (float)Math.Pow(luminosity/1.5, 0.22222d);
            else return (float)(luminosity/3200);
        }

        private double SetTemperatureByClass(){
            switch (spectralClass){
                case SpectralClass.W: return Random.Range(40000, 70000);
                case SpectralClass.O: return Random.Range(25000, 40000);
                case SpectralClass.B: return Random.Range(10000, 25000);
                case SpectralClass.A: return Random.Range(7000, 10000);
                case SpectralClass.F: return Random.Range(6000, 7000);
                case SpectralClass.G: return Random.Range(5000, 6000);
                case SpectralClass.K: return Random.Range(3500, 5000);
                case SpectralClass.M: return Random.Range(2500, 3500);
                case SpectralClass.L: return Random.Range(1500, 2500);
                case SpectralClass.T: return Random.Range(700, 1500);
                case SpectralClass.Y: return Random.Range(500, 700);
                default: throw new Exception($"Unknown star spectral class {spectralClass}");
            }
        }

        private double SetLuminosityByClass(){
            switch (luminosityClass){
                case LuminosityClass.O: return Random.Range(100000.0f,120000.0f);
                case LuminosityClass.Iap: return Random.Range(70000f,100000f);
                case LuminosityClass.Ia: return Random.Range(30000.0f,70000.0f);
                case LuminosityClass.Ib: return Random.Range(7000f,30000f);
                case LuminosityClass.II: return Random.Range(1000f,7000f);
                case LuminosityClass.III: return Random.Range(90f,1000f);
                case LuminosityClass.IV: 
                    if((int)spectralClass<3) return Random.Range(100f,600f);
                    return Random.Range(10,50);
                case LuminosityClass.V: 
                    if((int)spectralClass<3) return Random.Range(10f,500f);
                    if((int)spectralClass<4) return Random.Range(1f,10f);
                    if((int)spectralClass<6) return Random.Range(0.1f,1);
                    if((int)spectralClass<7)return Random.Range(0.001f,0.1f);
                    return Random.Range(0.00001f,0.001f);
                case LuminosityClass.VI: 
                    if((int)spectralClass<3) return Random.Range(1,100);
                    if((int)spectralClass<4) return Random.Range(0.1f,1f);
                    if((int)spectralClass<9) return Random.Range(0.01f,0.1f);
                    return Random.Range(0.0001f,0.01f);
                case LuminosityClass.VII: return Random.Range(0.0001f,0.01f);
                default: throw new Exception($"Unknown star luminosity class {luminosityClass}");
            }
        }

        public void SetLodColor(Color color){
            lodSprite.material.SetColor("_BaseColor", color);
            float normal = Mathf.InverseLerp(0.001f, 130000f, (float)luminosity);
            float normalLum = Mathf.Lerp(1f, 2f, normal);
            lodSprite.size = new Vector2(normalLum,normalLum);
        }
    }
}