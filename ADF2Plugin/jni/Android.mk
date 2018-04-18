LOCAL_PATH := $(call my-dir)
PROJECT_ROOT:= $(call my-dir)/../..

include $(CLEAR_VARS)
LOCAL_MODULE           := libadf2
LOCAL_SHARED_LIBRARIES := opencv_imgcodecs opencv_features2d opencv_core
LOCAL_STATIC_LIBRARIES := jpeg-turbo png
LOCAL_CFLAGS           := -std=c++11

LOCAL_C_INCLUDES := $(PROJECT_ROOT)/common/  \
		    $(PROJECT_ROOT)/third_party/glm/ \
                    $(PROJECT_ROOT)/third_party/opencv/include/

LOCAL_SRC_FILES := ../../common/utils.cc \
                   main.cc

LOCAL_LDLIBS    := -llog -lGLESv2 -L$(SYSROOT)/usr/lib -lz -landroid
include $(BUILD_SHARED_LIBRARY)

$(call import-add-path, $(PROJECT_ROOT)/third_party)
$(call import-module,opencv)
