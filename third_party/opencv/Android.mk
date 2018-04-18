LOCAL_PATH := $(call my-dir)

include $(CLEAR_VARS)
LOCAL_MODULE := libcpufeatures
LOCAL_SRC_FILES := libs/armeabi-v7a/libcpufeatures.a
include $(PREBUILT_STATIC_LIBRARY)

include $(CLEAR_VARS)
LOCAL_MODULE := libilmimf
LOCAL_SRC_FILES := libs/armeabi-v7a/libIlmImf.a
include $(PREBUILT_STATIC_LIBRARY)

include $(CLEAR_VARS)
LOCAL_MODULE := libjasper
LOCAL_SRC_FILES := libs/armeabi-v7a/liblibjasper.a
include $(PREBUILT_STATIC_LIBRARY)

include $(CLEAR_VARS)
LOCAL_MODULE := libjpeg
LOCAL_SRC_FILES := libs/armeabi-v7a/liblibjpeg.a
include $(PREBUILT_STATIC_LIBRARY)

include $(CLEAR_VARS)
LOCAL_MODULE := libpng
LOCAL_SRC_FILES := libs/armeabi-v7a/liblibpng.a
include $(PREBUILT_STATIC_LIBRARY)

include $(CLEAR_VARS)
LOCAL_MODULE := libtiff
LOCAL_SRC_FILES := libs/armeabi-v7a/liblibtiff.a
include $(PREBUILT_STATIC_LIBRARY)

include $(CLEAR_VARS)
LOCAL_MODULE := libwebp
LOCAL_SRC_FILES := libs/armeabi-v7a/liblibwebp.a
include $(PREBUILT_STATIC_LIBRARY)

include $(CLEAR_VARS)
LOCAL_MODULE := libtbb
LOCAL_SRC_FILES := libs/armeabi-v7a/libtbb.a
include $(PREBUILT_STATIC_LIBRARY)

include $(CLEAR_VARS)
LOCAL_MODULE := libtegra_hal
LOCAL_SRC_FILES := libs/armeabi-v7a/libtegra_hal.a
include $(PREBUILT_STATIC_LIBRARY)

include $(CLEAR_VARS)
LOCAL_MODULE := opencv_core
LOCAL_EXPORT_C_INCLUDES := $(LOCAL_PATH)/include
LOCAL_STATIC_LIBRARIES := libcpufeatures libtbb libtegra_hal libilmimf libjasper libjpeg libpng libtiff libwebp
LOCAL_SRC_FILES := libs/armeabi-v7a/libopencv_core.a
include $(PREBUILT_STATIC_LIBRARY)

include $(CLEAR_VARS)
LOCAL_MODULE := opencv_features2d
LOCAL_STATIC_LIBRARIES := opencv_flann opencv_core
LOCAL_SRC_FILES := libs/armeabi-v7a/libopencv_features2d.a
include $(PREBUILT_STATIC_LIBRARY)

include $(CLEAR_VARS)
LOCAL_MODULE := opencv_flann
LOCAL_STATIC_LIBRARIES := opencv_core
LOCAL_SRC_FILES := libs/armeabi-v7a/libopencv_flann.a
include $(PREBUILT_STATIC_LIBRARY)

include $(CLEAR_VARS)
LOCAL_MODULE := opencv_imgcodecs
LOCAL_STATIC_LIBRARIES := opencv_imgproc opencv_core
LOCAL_SRC_FILES := libs/armeabi-v7a/libopencv_imgcodecs.a
include $(PREBUILT_STATIC_LIBRARY)

include $(CLEAR_VARS)
LOCAL_MODULE := opencv_imgproc
LOCAL_STATIC_LIBRARIES := opencv_core
LOCAL_SRC_FILES := libs/armeabi-v7a/libopencv_imgproc.a
include $(PREBUILT_STATIC_LIBRARY)

