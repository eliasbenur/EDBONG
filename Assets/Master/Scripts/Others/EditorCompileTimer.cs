﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class EditorCompileTimer : MonoBehaviour
{
    private double compileStartTime;
    private bool isCompiling = false;

    private void Update()
    {
        if (isCompiling)
        {
            if (!EditorApplication.isCompiling)
            {
                isCompiling = false;
                CompileFinished();
            }
        }
        else
        {
            if (EditorApplication.isCompiling)
            {
                isCompiling = true;
                CompileStarted();
            }
        }
    }

    private void CompileStarted()
    {
        Debug.Log("Compile Started ...");
        compileStartTime = EditorApplication.timeSinceStartup;
    }

    private void CompileFinished()
    {
        double compileTime = EditorApplication.timeSinceStartup - compileStartTime;
        Debug.Log("Compile Finished: " + compileTime.ToString("F2") + "s");
    }
}
