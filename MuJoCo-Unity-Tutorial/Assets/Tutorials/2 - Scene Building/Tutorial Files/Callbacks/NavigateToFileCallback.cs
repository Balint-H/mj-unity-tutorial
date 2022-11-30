using UnityEngine;
using UnityEditor;
using Unity.Tutorials.Core.Editor;
using System.Reflection;
using System.IO;

/// <summary>
/// Implement your Tutorial callbacks here.
/// </summary>
[CreateAssetMenu(fileName = DefaultFileName, menuName = "Tutorials/" + DefaultFileName + " Instance")]
public class NavigateToFileCallback : ScriptableObject
{
    /// <summary>
    /// The default file name used to create asset of this class type.
    /// </summary>
    public const string DefaultFileName = "NavigateToFileCallback";

    public UnityEngine.Object TargetObject;

    /// <summary>
    /// Creates a TutorialCallbacks asset and shows it in the Project window.
    /// </summary>
    /// <param name="assetPath">
    /// A relative path to the project's root. If not provided, the Project window's currently active folder path is used.
    /// </param>
    /// <returns>The created asset</returns>
    public static ScriptableObject CreateAndShowAsset(string assetPath = null)
    {
        assetPath = assetPath ?? $"{TutorialEditorUtils.GetActiveFolderPath()}/{DefaultFileName}.asset";
        var asset = CreateInstance<NavigateToFileCallback>();
        AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
        EditorUtility.FocusProjectWindow(); // needed in order to make the selection of newly created asset to really work
        Selection.activeObject = asset;
        return asset;
    }

    /// <summary>
    /// Example callback for basic UnityEvent
    /// </summary>
    public void NavigateToObject()
    {
        try
        {
            ShowFolderContents(AssetDatabase.LoadAssetAtPath<Object>(Path.GetDirectoryName(AssetDatabase.GetAssetPath(TargetObject))).GetInstanceID());
        }
        catch
        {
            // We might run into exceptions if project browser is minimized
        }
    }


    /// <summary>
    /// Selects a folder in the project window and shows its content.
    /// Opens a new project window, if none is open yet.
    /// Author: Xarbrough, https://forum.unity.com/threads/tutorial-how-to-to-show-specific-folder-content-in-the-project-window-via-editor-scripting.508247/
    /// </summary>
    /// <param name="folderInstanceID">The instance of the folder asset to open.</param>
    private static void ShowFolderContents(int folderInstanceID)
    {
        // Find the internal ProjectBrowser class in the editor assembly.
        Assembly editorAssembly = typeof(Editor).Assembly;
        System.Type projectBrowserType = editorAssembly.GetType("UnityEditor.ProjectBrowser");

        // This is the internal method, which performs the desired action.
        // Should only be called if the project window is in two column mode.
        MethodInfo showFolderContents = projectBrowserType.GetMethod(
            "ShowFolderContents", BindingFlags.Instance | BindingFlags.NonPublic);

        // Find any open project browser windows.
        Object[] projectBrowserInstances = Resources.FindObjectsOfTypeAll(projectBrowserType);

        if (projectBrowserInstances.Length > 0)
        {
            for (int i = 0; i < projectBrowserInstances.Length; i++)
                ShowFolderContentsInternal(projectBrowserInstances[i], showFolderContents, folderInstanceID);
        }
        else
        {
            EditorWindow projectBrowser = OpenNewProjectBrowser(projectBrowserType);
            ShowFolderContentsInternal(projectBrowser, showFolderContents, folderInstanceID);
        }
    }

    private static void ShowFolderContentsInternal(Object projectBrowser, MethodInfo showFolderContents, int folderInstanceID)
    {
        // Sadly, there is no method to check for the view mode.
        // We can use the serialized object to find the private property.
        SerializedObject serializedObject = new SerializedObject(projectBrowser);
        bool inTwoColumnMode = serializedObject.FindProperty("m_ViewMode").enumValueIndex == 1;

        if (!inTwoColumnMode)
        {
            // If the browser is not in two column mode, we must set it to show the folder contents.
            MethodInfo setTwoColumns = projectBrowser.GetType().GetMethod(
                "SetTwoColumns", BindingFlags.Instance | BindingFlags.NonPublic);
            setTwoColumns.Invoke(projectBrowser, null);
        }

        bool revealAndFrameInFolderTree = true;
        showFolderContents.Invoke(projectBrowser, new object[] { folderInstanceID, revealAndFrameInFolderTree });
    }

    private static EditorWindow OpenNewProjectBrowser(System.Type projectBrowserType)
    {
        EditorWindow projectBrowser = EditorWindow.GetWindow(projectBrowserType);
        projectBrowser.Show();

        // Unity does some special initialization logic, which we must call,
        // before we can use the ShowFolderContents method (else we get a NullReferenceException).
        MethodInfo init = projectBrowserType.GetMethod("Init", BindingFlags.Instance | BindingFlags.Public);
        init.Invoke(projectBrowser, null);

        return projectBrowser;
    }

}
