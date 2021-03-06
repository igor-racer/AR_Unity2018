﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif
using OpenCVForUnity;

namespace MarkerLessARExample
{
    /// <summary>
    /// Pattern capture.
    /// </summary>
    [RequireComponent(typeof(WebCamTextureToMatHelper))]
    public class CapturePattern : MonoBehaviour
    {


        /// <summary>
        /// The texture.
        /// </summary>
        Texture2D texture;

        /// <summary>
        /// The web cam texture to mat helper.
        /// </summary>
        WebCamTextureToMatHelper webCamTextureToMatHelper;

        /// <summary>
        /// The pattern rect.
        /// </summary>
        OpenCVForUnity.Rect patternRect;

        /// <summary>
        /// The rgb mat.
        /// </summary>
        Mat rgbMat;

        /// <summary>
        /// The pattern raw image.
        /// </summary>
        public RawImage patternRawImage;

        /// <summary>
        /// The detector.
        /// </summary>
        ORB detector;

        /// <summary>
        /// The keypoints.
        /// </summary>
        MatOfKeyPoint keypoints;

        // Use this for initialization
        void Start ()
        {
//            Utils.setDebugMode(true);

            using (Mat patternMat = Imgcodecs.imread (Application.persistentDataPath + "/patternImg.jpg")) {
                if (patternMat.total () == 0) {
                
                    patternRawImage.gameObject.SetActive (false);

                } else {
                
                    Imgproc.cvtColor (patternMat, patternMat, Imgproc.COLOR_BGR2RGB);
                
                    Texture2D patternTexture = new Texture2D (patternMat.width (), patternMat.height (), TextureFormat.RGBA32, false);

                    Utils.matToTexture2D (patternMat, patternTexture);

                    patternRawImage.texture = patternTexture;
                    patternRawImage.rectTransform.localScale = new Vector3 (1.0f, (float)patternMat.height () / (float)patternMat.width (), 1.0f);

                    patternRawImage.gameObject.SetActive (true);
                }
            }
            
            webCamTextureToMatHelper = gameObject.GetComponent<WebCamTextureToMatHelper> ();
            webCamTextureToMatHelper.Init ();


            detector = ORB.create ();
            detector.setMaxFeatures (1000);
            keypoints = new MatOfKeyPoint ();
        }

        /// <summary>
        /// Raises the web cam texture to mat helper inited event.
        /// </summary>
        public void OnWebCamTextureToMatHelperInited ()
        {
            Debug.Log ("OnWebCamTextureToMatHelperInited");


            Mat webCamTextureMat = webCamTextureToMatHelper.GetMat ();
                    
            texture = new Texture2D (webCamTextureMat.width (), webCamTextureMat.height (), TextureFormat.RGBA32, false);

            rgbMat = new Mat (webCamTextureMat.rows (), webCamTextureMat.cols (), CvType.CV_8UC3);
                    

                    
            gameObject.transform.localScale = new Vector3 (webCamTextureMat.width (), webCamTextureMat.height (), 1);
            
            Debug.Log ("Screen.width " + Screen.width + " Screen.height " + Screen.height + " Screen.orientation " + Screen.orientation);

                    
            float width = webCamTextureMat.width ();
            float height = webCamTextureMat.height ();

            float widthScale = (float)Screen.width / width;
            float heightScale = (float)Screen.height / height;
            if (widthScale < heightScale) {
                Camera.main.orthographicSize = (width * (float)Screen.height / (float)Screen.width) / 2;
            } else {
                Camera.main.orthographicSize = height / 2;
            }
                        
            gameObject.GetComponent<Renderer> ().material.mainTexture = texture;
        
                        


            //if WebCamera is frontFaceing,flip Mat.
            if (webCamTextureToMatHelper.GetWebCamDevice ().isFrontFacing) {
                webCamTextureToMatHelper.flipHorizontal = true;
            }               


            int patternWidth = (int)(Mathf.Min (webCamTextureMat.width (), webCamTextureMat.height ()) * 0.8f);

            patternRect = new OpenCVForUnity.Rect (webCamTextureMat.width () / 2 - patternWidth / 2, webCamTextureMat.height () / 2 - patternWidth / 2, patternWidth, patternWidth);

        }

