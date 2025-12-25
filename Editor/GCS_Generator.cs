using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GlobalComponentSystem))]
public class GCS_Generator : Editor
{
    private static string PackageRoot()
    {
        string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(CreateInstance<GCS_Generator>()));
        return Path.GetDirectoryName(scriptPath);
    }

    private static string LoadTemplate(string path)
    {
        string fullPath = Path.Combine(PackageRoot(), path);
        if (!File.Exists(fullPath))
        {
            Debug.LogError($"Template not found: {fullPath}");
            return string.Empty;
        }
        return File.ReadAllText(fullPath);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);
        if(GUILayout.Button("Update"))
        {
            GenerateCode();
        }
    }

    public void GenerateCode()
    {
        string arrayPropertiesTemplate = LoadTemplate("Templates/GCS_array_properties_template.txt");
        string propertiesTemplate = LoadTemplate("Templates/GCS_properties_template.txt");
        string classTemplate = LoadTemplate("Templates/GCS_class_template.txt");

        HashSet<string> uniqueScriptNames = new HashSet<string>();
        GlobalComponentSystem data = (GlobalComponentSystem)target;
        // Validation
        if (data.globalComponents != null)
        {
            List<GlobalComponentSystem.GlobalComponentSettings> uniqueList = new List<GlobalComponentSystem.GlobalComponentSettings>();
            foreach (GlobalComponentSystem.GlobalComponentSettings settings in data.globalComponents)
            {
                MonoScript script = settings.script;
                if (script == null)
                {
                    continue;
                }
                if (uniqueScriptNames.Contains(script.GetClass().Name))
                {
                    Debug.LogError($"Script {script.name} is duplicated in the list!");
                    continue;
                }   
                if (script.GetClass() == null || !typeof(MonoBehaviour).IsAssignableFrom(script.GetClass()))
                {
                    Debug.LogError($"Script {script.name} can't be a global component because it is not a MonoBehaviour script!");
                    continue;
                }
                uniqueList.Add(settings);
                uniqueScriptNames.Add(script.GetClass().Name);
            }
            if (uniqueList.Count != data.globalComponents.Count)
            {
                data.globalComponents = uniqueList;
                EditorUtility.SetDirty(data);
            }
        }

        if(data.globalComponents == null || data.globalComponents.Count == 0) {
            Debug.LogError("No global component is set. Generation is cancelled.");
            return;
        }
        string filePath = Path.Combine(data.generatedScriptOutputFolder, "GCS.generated.cs");

        StringBuilder stringBuilder = new StringBuilder();
        foreach(var settings in data.globalComponents)
        {
            MonoScript script = settings.script;
            if(settings.allowMultiple)
            {
                stringBuilder.Append(arrayPropertiesTemplate
                    .Replace("{{ CLASS_TYPE }}", script.GetClass().Name)
                    .Replace("{{ LOWERCASE_NAME }}", "_" + script.GetClass().Name.ToLower())
                    );
            } else
            {
                stringBuilder.Append(propertiesTemplate
                    .Replace("{{ CLASS_TYPE }}", script.GetClass().Name)
                    .Replace("{{ LOWERCASE_NAME }}", "_" + script.GetClass().Name.ToLower())
                    );
            }
        }
        File.WriteAllText(filePath, classTemplate.
            Replace("{{ PROPERTIES }}", stringBuilder.ToString())
                .Replace("\r\n", "\n")
                .Replace("\r", "\n"),
            Encoding.UTF8);

        AssetDatabase.Refresh();
        Debug.Log($"GCS generated at {filePath}");
    }
}
