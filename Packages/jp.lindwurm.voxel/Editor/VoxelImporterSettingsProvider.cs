using UnityEditor;
using UnityEngine;

namespace Lindwurm.Voxel.Editor
{
    public class VoxelImporterSettingsProvider
    {

        public const string ContentStr_Humanoid = "Try creating a Humanoid";
        public const string ContentStr_ImportThumbnail = "Import Thumbnail";
        public const string ContentStr_AttachCollider = "Attach Collider";
        public const string ContentStr_ImportForURP = "Import for URP";
        public const string ContentStr_SplitAllGroups = "Split All Groups";

        public const string ContentStr_IncludeRotation = "Include local rotation into meshes";
        public const string ContentStr_WithFreePose = "Free Pose";
        public const string ContentStr_OverrideSize = "Override Size";
        public const string ContentStr_UseMeshRenderer = "Use Mesh Renderer";
        public const string ContentStr_FlattenGameObject = "Flatten Game Object Tree";
        public const string ContentStr_LightmapUV = "Create Lightmap UV";
        public const string ContentStr_CreateTexture = "Create Texture";
        public const string ContentStr_CreateVoxelInfo = "Create Voxel Info";
        public const string ContentStr_CreateAnimator = "Create Animator";
        public const string ContentStr_standardLitVertexColorMaterialOverride = "Lit VertexColor Shader";
        public const string ContentStr_urpLitVertexColorMaterialOverride = "URP Lit VertexColor Shader";
        public const string ContentStr_standardLitMaterialOverride = "Lit Shader";
        public const string ContentStr_urpLitMaterialOverride = "URP Lit Shader";

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            var provider = new SettingsProvider("Project/VoxelImporter", SettingsScope.Project)
            {
                label = "VoxelImporter",
                guiHandler = searchContext =>
                {
                    var settings = VoxelImporterSettings.instance;

                    float oldLabelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 250f;

                    EditorGUILayout.LabelField("Project Default Settings", EditorStyles.boldLabel);

                    settings.Humanoid = EditorGUILayout.Toggle(ContentStr_Humanoid, settings.Humanoid);
                    settings.ImportThumbnail = EditorGUILayout.Toggle(ContentStr_ImportThumbnail, settings.ImportThumbnail);
                    settings.AttachCollider = EditorGUILayout.Toggle(ContentStr_AttachCollider, settings.AttachCollider);
                    settings.ImportForURP = EditorGUILayout.Toggle(ContentStr_ImportForURP, settings.ImportForURP);
                    settings.IncludeRotation = EditorGUILayout.Toggle(ContentStr_IncludeRotation, settings.IncludeRotation);
                    settings.SplitAllGroups = EditorGUILayout.Toggle(ContentStr_SplitAllGroups, settings.SplitAllGroups);
                    settings.WithFreePose = EditorGUILayout.Toggle(ContentStr_WithFreePose, settings.WithFreePose);
                    settings.OverrideSize = EditorGUILayout.FloatField(ContentStr_OverrideSize, settings.OverrideSize);
                    settings.UseMeshRenderer = EditorGUILayout.Toggle(ContentStr_UseMeshRenderer, settings.UseMeshRenderer);
                    settings.FlattenGameObject = EditorGUILayout.Toggle(ContentStr_FlattenGameObject, settings.FlattenGameObject);
                    settings.LightmapUV = EditorGUILayout.Toggle(ContentStr_LightmapUV, settings.LightmapUV);
                    settings.CreateTexture = EditorGUILayout.Toggle(ContentStr_CreateTexture, settings.CreateTexture);
                    settings.CreateVoxelInfo = EditorGUILayout.Toggle(ContentStr_CreateVoxelInfo, settings.CreateVoxelInfo);
                    settings.CreateAnimator = EditorGUILayout.Toggle(ContentStr_CreateAnimator, settings.CreateAnimator);
                    settings.standardLitVertexColorMaterialOverride = EditorGUILayout.TextField(ContentStr_standardLitVertexColorMaterialOverride, settings.standardLitVertexColorMaterialOverride);
                    settings.urpLitVertexColorMaterialOverride = EditorGUILayout.TextField(ContentStr_urpLitVertexColorMaterialOverride, settings.urpLitVertexColorMaterialOverride);
                    settings.standardLitMaterialOverride = EditorGUILayout.TextField(ContentStr_standardLitMaterialOverride, settings.standardLitMaterialOverride);
                    settings.urpLitMaterialOverride = EditorGUILayout.TextField(ContentStr_urpLitMaterialOverride, settings.urpLitMaterialOverride);

                    EditorGUILayout.Space(10);

                    if (GUILayout.Button("Reset to Defaults"))
                    {
                        settings.Humanoid = VoxelImporterSettings.default_Humanoid;
                        settings.ImportThumbnail = VoxelImporterSettings.default_ImportThumbnail;
                        settings.AttachCollider = VoxelImporterSettings.default_AttachCollider;
                        settings.ImportForURP = VoxelImporterSettings.default_ImportForURP;
                        settings.SplitAllGroups = VoxelImporterSettings.default_SplitAllGroups;
                        settings.IncludeRotation = VoxelImporterSettings.default_IncludeRotation;
                        settings.WithFreePose = VoxelImporterSettings.default_WithFreePose;
                        settings.OverrideSize = VoxelImporterSettings.default_OverrideSize;
                        settings.UseMeshRenderer = VoxelImporterSettings.default_UseMeshRenderer;
                        settings.FlattenGameObject = VoxelImporterSettings.default_FlattenGameObject;
                        settings.LightmapUV = VoxelImporterSettings.default_LightmapUV;
                        settings.CreateTexture = VoxelImporterSettings.default_CreateTexture;
                        settings.CreateVoxelInfo = VoxelImporterSettings.default_CreateVoxelInfo;
                        settings.CreateAnimator = VoxelImporterSettings.default_CreateAnimator;
                        settings.standardLitVertexColorMaterialOverride = VoxelImporterSettings.default_standardLitVertexColorMaterialOverride;
                        settings.urpLitVertexColorMaterialOverride = VoxelImporterSettings.default_urpLitVertexColorMaterialOverride;
                        settings.standardLitMaterialOverride = VoxelImporterSettings.default_standardLitMaterialOverride;
                        settings.urpLitMaterialOverride = VoxelImporterSettings.default_urpLitMaterialOverride;
                        settings.Save();
                    }


                    EditorGUIUtility.labelWidth = oldLabelWidth;

                    if (GUI.changed)
                    {
                        settings.Save();
                    }
                },
                keywords = new System.Collections.Generic.HashSet<string>(new[] {"vxp","voxel" })
            };

            return provider;
        }
    }
}

