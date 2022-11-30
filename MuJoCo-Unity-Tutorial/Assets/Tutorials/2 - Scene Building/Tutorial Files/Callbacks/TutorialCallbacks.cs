using UnityEngine;
using UnityEditor;
using Unity.Tutorials.Core.Editor;
using System.Linq;

/// <summary>
/// Implement your Tutorial callbacks here.
/// </summary>
[CreateAssetMenu(fileName = DefaultFileName, menuName = "Tutorials/" + DefaultFileName + " Instance")]
public class AdvanceTutorialCallback : ScriptableObject
{
    /// <summary>
    /// The default file name used to create asset of this class type.
    /// </summary>
    public const string DefaultFileName = "AdvanceTutorialCallback";

    public Tutorial tutorial;

    public void Advance()
    {
        tutorial.TryGoToNextPage();
    }
}

