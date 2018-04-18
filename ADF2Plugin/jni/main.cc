#include <jni.h>
#include "utils.h"

araction::Utils* utils;

std::string jstring2string( JNIEnv* env, jstring name )
{
    const char *s = env->GetStringUTFChars( name, NULL );
    std::string str( s );
    env->ReleaseStringUTFChars( name,s );
    return str;
}

jbyteArray string2byte( JNIEnv* env, std::string string )
{
    jbyteArray arr = env->NewByteArray( string.length() );
    env->SetByteArrayRegion( arr, 0, string.length(), ( jbyte* )string.c_str() );
    return arr;
}

#ifdef __cplusplus
extern "C" {
#endif

JNIEXPORT void JNICALL
Java_com_araction_adf2_Wrapper_init( JNIEnv* env, jobject, jstring path )
{
    utils = new araction::Utils( jstring2string(env, path) );
}

JNIEXPORT void JNICALL
Java_com_araction_adf2_Wrapper_createDsc( JNIEnv*, jobject, jint index )
{
    utils->extractDescriptions( index );
}

JNIEXPORT jbyteArray JNICALL
Java_com_araction_adf2_Wrapper_getOutput( JNIEnv* env, jobject, jint type )
{
    return string2byte( env, utils->getOutput( type ) );
}

JNIEXPORT void JNICALL
Java_com_araction_adf2_Wrapper_processDsc( JNIEnv*, jobject, jint index )
{
    utils->extractDescriptions( index );
    utils->processMatches( utils->processDescriptions( index ) );
}

JNIEXPORT void JNICALL
Java_com_araction_adf2_Wrapper_setConfig( JNIEnv*, jobject, jint type, jfloat value )
{
    utils->setConfig( type, value );
}

#ifdef __cplusplus
}
#endif
