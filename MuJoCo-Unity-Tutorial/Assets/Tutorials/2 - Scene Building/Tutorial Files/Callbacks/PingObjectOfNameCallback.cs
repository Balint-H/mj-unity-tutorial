using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = DefaultFileName, menuName = "Tutorials/" + DefaultFileName + " Instance")]
public class PingObjectOfNameCallback : ScriptableObject
{
    /// <summary>
    /// The default file name used to create asset of this class type.
    /// </summary>
    public const string DefaultFileName = "PingObjectOfNameCallback";

    public string nameToPing;
    public bool selectIt;

    public void PingIt()
    {
        EditorGUIUtility.PingObject(GameObject.Find(nameToPing));
        if(selectIt) Selection.activeGameObject = GameObject.Find(nameToPing);
    }
}
