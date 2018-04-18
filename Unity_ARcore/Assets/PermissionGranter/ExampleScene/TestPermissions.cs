using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PartaGames.Android;

public class TestPermissions : MonoBehaviour
{

    public Text text;

    private static readonly string WRITE_EXTERNAL_STORAGE = "android.permission.WRITE_EXTERNAL_STORAGE";
    private static readonly string CAMERA = "android.permission.CAMERA";

    public void CheckShouldShowRequestPermissionRationale() {
        text.text += "\n Should show request permission rationale: " + PermissionGranterUnity.ShouldShowRequestPermissionRationale(WRITE_EXTERNAL_STORAGE);
    } 

    public void CheckStorage()
    {
        text.text += "\nCheck permission: WRITE_EXTERNAL_STORAGE" + ": " + (PermissionGranterUnity.IsPermissionGranted(WRITE_EXTERNAL_STORAGE) ? "Yes" : "No");
    }

    public void CheckCamera()
    {
        text.text += "\nCheck permission: CAMERA" + ": " + (PermissionGranterUnity.IsPermissionGranted(CAMERA) ? "Yes" : "No");
    }

    public void GrantStorage()
    {
        PermissionGranterUnity.GrantPermission(WRITE_EXTERNAL_STORAGE, PermissionGrantedCallback);
    }

    public void GrantCamera()
    {
        PermissionGranterUnity.GrantPermission(CAMERA, PermissionGrantedCallback);
    }

    public void GrantMultiplePerm()
    {
        PermissionGranterUnity.GrantPermissions(new string[] {WRITE_EXTERNAL_STORAGE, CAMERA}, PermissionGrantedCallback);
    }

    // this is the callback that will be called after user has decided to grant or deny the permission(s)
    // will be called once per permission
    public void PermissionGrantedCallback(string permission, bool isGranted)
    {
        text.text += "\nPermission granted: " + permission + ": " + (isGranted ? "Yes" : "No");
    }
}
