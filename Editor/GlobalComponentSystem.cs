using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalComponentSystem", menuName = "GCS/GlobalComponentSystem")]
public class GlobalComponentSystem : ScriptableObject
{
    [Serializable]
    public class GlobalComponentSettings
    {
        public MonoScript script;
        public bool allowMultiple;
    }
    public List<GlobalComponentSettings> globalComponents;
    public string generatedScriptOutputFolder = "Assets";
}
