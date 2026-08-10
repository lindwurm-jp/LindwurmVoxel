using UnityEngine;
using UnityEditor;

namespace Lindwurm.Voxel.Editor
{
    [FilePath("ProjectSettings/VoxelImporterSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class VoxelImporterSettings : ScriptableSingleton<VoxelImporterSettings>
    {
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

        public bool Humanoid = default_Humanoid;
        public bool ImportThumbnail = default_ImportThumbnail;
        public bool AttachCollider = default_AttachCollider;
        public bool ImportForURP = default_ImportForURP;
        public bool SplitAllGroups = default_SplitAllGroups;
        public bool IncludeRotation = default_IncludeRotation;
        public bool WithFreePose = default_WithFreePose;
        public float OverrideSize = default_OverrideSize;
        public bool UseMeshRenderer = default_UseMeshRenderer;
        public bool FlattenGameObject = default_FlattenGameObject;
        public bool LightmapUV = default_LightmapUV;
        public bool CreateTexture = default_CreateTexture;
        public bool CreateVoxelInfo = default_CreateVoxelInfo;
        public bool CreateAnimator = default_CreateAnimator;
        public string standardLitVertexColorMaterialOverride = default_standardLitVertexColorMaterialOverride;
        public string urpLitVertexColorMaterialOverride = default_urpLitVertexColorMaterialOverride;
        public string standardLitMaterialOverride = default_standardLitMaterialOverride;
        public string urpLitMaterialOverride = default_urpLitMaterialOverride;

        public void Save()
        {
            Save(true);
        }

    }
}
