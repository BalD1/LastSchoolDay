using UnityEditor;
using UnityEngine;

public class PopUpAssetInspector : EditorWindow
{
    private Object asset;
    private Editor assetEditor;

    public static PopUpAssetInspector Create(Object asset)
    {
        var window = CreateWindow<PopUpAssetInspector>($"{asset.name} | {asset.GetType().Name}");
        window.asset = asset;
        window.assetEditor = Editor.CreateEditor(asset);
        return window;
    }

    private void OnGUI()
    {
        assetEditor.OnInspectorGUI();
    }
}