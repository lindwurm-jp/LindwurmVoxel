using UnityEditor;
using UnityEngine;

namespace Lindwurm.Voxel.Editor
{
    static class VoxelImporterPreference
    {
        private const string PrefKey_EnableOption = "VoxelImporter.EnableOption";
        private const string PrefKey_DefaultScale = "VoxelImporter.DefaultScale";

        public const string PrefKey_Humanoid = "VoxelImporter.Humanoid";
        public const string PrefKey_ImportThumbnail = "VoxelImporter.ImportThumbnail";
        public const string PrefKey_AttachCollider = "VoxelImporter.AttachCollider";
        public const string PrefKey_ImportForURP = "VoxelImporter.ImportForURP";
        public const string PrefKey_SplitAllGroups = "VoxelImporter.SplitAllGroups";
        public const string PrefKey_IncludeRotation = "VoxelImporter.IncludeRotation";
        public const string PrefKey_WithFreePose = "VoxelImporter.WithFreePose";
        public const string PrefKey_OverrideSize = "VoxelImporter.OverrideSize";
        public const string PrefKey_UseMeshRenderer = "VoxelImporter.UseMeshRenderer";
        public const string PrefKey_FlattenGameObject = "VoxelImporter.FlattenGameObject";
        public const string PrefKey_LightmapUV = "VoxelImporter.LightmapUV";
        public const string PrefKey_CreateTexture = "VoxelImporter.CreateTexture";
        public const string PrefKey_CreateVoxelInfo = "VoxelImporter.CreateVoxelInfo";
        public const string PrefKey_CreateAnimator = "VoxelImporter.CreateAnimator";
        public const string PrefKey_standardLitVertexColorMaterialOverride = "VoxelImporter.standardLitVertexColorMaterialOverride";
        public const string PrefKey_urpLitVertexColorMaterialOverride = "VoxelImporter.urpLitVertexColorMaterialOverride";
        public const string PrefKey_standardLitMaterialOverride = "VoxelImporter.standardLitMaterialOverride";
        public const string PrefKey_urpLitMaterialOverride = "VoxelImporter.urpLitMaterialOverride";

        public const bool default_Humanoid = true;
        public const bool default_ImportThumbnail = true;
        public const bool default_AttachCollider = true;
        public const bool default_ImportForURP = true;
        public const bool default_SplitAllGroups = false;
        public const bool default_IncludeRotation = true;
        public const bool default_WithFreePose = false;
        public const float default_OverrideSize = 0f;
        public const bool default_UseMeshRenderer = false;
        public const bool default_FlattenGameObject = false;
        public const bool default_LightmapUV = false;
        public const bool default_CreateTexture = false;
        public const bool default_CreateVoxelInfo = true;
        public const bool default_CreateAnimator = false;
        public const string default_standardLitVertexColorMaterialOverride = "Shader Graphs/LitVertexColor";
        public const string default_urpLitVertexColorMaterialOverride = "Shader Graphs/URPLitVertexColor";
        public const string default_standardLitMaterialOverride = "Standard";
        public const string default_urpLitMaterialOverride = "Universal Render Pipeline/Lit";

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


        public static bool Humanoid
        {
            get => EditorPrefs.GetBool(PrefKey_Humanoid, default_Humanoid);
            set => EditorPrefs.SetBool(PrefKey_Humanoid, value);
        }
        public static bool ImportThumbnail
        {
            get => EditorPrefs.GetBool(PrefKey_ImportThumbnail, default_ImportThumbnail);
            set => EditorPrefs.SetBool(PrefKey_ImportThumbnail, value);
        }
        public static bool AttachCollider
        {
            get => EditorPrefs.GetBool(PrefKey_AttachCollider, default_AttachCollider);
            set => EditorPrefs.SetBool(PrefKey_AttachCollider, value);
        }
        public static bool ImportForURP
        {
            get => EditorPrefs.GetBool(PrefKey_ImportForURP, default_ImportForURP);
            set => EditorPrefs.SetBool(PrefKey_ImportForURP, value);
        }
        public static bool SplitAllGroups
        {
            get => EditorPrefs.GetBool(PrefKey_SplitAllGroups, default_SplitAllGroups);
            set => EditorPrefs.SetBool(PrefKey_SplitAllGroups, value);
        }
        public static bool IncludeRotation
        {
            get => EditorPrefs.GetBool(PrefKey_IncludeRotation, default_IncludeRotation);
            set => EditorPrefs.SetBool(PrefKey_IncludeRotation, value);
        }
        public static bool WithFreePose
        {
            get => EditorPrefs.GetBool(PrefKey_WithFreePose, default_WithFreePose);
            set => EditorPrefs.SetBool(PrefKey_WithFreePose, value);
        }
        public static float OverrideSize
        {
            get => EditorPrefs.GetFloat(PrefKey_OverrideSize, default_OverrideSize);
            set => EditorPrefs.SetFloat(PrefKey_OverrideSize, value);
        }
        public static bool UseMeshRenderer
        {
            get => EditorPrefs.GetBool(PrefKey_UseMeshRenderer, default_UseMeshRenderer);
            set => EditorPrefs.SetBool(PrefKey_UseMeshRenderer, value);
        }
        public static bool FlattenGameObject
        {
            get => EditorPrefs.GetBool(PrefKey_FlattenGameObject, default_FlattenGameObject);
            set => EditorPrefs.SetBool(PrefKey_FlattenGameObject, value);
        }
        public static bool LightmapUV
        {
            get => EditorPrefs.GetBool(PrefKey_LightmapUV, default_LightmapUV);
            set => EditorPrefs.SetBool(PrefKey_LightmapUV, value);
        }
        public static bool CreateTexture
        {
            get => EditorPrefs.GetBool(PrefKey_CreateTexture, default_CreateTexture);
            set => EditorPrefs.SetBool(PrefKey_CreateTexture, value);
        }
        public static bool CreateVoxelInfo
        {
            get => EditorPrefs.GetBool(PrefKey_CreateVoxelInfo, default_CreateVoxelInfo);
            set => EditorPrefs.SetBool(PrefKey_CreateVoxelInfo, value);
        }
        public static bool CreateAnimator
        {
            get => EditorPrefs.GetBool(PrefKey_CreateAnimator, default_CreateAnimator);
            set => EditorPrefs.SetBool(PrefKey_CreateAnimator, value);
        }
        public static string StandardLitVertexColorMaterialOverride
        {
            get => EditorPrefs.GetString(PrefKey_standardLitVertexColorMaterialOverride, default_standardLitVertexColorMaterialOverride);
            set => EditorPrefs.SetString(PrefKey_standardLitVertexColorMaterialOverride, value);
        }
        public static string UrpLitVertexColorMaterialOverride
        {
            get => EditorPrefs.GetString(PrefKey_urpLitVertexColorMaterialOverride, default_urpLitVertexColorMaterialOverride);
            set => EditorPrefs.SetString(PrefKey_urpLitVertexColorMaterialOverride, value);
        }
        public static string StandardLitMaterialOverride
        {
            get => EditorPrefs.GetString(PrefKey_standardLitMaterialOverride, default_standardLitMaterialOverride);
            set => EditorPrefs.SetString(PrefKey_standardLitMaterialOverride, value);
        }
        public static string UrpLitMaterialOverride
        {
            get => EditorPrefs.GetString(PrefKey_urpLitMaterialOverride, default_urpLitMaterialOverride);
            set => EditorPrefs.SetString(PrefKey_urpLitMaterialOverride, value);
        }

