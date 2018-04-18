using GoogleARCore;
using GoogleARCore.TextureReader;
using System;
using System.IO;
using UnityEngine;

namespace ARaction
{
    public class ADF2Wrapper : MonoBehaviour
    {
        public Camera arCamera;
        public GameObject arTransform;
        public TextureReader textureReaderComponent;

        //OpenCV feature points matching accuracy (100=very high accuracy, 300=low accuracy)
        private const int CFG_KEYPOINT_DISTANCE = 0;

        //Amount of feature points to be used (max 250)
        private const int CFG_KEYPOINT_LIMIT = 1;

        //Amount of matching feature points to say that the frame is matching
        private const int CFG_KEYPOINT_MATCHES = 2;

        //Index of the first frame to be processed (for cycle begin)
        private const int CFG_RANGE_FIRST = 3;

        //Index of the first frame to be processed (for cycle end)
        private const int CFG_RANGE_LAST = 4;

        //Surrounding around point in meters to be used (without using CFG_SURR_ENABLED is it useless)
        private const int CFG_SURR_DISTANCE = 5;

        //Enabling of surrounding filter (only frames around point will be processed if value is 1 or bigger)
        private const int CFG_SURR_ENABLED = 6;

        //Surrounding point position coordinate X (stored area coordinate system)
        private const int CFG_SURR_X = 7;

        //Surrounding point position coordinate Y (stored area coordinate system)
        private const int CFG_SURR_Y = 8;

        //Surrounding point position coordinate Z (stored area coordinate system)
        private const int CFG_SURR_Z = 9;

        //Aspect between error from ADF2 relocalisation and meters
        private const float DRIFT_PER_ERROR = 1;

        //Amount of drift for 1 meter movement from coordinate system origin
        private const float DRIFT_PER_METER = 0.01f;

        //UV coordinates of the frame used for relocalisation
        private const int OUT_FRAME_CALIBRATION = 0;

        //amount of matching frames found
        private const int OUT_MATCHES_COUNT = 1;

        //error of the returned pose
        private const int OUT_POSE_ERROR = 2;

        //filename of the frame used for relocalisation
        private const int OUT_POSE_FRAME = 3;

        //relocalised pose coordinate X
        private const int OUT_POSE_X = 4;

        //relocalised pose coordinate Y
        private const int OUT_POSE_Y = 5;

        //relocalised pose coordinate Z
        private const int OUT_POSE_Z = 6;

        private AndroidJavaObject androidPlugin = null;
        private float error = 0;
        private Mode mode;
        private Vector3 offset;
        private int pointCount;
        private Vector3[] points = new Vector3[61440];
        private int pose = 0;
        private int size = 0;
        private Texture2D textureToRender = null;
        private float tolerancy = 10;

        public enum Mode { LEARNING, RELOCALISATION, DUMMY }

        public Mode ADFMode
        {
            get
            {
                return mode;
            }
            set
            {
                mode = value;
                DatasetRestore();
                if (mode == Mode.LEARNING)
                {
                    error = 0;
                    offset = Vector3.zero;
                    pose = size + 1;
                }
                if (mode == Mode.RELOCALISATION)
                {
                    error = 1000;
                    offset = Vector3.zero;
                }
            }
        }

        public float DriftTolerancy
        {
            get
            {
                return tolerancy;
            }
            set
            {
                tolerancy = value;
            }
        }

        public void DatasetReset()
        {
            pose = 0;
            size = 0;
            Directory.Delete(Application.persistentDataPath, true);
            Directory.CreateDirectory(Application.persistentDataPath);
        }

        public void DatasetRestore()
        {
            size = 0;
            while (true)
            {
                if (File.Exists(GetFileName(size + 1, ".dsc")))
                {
                    size++;
                }
                else
                {
                    break;
                }
            }
        }

        public int DatasetSize()
        {
            return size;
        }

        public string DebugInfo()
        {
            if (androidPlugin != null)
            {
                string output = "";
                output += "error: " + androidPlugin.CallStatic<string>("getData", OUT_POSE_ERROR) + "\n";
                output += "frame: " + androidPlugin.CallStatic<string>("getData", OUT_POSE_FRAME) + "\n";
                output += "matches: " + androidPlugin.CallStatic<string>("getData", OUT_MATCHES_COUNT) + "\n";
                output += "pose: ";
                output += androidPlugin.CallStatic<string>("getData", OUT_POSE_X) + " ";
                output += androidPlugin.CallStatic<string>("getData", OUT_POSE_Y) + " ";
                output += androidPlugin.CallStatic<string>("getData", OUT_POSE_Z) + "\n";
                output += "uv: " + androidPlugin.CallStatic<string>("getData", OUT_FRAME_CALIBRATION) + "\n";
                return output;
            }
            return "";
        }

        public float EstimateDrift()
        {
            return (arCamera.transform.position - offset).magnitude * DRIFT_PER_METER + error * DRIFT_PER_ERROR;
        }

