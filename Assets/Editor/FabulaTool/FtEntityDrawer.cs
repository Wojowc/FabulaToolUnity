using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(FTEntity))]
public class FtEntityDrawer : UnityEditor.Editor
{
    //public VisualTreeAsset m_InspectorXML;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        FTEntity fTEntity = (FTEntity)target;
        if (GUILayout.Button("Validate")) fTEntity.Validate();
    }
}