        [SettingsProvider]
        public static SettingsProvider CreateVoxelImporterProvider()
        {
            var provider = new SettingsProvider("Preferences/VoxelImporter", SettingsScope.User)
            {
                label = "VoxelImporter",
                guiHandler = searchContext =>
                {
                    EditorGUILayout.LabelField("VoxelImporter Default Settings", EditorStyles.boldLabel);

                    EditorGUIUtility.labelWidth = 250f;
                    Humanoid = EditorGUILayout.Toggle(ContentStr_Humanoid, Humanoid);
                    ImportThumbnail = EditorGUILayout.Toggle(ContentStr_ImportThumbnail, ImportThumbnail);
                    AttachCollider = EditorGUILayout.Toggle(ContentStr_AttachCollider, AttachCollider);
                    ImportForURP = EditorGUILayout.Toggle(ContentStr_ImportForURP, ImportForURP);
                    SplitAllGroups = EditorGUILayout.Toggle(ContentStr_SplitAllGroups, SplitAllGroups);
                    IncludeRotation = EditorGUILayout.Toggle(ContentStr_IncludeRotation, IncludeRotation);
                    WithFreePose = EditorGUILayout.Toggle(ContentStr_WithFreePose, WithFreePose);
                    OverrideSize = EditorGUILayout.FloatField(ContentStr_OverrideSize, OverrideSize);
                    UseMeshRenderer = EditorGUILayout.Toggle(ContentStr_UseMeshRenderer, UseMeshRenderer);
                    FlattenGameObject = EditorGUILayout.Toggle(ContentStr_FlattenGameObject, FlattenGameObject);
                    LightmapUV = EditorGUILayout.Toggle(ContentStr_LightmapUV, LightmapUV);
                    CreateTexture = EditorGUILayout.Toggle(ContentStr_CreateTexture, CreateTexture);
                    CreateVoxelInfo = EditorGUILayout.Toggle(ContentStr_CreateVoxelInfo, CreateVoxelInfo);
                    CreateAnimator = EditorGUILayout.Toggle(ContentStr_CreateAnimator, CreateAnimator);
                    StandardLitVertexColorMaterialOverride = EditorGUILayout.TextField(ContentStr_standardLitVertexColorMaterialOverride, StandardLitVertexColorMaterialOverride);
                    UrpLitVertexColorMaterialOverride = EditorGUILayout.TextField(ContentStr_urpLitVertexColorMaterialOverride, UrpLitVertexColorMaterialOverride);
                    StandardLitMaterialOverride = EditorGUILayout.TextField(ContentStr_standardLitMaterialOverride, StandardLitMaterialOverride);
                    UrpLitMaterialOverride = EditorGUILayout.TextField(ContentStr_urpLitMaterialOverride, UrpLitMaterialOverride);

                    if(GUILayout.Button("Reset to Defaults"))
                    {
                        Humanoid = default_Humanoid;
                        ImportThumbnail = default_ImportThumbnail;
                        AttachCollider = default_AttachCollider;
                        ImportForURP = default_ImportForURP;
                        SplitAllGroups = default_SplitAllGroups;
                        IncludeRotation = default_IncludeRotation;
                        WithFreePose = default_WithFreePose;
                        OverrideSize = default_OverrideSize;
                        UseMeshRenderer = default_UseMeshRenderer;
                        FlattenGameObject = default_FlattenGameObject;
                        LightmapUV = default_LightmapUV;
                        CreateTexture = default_CreateTexture;
                        CreateVoxelInfo = default_CreateVoxelInfo;
                        CreateAnimator = default_CreateAnimator;
                        StandardLitVertexColorMaterialOverride = default_standardLitVertexColorMaterialOverride;
                        UrpLitVertexColorMaterialOverride = default_urpLitVertexColorMaterialOverride;
                        StandardLitMaterialOverride = default_standardLitMaterialOverride;
                        UrpLitMaterialOverride = default_urpLitMaterialOverride;
                    }
                }
            };

            return provider;
        }
    }
}
