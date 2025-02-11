using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PatchworkGames
{
    public static class GridCube
    {
        public static Vector3 originPosition;
        public static float cellSize;

        /// Get XY
        public static Vector3Int GetXYZ(Vector3 worldPosition)
        {
            Vector3Int xyz = new Vector3Int();
            xyz.x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
            xyz.y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
            xyz.z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
            return xyz;
        }

        /// Get Vector3
        public static Vector3 GetWorldPosition3D(Vector3Int gridXYZ)
        {
            return new Vector3(gridXYZ.x, gridXYZ.y, gridXYZ.z) * cellSize + originPosition;
        }
        public static Vector3 GetWorldPosition3D(Vector3 position)
        {
            Vector3Int xyz = GetXYZ(position);
            return GetWorldPosition3D(xyz);
        }

        /// Get Center Vector3
        public static Vector3 GetCenterWorldPosition3D(Vector3Int gridPosition)
        {
            Vector3 tileCenter = GetWorldPosition3D(gridPosition);
            return tileCenter + new Vector3(cellSize * 0.5f, cellSize * 0.5f, cellSize * 0.5f);
        }
        public static Vector3 GetCenterWorldPosition3D(Vector3 position)
        {
            Vector3Int xyz = GetXYZ(position);
            Vector3 tileCenter = GetWorldPosition3D(xyz);
            return tileCenter + new Vector3(cellSize * 0.5f, cellSize * 0.5f, cellSize * 0.5f);
        }
    }
}

