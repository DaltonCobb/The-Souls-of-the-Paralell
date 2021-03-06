using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
public static class ScriptableObjectManager 
{
    public static void CreateAsset<T>() where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        if(Resources.Load(typeof(T).ToString()) == null)
        {
            string assetPath = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/" + typeof(T).ToString()
                + ".asset"
                );

            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
        else
        {
            Debug.Log(typeof(T).ToString() + " already created!");
        }    
    }
    [MenuItem("Assets/Inventory/CreateInventory")]
    public static void CreateInventory()
    {

    }

    [MenuItem("Assets/Inventory/Create Spell Item List Scriptable Object")]
    public static void CreateSpellItemList()
    {
        ScriptableObjectManager.CreateAsset<SpellItemScriptableObject>();
    }

    [MenuItem("Assets/Inventory/Create Weapon List Scriptable Object")]
    public static void CreateWeaponList()
    {
        ScriptableObjectManager.CreateAsset<WeaponScriptableObject>();
    }
}
