Android Permission Granter Plugin by Parta Games

#### Unity3D Android project requirements and settings ####

* Unity version 5.4.3p4 or later
* Minimum Android SDK level: 13 (3.2 Honeycomb)
* Target Android SDK level: 23 (6.0 Marhsmallow)

#### Getting Started ####

1) Import all the files from PermisisonGranter.unitypackage or the Asset Store
2) Go to "Build Settings" and switch platform to Android
3) Go to "Player Settings" and select the Android tab. Set the "Minimum API level" to "Android 3.2 'Honeycomb' (API Level 13)"
4) Use the provided AndrodManifest.xml or copy the contents to your existing manifest
5) Add tag <uses-permission android:name="android.permission.PERMISSION_TO_BE_GRANTED" /> to AndroidManifest.xml with the proper android permission name (like android.permission.WRITE_EXTERNAL_STORAGE)
6) Create a C# script and call PermissionGranter.IsPermissionGranted(...) or PermissionGranter.GrantPermission(...) methods according to your UI flow

Note: Only "dangerous" permissions will be prompted. If the permission is not classified as "dangerous" by the Android SDK the dialog will not appear.

New in version 1.1.0:
- Added support for granting multiple permissions with one method call: PermissionGranter.GrantPermissions(...)

Copyright 2016 Parta Games Oy