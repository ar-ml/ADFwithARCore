package com.araction.adf2;

public class Wrapper
{
    static
    {
        System.loadLibrary("adf2");
    }
    
    public static native void init(String path);
    public static native void createDsc(int index);
    private static native byte[] getOutput(int type);
    public static native void processDsc(int index);
    public static native void setConfig(int type, float value);
    
    public static String getData(int type)
    {
      return new String(getOutput(type));
    }
}
