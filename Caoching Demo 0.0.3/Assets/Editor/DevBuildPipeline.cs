// /**
// * @file DevBuildPipeline.cs
// * @brief Contains the DevBuildPipeline class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date 02 2017
// * Copyright Heddoko(TM) 2017,  all rights reserved
// */

using System.Linq;
using UnityEditor;

/// <summary>
/// A script for dev build creations
/// </summary>
public class DevBuildPipeline
{
    private static string[] EnabledLevels()
    {
        return (from vScene in EditorBuildSettings.scenes where vScene.enabled select vScene.path).ToArray();
    }
    public static void DevelopmentBuild()
    {
        BuildPipeline.BuildPlayer(new []{ "AppMain" }, "\\\\HeddokoQnap\\Heddoko\\02_SOFTWARE\\RESOURCES\\TempBuildOutput\\WindowsApp\\Dev\\HeddokoDesktop.exe",
            BuildTarget.StandaloneWindows, BuildOptions.Development );
    }
}
