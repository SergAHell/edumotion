using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class WindowsAndroidBuilder : MonoBehaviour {

    [MenuItem("BitGym/Windows Android Compile")]
    public static void BuildGame ()
    {
        // Get filename.
        //string path = EditorUtility.SaveFolderPanel("Choose Location of Build Game", "", "");
		string [] scenes = {EditorApplication.currentScene};
        // Build player.
        BuildPipeline.BuildPlayer(scenes, "Dummy.apk", BuildTarget.Android, BuildOptions.None);

        // Copy a file from the project folder to the build folder, alongside the built game.
        FileUtil.CopyFileOrDirectory("Temp/StagingArea", "UnityPlayer");
		FileUtil.CopyFileOrDirectory("Temp/StagingArea/assets", "AndroidBGProject/assets");
		FileUtil.CopyFileOrDirectory("Temp/StagingArea/res/drawable*", "AndroidBGProject/res/drawable*");
		
        // Run the game (Process class from System.Diagnostics).
		/*
        Process proc = new Process();
        proc.StartInfo.FileName = path + "BuiltGame.exe";
        proc.Start();
        */
    }
}
