using System;

namespace MoistureUpsetRemix.Skins;

[Serializable]
public struct ModelSkinControllerProxy : IEquatable<ModelSkinControllerProxy>
{
    public string prefabAddressablePath;
    public string mscPath;

    public bool Equals(ModelSkinControllerProxy other)
    {
        return prefabAddressablePath == other.prefabAddressablePath && mscPath == other.mscPath;
    }

    public override bool Equals(object? obj)
    {
        return obj is ModelSkinControllerProxy other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(prefabAddressablePath, mscPath);
    }

    public static bool operator ==(ModelSkinControllerProxy left, ModelSkinControllerProxy right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ModelSkinControllerProxy left, ModelSkinControllerProxy right)
    {
        return !left.Equals(right);
    }
}