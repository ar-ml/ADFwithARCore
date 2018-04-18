#include <opencv2/highgui.hpp>
#include <iostream>
#include "utils.h"

#define DATASET_PATH "../dataset/"
#define DATASET_SIZE 76
#define DATASET_TEST 50
#define DATASET_WAIT 10

araction::Utils* utils;
cv::Ptr< cv::ORB > detector = cv::ORB::create();
cv::Ptr< cv::ORB > extractor = cv::ORB::create();
cv::BFMatcher matcher( cv::NORM_L2, true );

void drawPoints( int index, cv::Mat img )
{
    std::vector< std::pair< glm::vec2, glm::vec3 > > pointCloud;
    pointCloud = utils->getPointCloud( index, glm::ivec2( img.size().width, img.size().height) );

    cv::Point2f point;
    for( int i = 0; i < pointCloud.size(); i++ )
    {
        point.x = pointCloud[ i ].first.x;
        point.y = pointCloud[ i ].first.y;
        cv::drawMarker( img, point, cv::Scalar( 255, 255, 255 ), cv::MARKER_DIAMOND, 5 );
    }
}

void matchFrame( int index, cv::Mat img1, cv::Mat img2 )
{
    // detecting keypoints
    std::vector< cv::KeyPoint > keypoints1, keypoints2;
    detector->detect( img1, keypoints1 );
    detector->detect( img2, keypoints2 );
    if( keypoints1.empty() || keypoints2.empty() )
    {
        return;
    }

    // computing descriptors
    cv::Mat descriptors1, descriptors2;
    extractor->compute( img1, keypoints1, descriptors1 );
    extractor->compute( img2, keypoints2, descriptors2 );

    // matching descriptors
    std::vector< cv::DMatch > matches;
    matcher.match( descriptors1, descriptors2, matches );

    // filter the matches
    std::vector< cv::DMatch > good_matches;
    for( int i = 0; i < matches.size(); i++ )
    {
        if( matches[i].distance < utils->getConfig( CFG_KEYPOINT_DISTANCE ) )
        {
            good_matches.push_back( matches[i] );
        }
    }

    // drawing the results
    std::string window = "Matches | on the left the camera frame | "\
                         "on the right the frame from the dataset";
    cv::namedWindow( window );
    cv::Mat img_matches;
    drawPoints( DATASET_TEST, img1 );
    drawPoints( index, img2 );
    cv::drawMatches( img1, keypoints1, img2, keypoints2, good_matches, img_matches,
                     cv::Scalar::all( -1 ), cv::Scalar::all( -1 ), std::vector< char >(),
                     cv::DrawMatchesFlags::NOT_DRAW_SINGLE_POINTS );
    cv::imshow( window, img_matches );
    cv::waitKey( DATASET_WAIT );

    // debug info
    if( good_matches.size() >= utils->getConfig( CFG_KEYPOINT_MATCHES ) )
    {
        std::cout << "matching " << good_matches.size() << "x - in the frame " << index << std::endl;
    }
}

int main( int, char** )
{
    utils = new araction::Utils( DATASET_PATH );
    utils->setConfig( CFG_KEYPOINT_DISTANCE, 150 );
    utils->setConfig( CFG_KEYPOINT_MATCHES, 3 );
    cv::Mat cameraImg = utils->getImage( DATASET_TEST );

    // process dataset images
    for( int index = 0; index <= DATASET_SIZE; index++ )
    {
        if( index != DATASET_TEST )
        {
            matchFrame( index, cameraImg, utils->getImage( index ) );
        }
    }
    return 0;
}
