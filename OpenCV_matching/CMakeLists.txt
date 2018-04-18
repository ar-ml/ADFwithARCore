cmake_minimum_required (VERSION 2.6)

project(OpenCV_matching)

find_package( OpenCV 3.3.1 REQUIRED )
find_package( Ceres REQUIRED )
add_definitions("-DCERES_FOUND=1")

set(BASEPATH "${CMAKE_SOURCE_DIR}")
include_directories("${BASEPATH}" ../common)

add_executable (a.out
../common/utils.cc
testprog.cpp)
target_link_libraries( a.out ${OpenCV_LIBS})
