using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Lindwurm.Voxel.Editor
{
    [ScriptedImporter(1, new string[] {"vxk", "vxp"})]
    public class VoxelImporter : ScriptedImporter
    {
        public bool m_Humanoid = VoxelImporterSettings.default_Humanoid;
        public bool m_ImportThumbnail = VoxelImporterSettings.default_ImportThumbnail;
        public bool m_AttachCollider = VoxelImporterSettings.default_AttachCollider;
        public bool m_ImportForURP = VoxelImporterSettings.default_ImportForURP;
        public bool m_SplitAllGroups = VoxelImporterSettings.default_SplitAllGroups;
        public bool m_IncludeRotation = VoxelImporterSettings.default_IncludeRotation;
        public bool m_WithFreePose = VoxelImporterSettings.default_WithFreePose;
        public float m_OverrideSize = VoxelImporterSettings.default_OverrideSize;
        public bool m_UseMeshRenderer = VoxelImporterSettings.default_UseMeshRenderer;
        public bool m_FlattenGameObject = VoxelImporterSettings.default_FlattenGameObject;
        public bool m_LightmapUV = VoxelImporterSettings.default_LightmapUV;
        public bool m_CreateTexture = VoxelImporterSettings.default_CreateTexture;
		public bool m_CreateVoxelInfo = VoxelImporterSettings.default_CreateVoxelInfo;
		public bool m_CreateAnimator = VoxelImporterSettings.default_CreateAnimator;
        public string m_standardLitVertexColorMaterialOverride = VoxelImporterSettings.default_standardLitVertexColorMaterialOverride;
        public string m_urpLitVertexColorMaterialOverride = VoxelImporterSettings.default_urpLitVertexColorMaterialOverride;
        public string m_standardLitMaterialOverride = VoxelImporterSettings.default_standardLitMaterialOverride;
        public string m_urpLitMaterialOverride = VoxelImporterSettings.default_urpLitMaterialOverride;

        public float m_OriginSize = 50f;
        public string m_shaderInsidePackage = "Packages/jp.lindwurm.voxel/Shaders/URPLitVertexColor.shadergraph";

        [SerializeField]
        private int m_SettingsVersion = 1;  // m_SettingsVersionがないのは古いインポータを使用したものなので変更しないように
        public override void OnImportAsset(AssetImportContext ctx)
        {
            if (m_SettingsVersion == 1)
            {
                ResetDefault();
            }
            m_SettingsVersion = 2;
            var voxelBlock = new VoxelBlock();
            VoxelUtil.LoadFromFile(ctx.assetPath, voxelBlock, false, (info)=>
            {
                if (info != null && info.thumbnail != null)
                {
                    if (m_ImportThumbnail)
                    {
                        info.thumbnail.name = "Thumbnail";
                        ctx.AddObjectToAsset("Thumbnail", info.thumbnail);
                    }else
                    {
                        DestroyImmediate(info.thumbnail);
						info.thumbnail = null;
                    }
                }
            });
            if (voxelBlock.voxelModelFileParameter != null)
            {
                m_OriginSize = voxelBlock.voxelModelFileParameter.scale * 1000;
            }else
            {
                m_OriginSize = 50f;
            }
            var go = new GameObject();
            string shaderString;
            Shader shader;
            if (m_CreateTexture)
            {
                shaderString = m_ImportForURP ? m_urpLitMaterialOverride : m_standardLitMaterialOverride;
                shader = Shader.Find(shaderString);
            }
            else
            {
                shaderString = m_ImportForURP ? m_urpLitVertexColorMaterialOverride : m_standardLitVertexColorMaterialOverride;
                shader = Shader.Find(shaderString);
                if (shader == null && m_ImportForURP)
                {
                    var guid = AssetDatabase.GUIDFromAssetPath(m_shaderInsidePackage);
                    ctx.DependsOnArtifact(guid);
                    shader = AssetDatabase.LoadAssetAtPath<Shader>(m_shaderInsidePackage);
                    if(shader == null)
                    {
                        return;
                    }
                }
            }

            if (shader == null)
            {
                Debug.Log("Shader Search");
                string[] importers = AssetDatabase.FindAssets(shaderString)
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .ToArray();
                foreach (var one in importers)
                {
                    if (one != null)
                    {
                        if (one.Contains(shaderString + ".shadergraph"))
                        {
                            shader = AssetDatabase.LoadAssetAtPath<Shader>(one);
                            break;
                        }
                    }
                }
                if (shader == null)
                {
                    shader = Shader.Find("Standard");
                    if (shader == null)
                    {
                        Debug.LogError("Shader not found.");
                        return;
                    }
                }
            }

            var mat = new Material(shader);
            mat.name = shaderString.Split('/').LastOrDefault();
            var option = new VoxelUtil.CreateModelOption(m_UseMeshRenderer);
            option.createCollider = m_AttachCollider;
            option.splitAllGroup = m_SplitAllGroups;
            option.thumbnailPose = m_WithFreePose;
            option.isHumanoid = m_Humanoid;
            option.overrideScale = m_OverrideSize;
            option.includeRotation = m_IncludeRotation;
            option.createLightMapUV = m_LightmapUV;
            option.createTexture = m_CreateTexture;
			option.createVoxelInfo = m_CreateVoxelInfo;
			option.createAnimator = m_CreateAnimator;
            VoxelUtil.CreateVoxelModelDirect(voxelBlock, go, mat, option);
			foreach(var one in go.GetComponentsInChildren<FunctionBlockBase>())
			{
				one.ApplyForPrefab();
			}
            foreach (var one in go.GetComponentsInChildren<VoxelMeshDisposer>())
            {
                DestroyImmediate(one);
            }
            // FlattenはCreateVoxelModelDirectに入れない（入れてもいいけど用途的になさそう）
            if (m_FlattenGameObject && m_UseMeshRenderer)
            {
                var children = new List<Transform>();
                for (var i = 0; i < go.transform.childCount; i++)
                {
                    children.Add(go.transform.GetChild(i));
                }
                for (var i = 0; i < children.Count; i++)
                {
                    var grp = children[i];
                    var tf = false;
                    while (!tf)
                    {
                        tf = true;
                        for(var j = 0; j < grp.childCount; j++)
                        {
                            var child = grp.GetChild(j);
                            while (child.childCount > 0)
                            {
                                tf = false;
                                var mago = child.GetChild(0);
                                mago.SetParent(grp.transform);
                            }
                        }
                    }
                }
            }
            ctx.AddObjectToAsset("main obj", go);
            ctx.SetMainObject(go);
            if(m_CreateTexture)
            {
                Debug.Log("AddTexture");
                if (mat.HasProperty("_BaseMap"))
                {
                    var tex = mat.GetTexture("_BaseMap");
                    tex.name = "BaseMap";
                    ctx.AddObjectToAsset(tex.name, tex);
                }
                else if(mat.HasProperty("_MainTex"))
                {
                    var tex = mat.GetTexture("_MainTex");
                    tex.name = "MainTex";
                    ctx.AddObjectToAsset(tex.name, tex);
                }
            }
            ctx.AddObjectToAsset("material", mat);
			if (m_UseMeshRenderer)
			{
				var meshes = go.GetComponentsInChildren<MeshFilter>();
				var c = 0;
				foreach (var one in meshes)
				{
					var n = one.sharedMesh.name;
					if (string.IsNullOrEmpty(n))
					{
						n = $"mesh{c}";
						one.sharedMesh.name = n;
					}
					ctx.AddObjectToAsset(n, one.sharedMesh);
					c++;
				}
			}
			else
			{
				var meshes = go.GetComponentsInChildren<SkinnedMeshRenderer>();
				var c = 0;
				foreach (var one in meshes)
				{
					var n = one.sharedMesh.name;
					if (string.IsNullOrEmpty(n))
					{
						n = $"mesh{c}";
						one.sharedMesh.name = n;
					}
					ctx.AddObjectToAsset(n, one.sharedMesh);
					c++;
				}
			}
			var animator = go.GetComponent<Animator>();
			if (animator != null)
			{
				ctx.AddObjectToAsset("Avater", animator.avatar);
			}
		}
        private void ResetDefault()
        {
            var settings = VoxelImporterSettings.instance;

            m_Humanoid = settings.Humanoid;
            m_ImportThumbnail = settings.ImportThumbnail;
            m_AttachCollider = settings.AttachCollider;
            m_ImportForURP = settings.ImportForURP;
            m_SplitAllGroups = settings.SplitAllGroups;
            m_IncludeRotation = settings.IncludeRotation;
            m_WithFreePose = settings.WithFreePose;
            m_OverrideSize = settings.OverrideSize;
            m_UseMeshRenderer = settings.UseMeshRenderer;
            m_FlattenGameObject = settings.FlattenGameObject;
            m_LightmapUV = settings.LightmapUV;
            m_CreateTexture = settings.CreateTexture;
            m_CreateVoxelInfo = settings.CreateVoxelInfo;
            m_CreateAnimator = settings.CreateAnimator;
            m_standardLitVertexColorMaterialOverride = settings.standardLitVertexColorMaterialOverride;
            m_urpLitVertexColorMaterialOverride = settings.urpLitVertexColorMaterialOverride;
            m_standardLitMaterialOverride = settings.standardLitMaterialOverride;
            m_urpLitMaterialOverride = settings.urpLitMaterialOverride;
        }
    }
    [CustomEditor(typeof(VoxelImporter))]
    public class VoxelImporterEditor : ScriptedImporterEditor
    {
        public override void OnInspectorGUI()
        {
            var humanoid = new GUIContent("Try creating a Humanoid");
            var thumbnail = new GUIContent("Import Thumbnail");
            var collider = new GUIContent("Attach Collider");
            var importUrp = new GUIContent("Import for URP");
            var importFreePose = new GUIContent("Free Pose");
            var includeRotation = new GUIContent("Include local rotation into meshs");
            var splitAllGroups = new GUIContent("Split All Groups");
            var overrideSise = new GUIContent("Override Size");
            var useMeshRenderer = new GUIContent("Use Mesh Renderer");
            var createLightmapUV = new GUIContent("Create Litemap UV");
            var createTexture = new GUIContent("Create Texture");
			var createVoxelInfo = new GUIContent("Create Voxel Info");
			var createAnimator = new GUIContent("Create Animator");
			var prop0 = serializedObject.FindProperty("m_Humanoid");
            EditorGUILayout.PropertyField(prop0, humanoid);
            var prop1 = serializedObject.FindProperty("m_ImportThumbnail");
            EditorGUILayout.PropertyField(prop1, thumbnail);
            var prop2 = serializedObject.FindProperty("m_AttachCollider");
            EditorGUILayout.PropertyField(prop2, collider);
            var prop3 = serializedObject.FindProperty("m_ImportForURP");
            EditorGUILayout.PropertyField(prop3, importUrp);
            var prop8 = serializedObject.FindProperty("m_IncludeRotation");
            EditorGUILayout.PropertyField(prop8, includeRotation);
            var prop4 = serializedObject.FindProperty("m_SplitAllGroups");
            EditorGUILayout.PropertyField(prop4, splitAllGroups);
            var prop5 = serializedObject.FindProperty("m_WithFreePose");
            EditorGUILayout.PropertyField(prop5, importFreePose);
            var prop6 = serializedObject.FindProperty("m_OverrideSize");
            var prop7 = serializedObject.FindProperty("m_OriginSize");
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(prop6, overrideSise);
                EditorGUILayout.LabelField($"({prop7.floatValue})");
            }

            var prop9 = serializedObject.FindProperty("m_UseMeshRenderer");
            EditorGUILayout.PropertyField(prop9, useMeshRenderer);
            var prop16 = serializedObject.FindProperty("m_FlattenGameObject");
            var prop11 = serializedObject.FindProperty("m_LightmapUV");
            if (prop9.boolValue)
            {
                EditorGUILayout.PropertyField(prop16, new GUIContent("Flatten Game Object Tree"));
                EditorGUILayout.PropertyField(prop11, createLightmapUV);
            }

            var prop10 = serializedObject.FindProperty("m_CreateTexture");
            EditorGUILayout.PropertyField(prop10, createTexture);

			var prop17 = serializedObject.FindProperty("m_CreateVoxelInfo");
			EditorGUILayout.PropertyField(prop17, createVoxelInfo);

			var prop18 = serializedObject.FindProperty("m_CreateAnimator");
			EditorGUILayout.PropertyField(prop18, createAnimator);

			var prop12 = serializedObject.FindProperty("m_standardLitVertexColorMaterialOverride");
            EditorGUILayout.PropertyField(prop12, new GUIContent("Lit VertexColor shader"));
            var prop13 = serializedObject.FindProperty("m_urpLitVertexColorMaterialOverride");
            EditorGUILayout.PropertyField(prop13, new GUIContent("URP Lit VertexColor shader"));
            var prop14 = serializedObject.FindProperty("m_standardLitMaterialOverride");
            EditorGUILayout.PropertyField(prop14, new GUIContent("Lit shader"));
            var prop15 = serializedObject.FindProperty("m_urpLitMaterialOverride");
            EditorGUILayout.PropertyField(prop15, new GUIContent("URP Lit Shader"));

            if(GUILayout.Button("Reset parameters"))
            {
                var settings = VoxelImporterSettings.instance;

                prop0.boolValue = settings.Humanoid;
                prop1.boolValue = settings.ImportThumbnail;
                prop2.boolValue = settings.AttachCollider;
                prop3.boolValue = settings.ImportForURP;
                prop8.boolValue = settings.IncludeRotation;
                prop4.boolValue = settings.SplitAllGroups;
                prop5.boolValue = settings.WithFreePose;
                prop6.floatValue = settings.OverrideSize;
                prop9.boolValue = settings.UseMeshRenderer;
                prop16.boolValue = settings.FlattenGameObject;
                prop11.boolValue = settings.LightmapUV;
                prop10.boolValue = settings.CreateTexture;
                prop17.boolValue = settings.CreateVoxelInfo;
                prop18.boolValue = settings.CreateAnimator;
                prop12.stringValue = settings.standardLitVertexColorMaterialOverride;
                prop13.stringValue = settings.urpLitVertexColorMaterialOverride;
                prop14.stringValue = settings.standardLitMaterialOverride;
                prop15.stringValue = settings.urpLitMaterialOverride;
            }

            base.ApplyRevertGUI();
        }
    }
}