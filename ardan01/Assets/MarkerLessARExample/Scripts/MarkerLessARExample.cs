﻿using UnityEngine;
using System.Collections;

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

namespace MarkerLessARExample
{
    public class MarkerLessARExample : MonoBehaviour
    {
        
        // Use this for initialization
        void Start ()
        {
            
        }
        
        // Update is called once per frame
        void Update ()
        {
            
        }

        public void OnShowLicenseButton ()
        {
            #if UNITY_5_3 || UNITY_5_3_OR_NEWER
            SceneManager.LoadScene ("ShowLicense");
            #else
            Application.LoadLevel ("ShowLicense");
            #endif
        }
        
        public void Texture2DMarkerLessARExample ()
        {
            #if UNITY_5_3 || UNITY_5_3_OR_NEWER
            SceneManager.LoadScene ("Texture2DMarkerLessARExample");
            #else
            Application.LoadLevel ("Texture2DMarkerLessARExample");
            #endif
        }
        
        public void WebCamTextureMarkerLessARExample ()
        {
            #if UNITY_5_3 || UNITY_5_3_OR_NEWER
            SceneManager.LoadScene ("WebCamTextureMarkerLessARExample");
            #else
            Application.LoadLevel ("WebCamTextureMarkerLessARExample");
            #endif
        }
    }
}