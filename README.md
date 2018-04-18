# ar-action-adf
Research and development of own ADF system

Project compatibility: QT Creator (OpenCV tests on Linux only), Unity 2017.3.0f3

# For license information, please read license.txt

# NO WARRANTY, NO LIABILITY, NO SUPPORT!

# Background information
The initial version of this code has been created by ARaction GmbH, Germany, as a preparation for their R&D project "REPLACING TANGO ADF SYSTEM".
The actual R&D project may or may not be started by ARaction GmbH on basis of this preparation.

You can find more information on the goals of the R&D project here:
https://nc.ar-action.com/index.php/s/jkS3jE5ECy7pyqw

Helpful feedback is highly appreciated.

Contact and imprint ARaction GmbH: 
https://www.ar-action.com/contact/

# Idea
Having the orientation of scene coordinate aligned with world coordinate system brings the benefit that we do not have to relocalise whole camera pose. So it should be possible to convert coordinate system between different ADFs just by adding Vector3 offset. We captured every frame point cloud, camera position and camera bitmap. Camera bitmap is processed by OpenCV (generating feature points), after having bitmap processed we do not need bitmap anymore.

# Problems

* Amount data needed to processed during relocalisation

* IMU does not provide enough precise rotation (drifting increases by sin(rotation_error) * distance_from_origin)

* Triangulation (for us was better working returning the best matching camera position then calculating position)

# Instructions to solve the problem of data amount

1) Show on screen matching frame
During relocalisation there is in debug box "frame:" and path to the jpg filename (if the filename is the same like ADF2Wrapper::GetFileName(0, ".jpg") then it is camera frame -> ignore it). Show this frame on the screen (the jpg are raw data directly from sensor so it has incorrect rotation but thats ok, it is just for us for testing to see which frame was used as the match)

2) Optimalize area learning
Currently it is generating too much frames (when you do not move it generate a lot of frames), create there some threshold and store frames only when the camera moves (it could be when e.g. 1/4 of camera image changed). Frames are stored in ADF2Wrapper::OnImageAvailable.

3) Low accurate relocalisation
Tune parameters in ADF2Wrapper::InitPlugin to find position with accuracy +/- 5 meters in resonable time (max 1 second processing ADF2Wrapper::Relocalise) for dataset of about 1000 frames.

4) Process only a part of dataset per frame
By calling in ADF2Wrapper
m_AndroidPlugin.CallStatic("setConfig", CFG_RANGE_FIRST, (float)1);
m_AndroidPlugin.CallStatic("setConfig", CFG_RANGE_LAST, (float)m_Size);
you can set which frames will be in for cycle processed. 
Process in one calling of ADF2Wrapper::Relocalise max. 1000 frames. If there was nothing found then process next 1000 frames in the next call.

5) High accurate relocalisation using surrounding filter
If we have inaccurate position from subtask 3) then disable function from subtask 4) (process whole dataset) and use this inaccurate position for filtering frames (using CFG_SURR_ENABLED). For finding position with higher accuracy you will probably have to change parameters (just like in the subtasks but do it after low accurate relocalisation).

# Code formatting
Code formatting is important when more developers are working on the same code. Merging code with different formatting is very difficult!

1) Install CodeMaid into your installation of Visual Studio 2017:
https://github.com/ar-ml/ar-action-adf/raw/master/CodeMaid%20v10.4.53.vsix

2) Import the formatting config (codemaid->options->import at the bottom):
https://github.com/ar-ml/ar-action-adf/blob/master/CodeMaid.config

3) In Visual Studio Tools/Options/Text Editor/All Languages/Tabs make sure that "Insert spaces" is selected.

4) Do not use CodeMaid when editing 3rd party source codes
