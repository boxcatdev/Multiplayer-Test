using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PatchworkGames
{
    public static class Utility
    {
        #region World Text
        public static TextMeshPro CreateWorldText3D(string value, float fontSize, Vector3 position, Quaternion rotation)
        {
            //create gameobject
            GameObject textObject = new GameObject("DebugDisplay");
            TextMeshPro textMesh = textObject.AddComponent<TextMeshPro>();
            RectTransform rectTransform = textObject.GetComponent<RectTransform>();

            //modify text settings
            textObject.transform.position = position;

            rectTransform.sizeDelta = new Vector2(1f, 0.5f);
            rectTransform.rotation = rotation;

            textMesh.fontSize = fontSize;
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.alignment = TextAlignmentOptions.Midline;

            //update string
            textMesh.text = value;

            return textMesh;
        }
        public static TextMeshPro CreateWorldText2D(string value, float fontSize, Vector3 position)
        {
            //create gameobject
            GameObject textObject = new GameObject("DebugDisplay");
            TextMeshPro textMesh = textObject.AddComponent<TextMeshPro>();
            RectTransform rectTransform = textObject.GetComponent<RectTransform>();

            //modify text settings
            textObject.transform.position = position;

            rectTransform.sizeDelta = new Vector2(1f, 0.5f);
            rectTransform.rotation = Quaternion.identity;

            textMesh.fontSize = fontSize;
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.alignment = TextAlignmentOptions.Midline;

            //update string
            textMesh.text = value;

            return textMesh;
        }
        #endregion

        #region Mouse Hits
        //get position
        public static Vector3 GetMouseHitPosition3D(Camera camera)
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                return hit.point;
            }
            else
            {
                return default;
            }
        }
        public static Vector3 GetMouseHitPosition3D(Camera camera, LayerMask layerMask)
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, ~layerMask))
            {
                return hit.point;
            }
            else
            {
                return default;
            }
        }
        public static Vector3 GetMouseHitPosition2D(Camera camera)
        {
            return camera.ScreenToWorldPoint(Input.mousePosition);
        }

        //get collider 3D
        public static Collider GetMouseHitCollider3D(Camera camera)
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                return hit.collider;
            }
            else
            {
                return null;
            }
        }
        public static Collider GetMouseHitCollider3D(Camera camera, LayerMask layerMask)
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, ~layerMask))
            {
                return hit.collider;
            }
            else
            {
                return null;
            }
        }

        //get collider 2D
        public static Collider2D GetMouseHitCollider2D(Camera camera)
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
            if (hit)
            {
                return hit.collider;
            }
            else
            {
                return null;
            }
        }
        public static Collider2D GetMouseHitCollider2D(Camera camera, LayerMask layerMask)
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, ~layerMask);
            if (hit)
            {
                return hit.collider;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Time Display
        public static string DisplayTimeMinutes(float timeToDisplay, bool countingUp)
        {
            if (countingUp)
            {
                if (timeToDisplay < 0)
                {
                    timeToDisplay = 0;
                }
            }
            else
            {
                if (timeToDisplay < 0)
                {
                    timeToDisplay += 1;
                }
            }

            float minutes = Mathf.FloorToInt(timeToDisplay / 60);
            float seconds = Mathf.FloorToInt(timeToDisplay % 60);
            //float milliseconds = timeToDisplay % 1 * 10; ;

            return string.Format("{0:00}:{1:00}", minutes, seconds); //:{2:00}
        }
        public static string DisplayTimeSeconds(float timeToDisplay, bool countingUp)
        {
            if (countingUp)
            {
                if (timeToDisplay < 0)
                {
                    timeToDisplay = 0;
                }
            }
            else
            {
                if (timeToDisplay < 0)
                {
                    timeToDisplay += 1;
                }
            }

            float minutes = Mathf.FloorToInt(timeToDisplay / 60);
            float seconds = Mathf.FloorToInt(timeToDisplay % 60);

            return string.Format("{0:00}", seconds + (minutes * 60));
        }
        #endregion

        #region Color
        public static Color ModifyColorAlpha(Color color, float transparency)
        {
            if (transparency > 1) transparency = 1;
            if (transparency < 0) transparency = 0;

            return new Color(color.r, color.g, color.b, transparency);

        }
        #endregion
    }
}
