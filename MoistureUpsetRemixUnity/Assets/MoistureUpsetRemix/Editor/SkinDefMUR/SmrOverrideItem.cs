using MoistureUpsetRemix.Common.Materials;
using UnityEngine;

namespace MoistureUpsetRemix.Editor.SkinDefMUR
{
    public struct SmrOverrideItem
    {
        public string SmrPath;
        public Mesh OriginalSmrMesh;
        public bool EnableMeshOverride;
        public Mesh MeshOverride;
        public HGStandardOverride MaterialOverride;

        public bool IsModified()
        {
            return EnableMeshOverride || MeshOverride;
        }
    }
}