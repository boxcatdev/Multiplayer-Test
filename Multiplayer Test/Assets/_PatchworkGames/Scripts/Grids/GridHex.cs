using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PatchworkGames
{
    public static class GridHex
    {
        public static float HEX_SIZE = 1f;
        public static float HEX_POINT_WIDTH = 2 * HEX_SIZE;
        public static float HEX_POINT_HEIGHT = Mathf.Sqrt(3) * HEX_SIZE;

        private static float xOffset = 0.866f;
        private static float yPos = 0f;
        private static float zOffset = 0.5f;

        //private static float side = 0.5774f;

        /// Get XY
        public static Vector2Int GetXY(Vector3 position)
        {
            //starting rounded int
            Vector2Int rounded = GetRoundedInts(position);

            float dist0 = Vector3.Distance(position, GetHexWorldPosition3D(rounded.x, rounded.y));
            float dist1 = Vector3.Distance(position, GetHexWorldPosition3D(rounded.x, rounded.y + 1));
            float dist2 = Vector3.Distance(position, GetHexWorldPosition3D(rounded.x + 1, rounded.y + 1));
            float dist3 = Vector3.Distance(position, GetHexWorldPosition3D(rounded.x + 1, rounded.y));
            float dist4 = Vector3.Distance(position, GetHexWorldPosition3D(rounded.x, rounded.y - 1));
            float dist5 = Vector3.Distance(position, GetHexWorldPosition3D(rounded.x - 1, rounded.y));
            float dist6 = Vector3.Distance(position, GetHexWorldPosition3D(rounded.x - 1, -rounded.y + 1));

            //sorted dictionary of surrounding ints
            SortedDictionary<float, Vector2Int> sortedPoints = new SortedDictionary<float, Vector2Int>()
        {
            { Vector3.Distance(position, GetHexWorldPosition3D(rounded.x, rounded.y)), new Vector2Int(rounded.x, rounded.y) },
            { Vector3.Distance(position, GetHexWorldPosition3D(rounded.x, rounded.y + 1)), new Vector2Int(rounded.x, rounded.y + 1) },
            { Vector3.Distance(position, GetHexWorldPosition3D(rounded.x + 1, rounded.y + 1)), new Vector2Int(rounded.x + 1, rounded.y + 1) },
            { Vector3.Distance(position, GetHexWorldPosition3D(rounded.x + 1, rounded.y)), new Vector2Int(rounded.x + 1, rounded.y) },
            { Vector3.Distance(position, GetHexWorldPosition3D(rounded.x, rounded.y - 1)), new Vector2Int(rounded.x, rounded.y - 1) },
            { Vector3.Distance(position, GetHexWorldPosition3D(rounded.x - 1, rounded.y)), new Vector2Int(rounded.x - 1, rounded.y) },
            { Vector3.Distance(position, GetHexWorldPosition3D(rounded.x - 1, -rounded.y + 1)), new Vector2Int(rounded.x - 1, -rounded.y + 1) },
        };

            #region TryAdd
            //sortedPoints.TryAdd(dist0, new Vector2Int(rounded.x, rounded.y));
            //sortedPoints.TryAdd(dist1, new Vector2Int(rounded.x, rounded.y + 1));
            //sortedPoints.TryAdd(dist2, new Vector2Int(rounded.x + 1, rounded.y + 1));
            //sortedPoints.TryAdd(dist3, new Vector2Int(rounded.x + 1, rounded.y));

            //sortedPoints.TryAdd(dist4, new Vector2Int(rounded.x, rounded.y - 1));
            //sortedPoints.TryAdd(dist5, new Vector2Int(rounded.x - 1, rounded.y));
            //sortedPoints.TryAdd(dist6, new Vector2Int(rounded.x - 1, -rounded.y + 1));
            #endregion

            //return the first in the dictionary (should be smallest distance)
            return sortedPoints.Values.ElementAt(0);
        }

        /// Get Vector3

        public static Vector3 GetHexWorldPosition3D(Vector2Int gridXY)
        {
            float xPos = gridXY.x * xOffset;
            float yPos = gridXY.x % 2 == 0 ? gridXY.y + 0 : gridXY.y + zOffset;
            return new Vector3(xPos, GridHex.yPos, yPos);
        }
        public static Vector3 GetHexWorldPosition3D(int x, int y)
        {
            float xPos = x * xOffset;
            float yPos = x % 2 == 0 ? y + 0 : y + zOffset;
            return new Vector3(xPos, GridHex.yPos, yPos);
        }
        public static Vector3 GetHexWorldPosition3D(Vector3 position)
        {
            Vector2Int coordinatePosition = GetXY(position);
            return GetHexWorldPosition3D(coordinatePosition);
        }

        /// Private 
        private static Vector2Int GetRoundedInts(Vector3 position)
        {
            int x = Mathf.RoundToInt(position.x);
            int z = Mathf.RoundToInt(position.z);

            return new Vector2Int(x, z);
        }

    }

}