        public void OnImageAvailable(TextureReaderApi.ImageFormatType format, int width, int height, IntPtr pixelBuffer, int bufferSize)
        {
            if (UpdatePointCloud() && (mode != Mode.DUMMY))
            {
                if (mode == Mode.RELOCALISATION)
                {
                    Relocalise(width, height, pixelBuffer, bufferSize);
                }
                if (mode == Mode.LEARNING)
                {
                    ProcessJPG(width, height, pixelBuffer, bufferSize);
                    ProcessTXT();
                    ProcessDSC();
                    size = pose;
                    pose++;
                }
            }
        }

        public void Start()
        {
            textureReaderComponent.OnImageAvailableCallback += OnImageAvailable;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            DatasetRestore();
            ADFMode = DatasetSize() > 0 ? Mode.RELOCALISATION : Mode.LEARNING;
        }

        private string GetFileName(int index, string extension)
        {
            string number = "" + index;
            while (number.Length < 8)
            {
                number = "0" + number;
            }
            return Application.persistentDataPath + "/" + number + extension;
        }

        private void InitPlugin()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                if (androidPlugin == null)
                {
                    androidPlugin = new AndroidJavaObject("com.araction.adf2.Wrapper");
                    androidPlugin.CallStatic("init", Application.persistentDataPath + "/");
                    androidPlugin.CallStatic("setConfig", CFG_KEYPOINT_DISTANCE, (float)150);
                    androidPlugin.CallStatic("setConfig", CFG_KEYPOINT_LIMIT, (float)250);
                    androidPlugin.CallStatic("setConfig", CFG_KEYPOINT_MATCHES, (float)3);
                }
            }
        }

        private void ProcessDSC()
        {
            InitPlugin();
            if (androidPlugin != null)
            {
                androidPlugin.CallStatic("createDsc", pose);
            }
        }

        private void ProcessJPG(int width, int height, IntPtr pixelBuffer, int bufferSize)
        {
            //init
            if (textureToRender == null)
            {
                textureToRender = new Texture2D(width, height, TextureFormat.RGBA32, false, false);
            }

            //set texture
            textureToRender.LoadRawTextureData(pixelBuffer, bufferSize);
            textureToRender.Apply();

            //save the image
            byte[] bytes = textureToRender.EncodeToJPG();
            File.WriteAllBytes(GetFileName(pose, ".jpg"), bytes);
        }

        private void ProcessTXT()
        {
            var uv = Frame.CameraImage.DisplayUvCoords;
            string output = "";
            output += arCamera.transform.position.x + " ";
            output += arCamera.transform.position.y + " ";
            output += arCamera.transform.position.z + " ";
            output += "\n" + uv.TopLeft.x + " " + uv.TopLeft.y;
            output += "\n" + uv.TopRight.x + " " + uv.TopRight.y;
            output += "\n" + uv.BottomLeft.x + " " + uv.BottomLeft.y;
            output += "\n" + uv.BottomRight.x + " " + uv.BottomRight.y;
            output += "\n" + pointCount + "\n";
            for (int i = 0; i < pointCount; i++)
            {
                Vector3 point = points[i];
                output += point.x + " ";
                output += point.y + " ";
                output += point.z + " ";
                point = arCamera.WorldToViewportPoint(point);
                point.y = 1.0f - point.y;
                output += point.x + " " + point.y + "\n";
            }
            File.WriteAllText(GetFileName(pose, ".txt"), output);
        }

        private void Relocalise(int width, int height, IntPtr pixelBuffer, int bufferSize)
        {
            InitPlugin();
            if (EstimateDrift() > tolerancy)
            {
                pose = 0;
                Vector3 position = arCamera.transform.position;
                ProcessJPG(width, height, pixelBuffer, bufferSize);
                ProcessTXT();
                if (androidPlugin != null)
                {
                    androidPlugin.CallStatic("setConfig", CFG_RANGE_FIRST, (float)1);
                    androidPlugin.CallStatic("setConfig", CFG_RANGE_LAST, (float)size);
                    androidPlugin.CallStatic("setConfig", CFG_SURR_ENABLED, (float)0);
                    androidPlugin.CallStatic("processDsc", 0);
                    int matches = int.Parse(androidPlugin.CallStatic<string>("getData", OUT_MATCHES_COUNT));
                    if (matches > 3)
                    {
                        Vector3 session;
                        session.x = float.Parse(androidPlugin.CallStatic<string>("getData", OUT_POSE_X));
                        session.y = float.Parse(androidPlugin.CallStatic<string>("getData", OUT_POSE_Y));
                        session.z = float.Parse(androidPlugin.CallStatic<string>("getData", OUT_POSE_Z));
                        error = float.Parse(androidPlugin.CallStatic<string>("getData", OUT_POSE_ERROR));
                        offset = position;
                        arTransform.transform.position = position - session;
                    }
                }
            }
        }

        private bool UpdatePointCloud()
        {
            if (Frame.PointCloud.IsUpdatedThisFrame)
            {
                for (int i = 0; i < Frame.PointCloud.PointCount; i++)
                {
                    points[i] = Frame.PointCloud.GetPoint(i);
                }
                if (Frame.PointCloud.PointCount > 0)
                {
                    pointCount = Frame.PointCloud.PointCount;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}