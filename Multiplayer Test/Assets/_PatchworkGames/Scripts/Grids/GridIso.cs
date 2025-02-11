using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PatchworkGames
{
    public static class GridIso
    {
        public const float Y_HALF = 0.29f; // 0.29f
        public const float X_HALF = 0.5f;
        public static float TILE_OFFSET { get; private set; } = 1.0f; //1.45f;

        public static Vector2 GetWorldPosition(Vector2Int gridPos)
        {
            return ConvertToWorld(gridPos.x, gridPos.y);
        }
        public static Vector2 GetWorldPosition(int x, int y)
        {
            return ConvertToWorld(x, y);
        }
        public static Vector2 GetWorldPosition(Vector2 position)
        {
            Vector2Int grid = ConvertToGrid(position);
            return ConvertToWorld(grid.x, grid.y);
        }
        public static Vector2Int GetGridPosition(Vector2 position)
        {
            return ConvertToGrid(position);
        }
        private static Vector2 ConvertToWorld(int x, int y)
        {
            #region Web
            /*float xPos = (x - y) * X_HALF;
            float yPos = (x + y) * Y_HALF;
            return new Vector2(xPos, yPos) * TILE_OFFSET;*/
            #endregion

            //my own
            float xPos = (y * X_HALF) + (x * X_HALF);
            float yPos = (y * Y_HALF) - (x * Y_HALF);

            return new Vector2(xPos * TILE_OFFSET, yPos * TILE_OFFSET);
        }
        private static Vector2Int ConvertToGrid(Vector2 pos)
        {
            float x = (pos.x / X_HALF + pos.y / Y_HALF) / 2f;
            float y = (pos.y / Y_HALF - (pos.x / X_HALF)) / 2f;
            return new Vector2Int(-Mathf.RoundToInt(y / TILE_OFFSET), Mathf.RoundToInt(x / TILE_OFFSET));
        }
    }
}

