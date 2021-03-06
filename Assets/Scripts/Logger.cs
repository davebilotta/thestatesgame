﻿using UnityEngine;

public static class Logger  {

    private static bool loggingEnabled = false;

	public static void Log(string logMessage)
    {
        if (loggingEnabled)
        {
            Debug.Log(logMessage);
        }
    }

    public static void LogError(string logMessage)
    {
        if (loggingEnabled)
        {
            Debug.LogError(logMessage);
        }
    }
}
