using System;
using System.Text;
using MoistureUpsetRemix.Common.AssetManagement;
using MoistureUpsetRemix.Common.Logging;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace MoistureUpsetRemix;

internal static class DebugClass
{
    public static void GetHierarchy(string name)
    {
        GameObject g = GameObject.Find(name);
        try
        {
            while (g.transform.parent.gameObject)
            {
                g = g.transform.parent.gameObject;
            }
        }
        catch (Exception)
        {
        }

        Log.Debug("nutting");
        Hierarchy(g, "");
    }

    public static void Hierarchy(GameObject g, string depth)
    {
        depth += "--";
        Log.Debug($"{depth}{g.name}");
        foreach (Transform item in g.transform)
        {
            Hierarchy(item.gameObject, depth);
        }
    }

    public static bool GetParent(string name)
    {
        GameObject g = GameObject.Find(name);
        try
        {
            Log.Debug($"-------------{g.transform.parent.gameObject}");
            return true;
        }
        catch (Exception)
        {
            Log.Debug($"-------------no parent");
            return false;
        }
    }

    public static void UIdebug()
    {
        Image[] objects = GameObject.FindObjectsOfType<Image>();
        Log.Debug($"--------------uicomponents----------------");
        foreach (var item in objects)
        {
            Log.Debug(item);
        }

        Log.Debug($"------------------------------------------");
    }

    // public static void UIdebugReplace()
    // {
    //     UnityEngine.UI.Image[] objects = GameObject.FindObjectsOfType<UnityEngine.UI.Image>();
    //     byte[] bytes = ByteReader.readbytes("MoistureUpset.Resources.roblox.png");
    //     Texture2D tex = new Texture2D(512, 512);
    //     tex.LoadImage(bytes);
    //     for (int i = 0; i < objects.Length; i++)
    //     {
    //         objects[i].sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f), 100);
    //     }
    // }
    public static void TextureDebugReplace()
    {
        var t = AssetManager.Load<Texture>("@MoistureUpset_noob:assets/Noob1TexLaser.png");
        SkinnedMeshRenderer[] objects = GameObject.FindObjectsOfType<SkinnedMeshRenderer>();
        for (int i = 0; i < objects.Length; i++)
        {
            Log.Debug($"------{objects[i].name}");
            if (objects[i].name == "golem")
            {
                foreach (var item in objects[i].sharedMaterials)
                {
                    item.mainTexture = t;
                    item.SetTexture("_EmTex", t);
                }
            }
            //objects[i] = t;
        }

        Log.Debug($"------end of list------");
    }

    public static void ListComponents(string name)
    {
        GameObject g = GameObject.Find(name);
        if (!g)
        {
            Log.Debug($"----------------components----------------");
            Log.Debug($"GameObject not found");
            Log.Debug($"------------------------------------------");
            return;
        }

        Log.Debug($"----------------components----------------");
        foreach (var item in g.GetComponents<Component>())
        {
            Log.Debug(item);
        }

        Log.Debug($"------------------------------------------");
    }

    public static void ListComponents(GameObject g)
    {
        if (!g)
        {
            Log.Debug($"----------------components----------------");
            Log.Debug($"GameObject not found");
            Log.Debug($"------------------------------------------");
            return;
        }

        Log.Debug($"----------------components----------------");
        foreach (var item in g.GetComponentsInChildren<Component>())
        {
            Log.Debug(item);
        }

        Log.Debug($"------------------------------------------");
    }

    public static void DebugBones(string resource, int pos = 0)
    {
        StringBuilder sb = new StringBuilder();
        var fab = Addressables.LoadAssetAsync<GameObject>(resource).WaitForCompletion();
        sb.Append($"{fab.ToString()}\n");
        var meshes = fab.GetComponentsInChildren<SkinnedMeshRenderer>();
        sb.Append($"rendererererer: {meshes[pos]}\n");
        sb.Append($"bone count: {meshes[pos].bones.Length}\n");
        sb.Append($"mesh count: {meshes.Length}\n");
        sb.Append($"root bone: {meshes[pos].rootBone.name}\n");
        sb.Append($"{resource}:\n");
        if (meshes[pos].bones.Length == 0)
        {
            sb.Append("No bones");
        }
        else
        {
            sb.Append("[");
            foreach (var bone in meshes[pos].bones)
            {
                sb.Append($"'{bone.name}', ");
            }

            sb.Remove(sb.Length - 2, 2);
            sb.Append("]");
        }

        sb.Append("\n\n");
        Log.Debug(sb.ToString());
    }

    public static void DebugBones(GameObject fab)
    {
        var meshes = fab.GetComponentsInChildren<SkinnedMeshRenderer>();
        StringBuilder sb = new StringBuilder();
        sb.Append($"rendererererer: {meshes[0]}\n");
        sb.Append($"bone count: {meshes[0].bones.Length}\n");
        sb.Append($"mesh count: {meshes.Length}\n");
        sb.Append($"root bone: {meshes[0].rootBone.name}\n");
        sb.Append($"{fab.ToString()}:\n");
        if (meshes[0].bones.Length == 0)
        {
            sb.Append("No bones");
        }
        else
        {
            sb.Append("[");
            foreach (var bone in meshes[0].bones)
            {
                sb.Append($"'{bone.name}', ");
            }

            sb.Remove(sb.Length - 2, 2);
            sb.Append("]");
        }

        sb.Append("\n\n");
        Log.Debug(sb.ToString());
    }

    public static void DebugBones(SkinnedMeshRenderer mesh)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("[");
        foreach (var bone in mesh.bones)
        {
            sb.Append($"'{bone.name}', ");
        }

        sb.Remove(sb.Length - 2, 2);
        sb.Append("]");
        Log.Debug(sb.ToString());
    }

    public static void GetAllGameObjects()
    {
        GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
        foreach (var item in objects)
        {
            ////Log.Debug($"-------sex----{item.name}");
            //if (item.name == "Mesh")
            //{
            //    try
            //    {
            //        DebugBones(item);
            //    }
            //    catch (Exception)
            //    {
            //    }
            //}
            Log.Debug($"-------------{item}");
        }
    }

    public static void GetAllTransforms()
    {
        Transform[] objects = GameObject.FindObjectsOfType<Transform>();
        foreach (var item in objects)
        {
            ////Log.Debug($"-------sex----{item.name}");
            //if (item.name == "Mesh")
            //{
            //    try
            //    {
            //        DebugBones(item);
            //    }
            //    catch (Exception)
            //    {
            //    }
            //}
            Log.Debug($"---{item}------parent:----{item.parent}");
        }
    }

    public static void GetAllGameObjects(GameObject g)
    {
        foreach (var item in g.GetComponents<GameObject>())
        {
            Log.Debug($"-------sex----{item}");
        }
    }
}