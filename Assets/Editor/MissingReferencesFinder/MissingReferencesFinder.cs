using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Unity 编辑器辅助工具：在场景或工程资源中查找序列化字段上「丢失引用」（引用实例 ID 非 0 但对象为空）的组件。
/// 通过菜单 EditorScripts/Missing References/ 执行。
/// </summary>
public class MissingReferencesFinder : MonoBehaviour
{
	private const string MENU_ROOT = "EditorScripts/Missing References/";

	/// <summary> 在当前已加载的活跃场景中查找丢失引用。 </summary>
	[MenuItem(MENU_ROOT + "Search in scene", false, 50)]
	public static void FindMissingReferencesInCurrentScene()
	{
		var sceneObjects = GetSceneObjects();
		Scene active = EditorSceneManager.GetActiveScene();
		string context = string.IsNullOrEmpty(active.path)
			? $"{active.name} (未保存场景)"
			: active.path;
		FindMissingReferences(context, sceneObjects);
	}

	/// <summary>
	/// 在 Build Settings 中已勾选的场景里依次打开并检查丢失引用；开始前会询问是否保存当前修改，结束后尽量恢复原先打开的场景。
	/// </summary>
	[MenuItem(MENU_ROOT + "Search in all scenes", false, 51)]
	public static void MissingSpritesInAllScenes()
	{
		var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).ToArray();
		if (scenes.Length == 0)
		{
			Debug.LogWarning("MissingReferencesFinder: Build Settings 中没有已启用的场景。");
			return;
		}

		if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			return;

		Scene original = EditorSceneManager.GetActiveScene();
		string originalPath = original.path;

		try
		{
			foreach (var scene in scenes)
			{
				EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);
				FindMissingReferencesInCurrentScene();
			}
		}
		finally
		{
			if (!string.IsNullOrEmpty(originalPath))
				EditorSceneManager.OpenScene(originalPath, OpenSceneMode.Single);
		}
	}

	/// <summary> 在工程 Assets 下的预制体等资源中查找丢失引用（仅检查能作为 GameObject 加载的路径）。 </summary>
	[MenuItem(MENU_ROOT + "Search in assets", false, 52)]
	public static void MissingSpritesInAssets()
	{
		var allAssets = AssetDatabase.GetAllAssetPaths().Where(path => path.StartsWith("Assets/")).ToArray();
		var objs = allAssets.Select(a => AssetDatabase.LoadAssetAtPath(a, typeof(GameObject)) as GameObject).Where(a => a != null).ToArray();

		FindMissingReferences("Project", objs);
	}

	private static void FindMissingReferences(string context, GameObject[] objects)
	{
		foreach (var go in objects)
		{
			var components = go.GetComponents<Component>();

			foreach (var c in components)
			{
				if (!c)
				{
					Debug.LogError("物体缺失的组件" + GetFullPath(go), go);
					continue;
				}

				SerializedObject so = new SerializedObject(c);
				var sp = so.GetIterator();

				while (sp.NextVisible(true))
				{
					if (sp.propertyType == SerializedPropertyType.ObjectReference)
					{
						if (sp.objectReferenceValue == null
						    && sp.objectReferenceInstanceIDValue != 0)
						{
							ShowError(context, go, c.GetType().Name, ObjectNames.NicifyVariableName(sp.name));
						}
					}
				}
			}
		}
	}

	private static GameObject[] GetSceneObjects()
	{
		return Resources.FindObjectsOfTypeAll<GameObject>()
			.Where(go => string.IsNullOrEmpty(AssetDatabase.GetAssetPath(go))
			             && go.hideFlags == HideFlags.None).ToArray();
	}

	private static void ShowError(string context, GameObject go, string componentName, string propertyName)
	{
		const string ERROR_TEMPLATE = "缺失的引用: [{3}]{0}. 组件: {1}, 特性: {2}";

		Debug.LogError(string.Format(ERROR_TEMPLATE, GetFullPath(go), componentName, propertyName, context), go);
	}

	private static string GetFullPath(GameObject go)
	{
		return go.transform.parent == null
			? go.name
			: GetFullPath(go.transform.parent.gameObject) + "/" + go.name;
	}
}