        /// <summary>
        /// Raises the web cam texture to mat helper disposed event.
        /// </summary>
        public void OnWebCamTextureToMatHelperDisposed ()
        {
            Debug.Log ("OnWebCamTextureToMatHelperDisposed");

            if (rgbMat != null) {
                rgbMat.Dispose ();
            }
                        
        }

        /// <summary>
        /// Raises the web cam texture to mat helper error occurred event.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        public void OnWebCamTextureToMatHelperErrorOccurred (WebCamTextureToMatHelper.ErrorCode errorCode)
        {
            Debug.Log ("OnWebCamTextureToMatHelperErrorOccurred " + errorCode);
        }
        
        // Update is called once per frame
        void Update ()
        {
            if (webCamTextureToMatHelper.IsPlaying () && webCamTextureToMatHelper.DidUpdateThisFrame ()) {
                
                Mat rgbaMat = webCamTextureToMatHelper.GetMat ();

                Imgproc.cvtColor (rgbaMat, rgbMat, Imgproc.COLOR_RGBA2RGB);


                detector.detect (rgbaMat, keypoints);
//                Debug.Log ("keypoints.ToString() " + keypoints.ToString());
                Features2d.drawKeypoints (rgbMat, keypoints, rgbaMat, Scalar.all (-1), Features2d.NOT_DRAW_SINGLE_POINTS);



                Imgproc.rectangle (rgbaMat, patternRect.tl (), patternRect.br (), new Scalar (255, 0, 0, 255), 5);
                            

                Utils.matToTexture2D (rgbaMat, texture, webCamTextureToMatHelper.GetBufferColors ());
                
            }
            
        }

        void OnDisable ()
        {
            webCamTextureToMatHelper.Dispose ();

            detector.Dispose ();
            if (keypoints != null)
                keypoints.Dispose ();

//            Utils.setDebugMode(false);
        }


        /// <summary>
        /// Raises the back button event.
        /// </summary>
        public void OnBackButton ()
        {
            #if UNITY_5_3 || UNITY_5_3_OR_NEWER
            SceneManager.LoadScene ("MarkerLessARExample");
            #else
            Application.LoadLevel ("MarkerLessARExample");
            #endif
        }
        
        /// <summary>
        /// Raises the play button event.
        /// </summary>
        public void OnPlayButton ()
        {
            webCamTextureToMatHelper.Play ();
        }
        
        /// <summary>
        /// Raises the pause button event.
        /// </summary>
        public void OnPauseButton ()
        {
            webCamTextureToMatHelper.Pause ();
        }
        
        /// <summary>
        /// Raises the stop button event.
        /// </summary>
        public void OnStopButton ()
        {
            webCamTextureToMatHelper.Stop ();
        }
        
        /// <summary>
        /// Raises the change camera button event.
        /// </summary>
        public void OnChangeCameraButton ()
        {
            webCamTextureToMatHelper.Init (null, webCamTextureToMatHelper.requestWidth, webCamTextureToMatHelper.requestHeight, !webCamTextureToMatHelper.requestIsFrontFacing);
        }

        /// <summary>
        /// Raises the capture button event.
        /// </summary>
        public void OnCaptureButton ()
        {

            Mat patternMat = new Mat (rgbMat, patternRect);

            Texture2D patternTexture = new Texture2D (patternMat.width (), patternMat.height (), TextureFormat.RGBA32, false);
            
            Utils.matToTexture2D (patternMat, patternTexture);
            
            patternRawImage.texture = patternTexture;

            patternRawImage.gameObject.SetActive (true);

        }

        /// <summary>
        /// Raises the save button event.
        /// </summary>
        public void OnSaveButton ()
        {
            if (patternRawImage.texture != null) {
                Texture2D patternTexture = (Texture2D)patternRawImage.texture;
                Mat patternMat = new Mat (patternRect.size (), CvType.CV_8UC3);
                Utils.texture2DToMat (patternTexture, patternMat);
                Imgproc.cvtColor (patternMat, patternMat, Imgproc.COLOR_RGB2BGR);

                string savePath = Application.persistentDataPath;
                Debug.Log ("savePath " + savePath);
            
                Imgcodecs.imwrite (savePath + "/patternImg.jpg", patternMat);
            
                #if UNITY_5_3 || UNITY_5_3_OR_NEWER
                SceneManager.LoadScene ("WebCamTextureMarkerLessARExample");
                #else
                Application.LoadLevel ("WebCamTextureMarkerLessARExample");
                #endif
            }
        }
    }
    
}
