#ifndef UTILS_H
#define UTILS_H

//OpenCV
#include <opencv2/core.hpp>
#include <opencv2/features2d.hpp>
#include <opencv2/imgcodecs.hpp>
#include <opencv2/imgproc.hpp>

//GLM
#define GLM_ENABLE_EXPERIMENTAL
#define GLM_FORCE_RADIANS
#include "glm/glm.hpp"
#include "glm/gtc/matrix_transform.hpp"
#include "glm/gtc/quaternion.hpp"
#include "glm/gtc/type_ptr.hpp"
#include "glm/gtx/matrix_decompose.hpp"

#include <map>

//logging
#ifdef ANDROID
#include <android/log.h>
#define LOGI(...) \
  __android_log_print(ANDROID_LOG_INFO, "araction", __VA_ARGS__)
#define LOGE(...) \
  __android_log_print(ANDROID_LOG_ERROR, "araction", __VA_ARGS__)
#else
#define LOGI(...); \
  printf(__VA_ARGS__); printf("\n")
#define LOGE(...); \
  printf(__VA_ARGS__); printf("\n")
#endif

//dataset format
#define DATASET_OFFSET_CAL 3
#define DATASET_OFFSET_PCL 11
#define DATASET_WIDTH 640
#define DATASET_HEIGHT 360

//config
#define CFG_KEYPOINT_DISTANCE 0
#define CFG_KEYPOINT_LIMIT 1
#define CFG_KEYPOINT_MATCHES 2
#define CFG_RANGE_FIRST 3
#define CFG_RANGE_LAST 4
#define CFG_SURR_DISTANCE 5
#define CFG_SURR_ENABLED 6
#define CFG_SURR_X 7
#define CFG_SURR_Y 8
#define CFG_SURR_Z 9

//output
#define OUT_FRAME_CALIBRATION 0
#define OUT_MATCHES_COUNT 1
#define OUT_POSE_ERROR 2
#define OUT_POSE_FRAME 3
#define OUT_POSE_X 4
#define OUT_POSE_Y 5
#define OUT_POSE_Z 6

namespace araction
{
  // descriptions of points in 2D image
  struct Description
  {
    std::vector< glm::ivec2 > coords;
    cv::Mat data;
    glm::ivec2 resolution;
  };

  // connection between two images
  struct Pair
  {
    glm::ivec2 camera;
    glm::ivec2 dataset;
  };

  // all connections between two images
  struct Match
  {
    int index;
    std::vector< Pair > matches;
  };

  class Utils
  {
    public:
      Utils( std::string dataset );
      void extractDescriptions( int index );
      cv::Mat getImage( int index );
      std::vector< std::pair< glm::vec2, glm::vec3 > > getPointCloud( int index, glm::ivec2 resolution );
      std::vector< Match > processDescriptions( int cameraIndex );
      void processMatches( std::vector< Match > matches );
      float getConfig( int type ) { return config[ type ]; }
      std::string getOutput( int type ) { return output[ type ]; }
      void setConfig( int type, float value ) { config[ type ] = value; }

    private:
      float countError( Match& match );
      std::string float2str( float v );
      std::vector< cv::Point2f > getCalibration( int index );
      Description getDescription( int index );
      std::string getFileName( int index, std::string extension );
      glm::vec3 getPose( int index );
      std::string int2str( int v );
      std::vector< Pair > matchDescription( Description& descriptors1, Description& descriptors2 );

      // OpenCV objects
      cv::Ptr< cv::ORB > detector;
      cv::Ptr< cv::ORB > extractor;
      cv::BFMatcher matcher;
      // Getters and setters
      std::map< int, float > config;
      std::map< int, std::string > output;
      // Dataset
      std::string path;
  };
}

#endif
