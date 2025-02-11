using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PatchworkGames
{
    public static class GridSqr
    {
        public static Vector3 originPosition;
        public static float cellSize;

        /// Get XY
        public static Vector2Int GetXY(Vector3 worldPosition)
        {
            Vector2Int xy = new Vector2Int();
            xy.x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
            xy.y = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
            return xy;
        }

        /// Get Vector3
        public static Vector3 GetWorldPosition3D(Vector2Int gridXY, float yPosition)
        {
            return new Vector3(gridXY.x, yPosition, gridXY.y) * cellSize + originPosition;
        }
        public static Vector3 GetWorldPosition3D(Vector3 position, float yPosition)
        {
            Vector2Int xy = GetXY(position);
            return GetWorldPosition3D(xy, yPosition);
        }

        /// Get Center Vector3
        public static Vector3 GetCenterWorldPosition3D(Vector2Int gridPosition, float yPosition)
        {
            Vector3 tileCenter = GetWorldPosition3D(gridPosition, yPosition);
            return tileCenter + new Vector3(cellSize * 0.5f, 0, cellSize * 0.5f);
        }
        public static Vector3 GetCenterWorldPosition3D(Vector3 position, float yPosition)
        {
            Vector2Int xy = GetXY(position);
            Vector3 tileCenter = GetWorldPosition3D(xy, yPosition);
            return tileCenter + new Vector3(cellSize * 0.5f, 0, cellSize * 0.5f);
        }
    }

}
