#include <GL/freeglut.h>
#include <iostream>
#include "utils.h"

#define DATASET_PATH "../dataset/"
#define DATASET_SIZE 76

araction::Utils* utils;

void extractDescriptions( void )
{
    for( int index = 0; index <= DATASET_SIZE; index++ )
    {
        utils->extractDescriptions( index );
        std::cout << index << "/" << DATASET_SIZE << std::endl;
    }
}

void process( int index )
{
    std::vector< araction::Match > matches = utils->processDescriptions( index );
    if( !matches.empty() )
    {
        utils->processMatches( matches );
        std::cout << "--------------------------------------------------------" << std::endl;
        std::cout << "camera_error: " << utils->getOutput( OUT_POSE_ERROR ) << std::endl;
        std::cout << "camera_frame: " << utils->getOutput( OUT_POSE_FRAME ) << std::endl;
        std::cout << "camera_pose: ";
        std::cout << utils->getOutput( OUT_POSE_X ) << " ";
        std::cout << utils->getOutput( OUT_POSE_Y ) << " ";
        std::cout << utils->getOutput( OUT_POSE_Z ) << std::endl;
        std::cout << "matches_found: " << utils->getOutput( OUT_MATCHES_COUNT ) << std::endl;
        std::cout << "tested_frame: " << index << std::endl;
    }
}

int main( int, char** )
{
    //init
    utils = new araction::Utils( DATASET_PATH );
    utils->setConfig( CFG_KEYPOINT_DISTANCE, 150 );
    utils->setConfig( CFG_KEYPOINT_LIMIT, 250 );
    utils->setConfig( CFG_KEYPOINT_MATCHES, 3 );
    utils->setConfig( CFG_RANGE_FIRST, 0 );
    utils->setConfig( CFG_RANGE_LAST, DATASET_SIZE );
    utils->setConfig( CFG_SURR_ENABLED, 0 );

    // create "ADF"
    //extractDescriptions();

    // estimate pose
    for( int index = 0; index < DATASET_SIZE; index += 10 )
    {
      process( index );
    }

    return( 0 );
}
