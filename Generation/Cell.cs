using UnityEngine.Serialization;

namespace VELDDev.BackroomsRenewed.Generation;

[System.Flags]
public enum WallFlags : byte
{
    None = 0,
    North = 1 << 0,  // +Z
    East  = 1 << 1,  // +X
    South = 1 << 2,  // -Z
    West  = 1 << 3   // -X
}

[Serializable]
public struct Cell : IEquatable<Cell>, INetworkSerializable
{
    public WallFlags walls;
    public Vector2Int position;
    
    public Cell()
    {
        walls = WallFlags.North | WallFlags.East | WallFlags.South | WallFlags.West;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref walls);
        serializer.SerializeValue(ref position);
    }

    public bool Equals(Cell? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return walls == other?.walls && position.Equals(other?.position);
    }

    public override bool Equals(object? obj)
    {
        return obj is Cell other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)walls, position);
    }

    public bool Equals(Cell other)
    {
        return walls == other.walls && position.Equals(other.position);
    }
}
