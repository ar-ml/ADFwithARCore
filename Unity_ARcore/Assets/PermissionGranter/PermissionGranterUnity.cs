using UnityEngine;
using System.Collections;
using System;
using SimpleJSON;

namespace PartaGames {
    namespace Android {

        public class PermissionGranterUnity : MonoBehaviour {

            // subscribe to this callback to see if your permission was granted.
            public static Action<string, bool> PermissionRequestCallback;

            private const string PERMISSION_GRANTER_GAMEOBJECT_NAME = "PermissionGranterUnity";
            private const string PERMISSION_GRANTED = "PERMISSION_GRANTED";
            private const string PERMISSION_DENIED = "PERMISSION_DENIED";

            private static PermissionGranterUnity instance;
            private static bool initialized = false;

            private static AndroidJavaObject currentActivity;
            private static AndroidJavaObject permissionGranter;

            public void Awake()
            {
                // instance is also set in initialize.
                // having it here ensures this thing doesn't break
                // if you added this component to the scene manually
                instance = this;
                DontDestroyOnLoad(this.gameObject);

                // object name must match UnitySendMessage call in native side
                if (name != PERMISSION_GRANTER_GAMEOBJECT_NAME)
                    name = PERMISSION_GRANTER_GAMEOBJECT_NAME;
            }

            private static void initialize()
            {
                // runs once when you call GrantPermission

                // add object to scene
                if (instance == null)
                {
                    GameObject go = new GameObject();
                    // instance will also be set in awake, but having it here as well seems extra safe
                    instance = go.AddComponent<PermissionGranterUnity>();
                    // object name must match UnitySendMessage call in native side
                    go.name = PERMISSION_GRANTER_GAMEOBJECT_NAME;
                }

                // get the jni stuff. we need the activity class and the PermissionGranterUnity class.
                AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                permissionGranter = new AndroidJavaObject("com.partagames.unity.permissiongranter.unity.PermissionGranterUnity");

                initialized = true;
            }

            public static bool ShouldShowRequestPermissionRationale(string permission) {
                #if !UNITY_ANDROID
                Debug.LogWarning("PermissionGranter supports only Android devices.");
                return false;
                #endif
                #if UNITY_ANDROID
                if (!initialized) {
                    initialize();
                }

                return permissionGranter.Call<bool>("shouldShowRequestPermissionRationale", currentActivity, permission);
                #endif
            }

            public static bool IsPermissionGranted(string permission)
            {
#if !UNITY_ANDROID
                Debug.LogWarning("PermissionGranter supports only Android devices.");
                return false;
#endif

#if UNITY_ANDROID
                if (!initialized)
                    initialize();

                return permissionGranter.Call<bool>("isPermissionGranted", currentActivity, permission);
#endif
            }

            public static void GrantPermission(string permission, Action<string, bool> callback)
            {
#if !UNITY_ANDROID
                Debug.LogWarning("PermissionGranter supports only Android devices.");
                return;
#endif
#if UNITY_ANDROID
                if (!initialized)
                    initialize();

                PermissionRequestCallback = callback;

                permissionGranter.Call("grantPermission", currentActivity, permission);
#endif
            }

            public static void GrantPermissions(string[] permissions, Action<string, bool> callback) {
#if !UNITY_ANDROID
                Debug.LogWarning("PermissionGranter supports only Android devices.");
                return;
#endif
#if UNITY_ANDROID
                if (!initialized)
                    initialize();

                PermissionRequestCallback = callback;

                permissionGranter.Call("grantPermissions", currentActivity, permissions);
#endif
            }

            private void permissionRequestCallbackInternal(string message)
            {
                
                // were calling this method from the java side.
                // the method name and gameobject must match PermissionGranterUnity.java's UnitySendMessage
                
                Debug.Log(message);
                var json = JSONNode.Parse(message);
                string permission = json["permission"];
                string grantResult = json["grantResult"];

                bool granted = false;
                if (grantResult == PERMISSION_GRANTED) {
                    granted = true;
                }

                if (PermissionRequestCallback != null) {
                    PermissionRequestCallback(permission, granted);
                }
            }

        }

    }
}
