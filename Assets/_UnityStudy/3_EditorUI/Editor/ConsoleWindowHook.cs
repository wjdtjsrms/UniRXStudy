using System.Reflection;
using UniAquarium.Aquarium;
using UnityEditor;
using UnityEngine.UIElements;

[InitializeOnLoad]
public class ConsoleWindowHook
{
    private static VisualElement _rootVisualElement;

    static ConsoleWindowHook()
    {
        EditorApplication.update += Update;
    }

    private static void Update()
    {
        if (_rootVisualElement != null) return;
        
        // get SceneHierarchyWindow.
        var hierarchyWindowType = typeof(Editor).Assembly.GetType("UnityEditor.ConsoleWindow");
        var hierarchyWindow = EditorWindow.GetWindow(hierarchyWindowType);

        // get VisualElement using reflection.
        if (hierarchyWindow == null) return;
        var fieldInfo = hierarchyWindowType.GetField("ms_ConsoleWindow",
            BindingFlags.NonPublic | BindingFlags.Static);
        if (fieldInfo == null) return;
        var sceneHierarchy = fieldInfo.GetValue(hierarchyWindow);
        var sceneHierarchyType = sceneHierarchy.GetType();
        var visualElementFieldInfo = sceneHierarchyType.GetField("ms_ConsoleWindow",
            BindingFlags.NonPublic | BindingFlags.Static);
        if (visualElementFieldInfo == null) return;

        var treeView = visualElementFieldInfo.GetValue(sceneHierarchy);
        var root = treeView as EditorWindow;

        _rootVisualElement = root.rootVisualElement;
        
        // Setting it to false disables clicks and other actions.
        var aquariumComponent = new AquariumComponent(false);
        _rootVisualElement.Add(aquariumComponent);
        aquariumComponent.Enable();
    }
}
