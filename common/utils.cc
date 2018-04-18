#include "utils.h"

namespace araction {

Utils::Utils( std::string dataset )
{
    detector = cv::ORB::create( 250 );
    extractor = cv::ORB::create();
    matcher = cv::BFMatcher( cv::NORM_L2, true );

    path = dataset;
}

float Utils::countError( Match& match )
{
    int count = 0;
    float error = 0;
    for( unsigned int i = 0; i < match.matches.size(); i++ )
    {
        for( unsigned int j = 0; j < match.matches.size(); j++ )
        {
            if( i == j)
            {
                continue;
            }
            float a = glm::length( glm::vec2( match.matches[ i ].camera - match.matches[ j ].camera ) );
            float b = glm::length( glm::vec2( match.matches[ i ].dataset - match.matches[ j ].dataset ) );
            error += fabs( a - b );
            count++;
        }
    }
    return error / ( float )count;
}

void Utils::extractDescriptions( int index )
{
    //get input and output
    std::string filename = getFileName( index, ".dsc" );
    cv::Mat img = getImage( index );

    // detecting keypoints
    std::vector< cv::KeyPoint > keypoints;
    detector->detect( img, keypoints );
    if( keypoints.empty() )
    {
        FILE *file = fopen( filename.c_str(), "wb" );
        fprintf( file, "0 0 0\n" );
        fclose( file );
        return;
    }

    // computing descriptors
    cv::Mat descriptors;
    extractor->compute( img, keypoints, descriptors );

    // store data into file
    FILE *file = fopen( filename.c_str(), "wb" );
    if( descriptors.type() == CV_8U )
    {
        int width = descriptors.size().width;
        int height = descriptors.size().height;
        fprintf( file, "%d %d %d %d %d\n", ( int )keypoints.size(), width, height, img.size().width, img.size().height );
        fwrite( descriptors.data, width * height, 1, file );
        for( int index = 0; index < keypoints.size(); index++ )
        {
            fprintf( file, "%d %d\n", ( int )keypoints[ index ].pt.x, ( int )keypoints[ index ].pt.y );
        }
        fclose( file );
    }
    else
    {
        fprintf( file, "0 0 0\n" );
        fclose( file );
    }
}

std::string Utils::float2str( float v )
{
    std::ostringstream ss;
    ss << v;
    return ss.str();
}

std::vector< cv::Point2f > Utils::getCalibration( int index )
{
    FILE* file = fopen( getFileName( index, ".txt" ).c_str(), "r" );
    for( int i = 0; i < DATASET_OFFSET_CAL; i++ )
    {
      char temp[ 128 ];
      fscanf( file, "%s", temp );
    }
    cv::Point2f value;
    std::vector< cv::Point2f > calibration;

    fscanf( file, "%f %f\n", &value.x, &value.y );
    calibration.push_back( value );

    fscanf( file, "%f %f\n", &value.x, &value.y );
    calibration.push_back( value );

    fscanf( file, "%f %f\n", &value.x, &value.y );
    calibration.push_back( value );

    fscanf( file, "%f %f\n", &value.x, &value.y );
    calibration.push_back( value );

    fclose( file );
    return calibration;
}

Description Utils::getDescription( int index )
{
    int count = 0;
    int width = 0;
    int height = 0;
    Description output;
    FILE *file = fopen( getFileName( index, ".dsc" ).c_str(), "rb" );
    fscanf( file, "%d %d %d %d %d\n", &count, &width, &height, &output.resolution.x, &output.resolution.y );
    output.data = cv::Mat( height, width, CV_8U );
    fread( output.data.data, width * height, 1, file );
    if( count > config[ CFG_KEYPOINT_LIMIT ] )
    {
        count = config[ CFG_KEYPOINT_LIMIT ];
        height = count;
        cv::resize( output.data, output.data, cv::Size( width, height ) );
    }
    for( int index = 0; index < count; index++ )
    {
        fscanf( file, "%d %d\n", &width, &height );
        output.coords.push_back( glm::ivec2( width, height ) );
    }
    fclose( file );
    return output;
}

std::string Utils::getFileName( int index, std::string extension )
{
    std::string number = int2str( index );
    while( number.size() < 8 )
    {
        number = "0" + number;
    }
    return path + number + extension;
}

cv::Mat Utils::getImage( int index )
{
    std::vector< cv::Point2f > src = getCalibration( index );
    std::vector< cv::Point2f > dst;

    int flipAxis = 0;
    bool flipPre = false;
    bool flipPost = false;
    bool rotate = false;
    if ( src[ 0 ].x > src[ 3 ]. x )
    {
        if ( src[ 0 ].y > src[ 3 ]. y )
        {
          dst.push_back( cv::Point2f( 1, 1 ) );
          dst.push_back( cv::Point2f( 0, 1 ) );
          dst.push_back( cv::Point2f( 1, 0 ) );
          dst.push_back( cv::Point2f( 0, 0 ) );
          rotate = true;
        }
        else
        {
          dst.push_back( cv::Point2f( 1, 0 ) );
          dst.push_back( cv::Point2f( 0, 0 ) );
          dst.push_back( cv::Point2f( 1, 1 ) );
          dst.push_back( cv::Point2f( 0, 1 ) );
          flipAxis = 1;
          flipPost = true;
        }
    }
    else
    {
        if ( src[ 0 ].y > src[ 3 ]. y )
        {
          dst.push_back( cv::Point2f( 0, 1 ) );
          dst.push_back( cv::Point2f( 1, 1 ) );
          dst.push_back( cv::Point2f( 0, 0 ) );
          dst.push_back( cv::Point2f( 1, 0 ) );
          flipPre = true;
        }
        else
        {
          dst.push_back( cv::Point2f( 0, 0 ) );
          dst.push_back( cv::Point2f( 1, 0 ) );
          dst.push_back( cv::Point2f( 0, 1 ) );
          dst.push_back( cv::Point2f( 1, 1 ) );
          flipPre = true;
          flipPost = true;
          rotate = true;
        }
    }

    cv::Mat transform = cv::getPerspectiveTransform( src, dst );
    cv::Mat image = cv::imread( getFileName( index, ".jpg" ), cv::IMREAD_GRAYSCALE );
    if( flipPre )
    {
        cv::flip( image, image, flipAxis );
    }
    cv::Size size = rotate ? cv::Size( image.size().height, image.size().width ) : image.size();
    cv::warpPerspective( image, image, transform, size );
    if( flipPost )
    {
        cv::flip( image, image, flipAxis );
    }
    return image;
}

std::vector< std::pair< glm::vec2, glm::vec3 > > Utils::getPointCloud( int index, glm::ivec2 resolution )
{
    int count = 0;
    std::pair< glm::vec2, glm::vec3 > point;
    std::vector< std::pair< glm::vec2, glm::vec3 > > points;
    FILE* file = fopen( getFileName( index, ".txt" ).c_str(), "r" );
    for( int i = 0; i < DATASET_OFFSET_PCL; i++ )
    {
      char temp[ 128 ];
      fscanf( file, "%s", temp );
    }
    fscanf( file, "%d\n", &count );

    for( int i = 0; i < count; i++ )
    {
        fscanf( file, "%f %f %f %f %f", &point.second.x, &point.second.y, &point.second.z, &point.first.x, &point.first.y );
        point.first.x *= ( float )( resolution.x - 1 );
        point.first.y *= ( float )( resolution.y - 1 );
        points.push_back( point );
    }
    fclose( file );
    return points;
}

glm::vec3 Utils::getPose( int index )
{
    glm::vec3 pose;
    FILE* file = fopen( getFileName( index, ".txt" ).c_str(), "r" );
    fscanf( file, "%f %f %f\n", &pose.x, &pose.y, &pose.z );
    fclose( file );
    return pose;
}

std::string Utils::int2str( int v )
{
    std::ostringstream ss;
    ss << v;
    return ss.str();
}

std::vector< Pair > Utils::matchDescription( Description& descriptors1, Description& descriptors2 )
{
    // check validity of description
    std::vector< Pair > output;
    if( ( descriptors1.data.size().area() == 0 ) || ( descriptors2.data.size().area() == 0 ) )
    {
        return output;
    }

    // matching descriptors
    std::vector< cv::DMatch > matches;
    matcher.match( descriptors1.data, descriptors2.data, matches );

    // filter the matches
    std::vector< cv::DMatch > good_matches;
    for( int i = 0; i < matches.size(); i++ )
    {
        if( matches[i].distance < config[ CFG_KEYPOINT_DISTANCE ] )
        {
            good_matches.push_back( matches[i] );
        }
    }

    // output
    if( good_matches.size() >= 3 )
    {
        Pair pair;
        for( int i = 0; i < good_matches.size(); i++ )
        {
            pair.camera = descriptors1.coords[ good_matches[ i ].queryIdx ];
            pair.dataset = descriptors2.coords[ good_matches[ i ].trainIdx ];
            output.push_back( pair );
        }
    }
    return output;
}

std::vector< Match > Utils::processDescriptions( int cameraIndex )
{
    float distance;
    glm::vec3 point;
    bool surrounding = config[ CFG_SURR_ENABLED ] > 0.5f;
    if( surrounding )
    {
        distance = config[ CFG_SURR_DISTANCE ];
        point.x = config[ CFG_SURR_X ];
        point.y = config[ CFG_SURR_Y ];
        point.z = config[ CFG_SURR_Z ];
    }
    Description cameraDesc = getDescription( cameraIndex );

    // process dataset images descriptions
    Match match;
    std::vector< Match > matches;
    for( match.index = config[ CFG_RANGE_FIRST ]; match.index <= config[ CFG_RANGE_LAST ]; match.index++ )
    {
        if( surrounding && ( glm::length( getPose( match.index ) - point ) > distance ) )
        {
            continue;
        }
        Description datasetDesc = getDescription( match.index );
        match.matches = matchDescription( cameraDesc, datasetDesc );
        if( match.matches.size() >= config[ CFG_KEYPOINT_MATCHES ] )
        {
            if( match.index != cameraIndex )
            {
               matches.push_back( match );
            }
        }
    }
    return matches;
}

void Utils::processMatches( std::vector< Match > matches )
{
    float error = 9999999;
    int index = 0;
    for (Match& match : matches)
    {
        if( error > countError( match ) )
        {
            error = countError( match );
            index = match.index;
        }
    }
    std::vector< cv::Point2f > calibration = getCalibration( index );
    glm::vec3 pose = getPose( index );
    std::string value;
    for( int i = 0; i < 4; i++ )
    {
        value += float2str( calibration[ i ].x ) + " " + float2str( calibration[ i ].y ) + " ";
    }
    output[ OUT_FRAME_CALIBRATION ] = value;
    output[ OUT_MATCHES_COUNT ] = int2str( matches.size() );
    output[ OUT_POSE_ERROR ] = float2str( error );
    output[ OUT_POSE_FRAME ] = getFileName( index, ".jpg" );
    output[ OUT_POSE_X ] = float2str( pose.x );
    output[ OUT_POSE_Y ] = float2str( pose.y );
    output[ OUT_POSE_Z ] = float2str( pose.z );
}

}
