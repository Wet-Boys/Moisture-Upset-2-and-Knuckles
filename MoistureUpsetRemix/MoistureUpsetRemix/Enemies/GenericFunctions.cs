using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MoistureUpsetRemix.Enemies;

public class GenericFunctions
{
    internal static void ReplaceModel(string prefab, string mesh, string png, int position = 0, bool replaceothers = false)
        {
            var fab = Addressables.LoadAssetAsync<GameObject>(prefab).WaitForCompletion();
            var meshes = fab.GetComponentsInChildren<SkinnedMeshRenderer>();
            meshes[position].sharedMesh = Assets.Load<Mesh>(mesh);
            var texture = Assets.Load<Texture>(png);
            var blank = Assets.Load<Texture>("@MoistureUpset_na:assets/blank.png");
            DebugClass.Log($"====================={meshes[position].sharedMaterials.Length}  {meshes.Length}");
            for (int i = 0; i < meshes[position].sharedMaterials.Length; i++)
            {
                if (prefab == "RoR2/Base/Titan/TitanGoldBody.prefab")
                {
                    meshes[position].sharedMaterials[i].shader = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoBody.prefab").WaitForCompletion().GetComponentInChildren<SkinnedMeshRenderer>().material.shader;
                }
                else if (prefab == "RoR2/Base/Shopkeeper/ShopkeeperBody.prefab")
                {
                    meshes[position].sharedMaterials[i] = Assets.LoadMaterial(png);
                }
                meshes[position].sharedMaterials[i].color = Color.white;
                meshes[position].sharedMaterials[i].mainTexture = texture;
                meshes[position].sharedMaterials[i].SetTexture("_EmTex", blank);
                meshes[position].sharedMaterials[i].SetTexture("_NormalTex", null);
                if (png.Contains("frog"))
                {
                    meshes[position].sharedMaterials[i].SetTexture("_FresnelRamp", null);
                }
                if (png.Contains("shop"))
                {
                    meshes[position].sharedMaterials[i].SetTexture("_FlowHeightRamp", null);
                    meshes[position].sharedMaterials[i].SetTexture("_FlowHeightmap", null);
                }
                if (png.Contains("dankengine"))
                {
                    meshes[1].sharedMaterials[0].SetTexture("_MainTex", texture);
                    meshes[position].sharedMaterials[i].SetTexture("_FresnelRamp", null);
                    meshes[position].sharedMaterials[i].SetTexture("_FlowHeightRamp", null);
                    meshes[position].sharedMaterials[i].SetTexture("_FlowHeightmap", null);
                }
            }
            if (replaceothers)
            {
                for (int i = 0; i < meshes.Length; i++)
                {
                    if (i != position)
                    {
                        meshes[i].sharedMesh = Assets.Load<Mesh>(mesh);
                    }
                }
            }
        }
}