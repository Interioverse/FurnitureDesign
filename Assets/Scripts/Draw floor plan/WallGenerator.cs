using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    public GameObject wallPrefab; // Prefab of the wall to be generated
    public float wallHeight = 2f; // Height of the wall
    public float wallWidth = 0.1f; // Width of the wall
    public float growthDuration = 1f; // Duration over which the wall height will increase

    public void GenerateWallsOnLines(List<LineRenderer> lines)
    {
        foreach (LineRenderer line in lines)
        {
            if (line.transform.Find("Wall") != null)
            {
                continue;
            }

            Vector3 startPoint = line.GetPosition(0);
            Vector3 endPoint = line.GetPosition(line.positionCount - 1);
            Vector3 direction = (endPoint - startPoint).normalized;
            float distance = Vector3.Distance(startPoint, endPoint);

            Vector3 wallPosition = startPoint + direction * (distance / 2f);
            Quaternion wallRotation = Quaternion.LookRotation(direction);

            wallPosition += Vector3.up * (wallHeight / 2f);
            GameObject newWall = Instantiate(wallPrefab, wallPosition, wallRotation);

            newWall.transform.SetParent(line.transform);
            newWall.name = "Wall";

            StartCoroutine(IncreaseWallHeight(newWall.transform, distance));
        }
    }


    IEnumerator IncreaseWallHeight(Transform wallTransform, float distance)
    {
        float elapsedTime = 0f;
        Vector3 originalScale = wallTransform.localScale;
        Vector3 targetScale = new Vector3(wallWidth, 0f, distance); // Initial height is set to 0

        while (elapsedTime < growthDuration)
        {
            float t = elapsedTime / growthDuration;
            float newHeight = Mathf.Lerp(0f, wallHeight, t); // Smoothly increase the wall height
            targetScale.y = newHeight;
            targetScale.z = distance + wallWidth * (1f - t); // Adjust Z-axis scale
            wallTransform.localScale = targetScale;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        targetScale.y = wallHeight;
        targetScale.z = distance + wallWidth;
        wallTransform.localScale = targetScale;
    }

    //public GameObject wallPrefab; // Prefab of the wall to be generated
    //public float wallWidth = 0.1f; // Width of the wall

    //public void GenerateWallsOnLines(List<LineRenderer> lines)
    //{
    //    foreach (LineRenderer line in lines)
    //    {
    //        for (int i = 0; i < line.positionCount - 1; i++)
    //        {
    //            Vector3 startPoint = line.GetPosition(i);
    //            Vector3 endPoint = line.GetPosition(i + 1);
    //            Vector3 direction = (endPoint - startPoint).normalized;
    //            float distance = Vector3.Distance(startPoint, endPoint);

    //            // Calculate position and rotation for the wall
    //            Vector3 wallPosition = startPoint + direction * (distance / 2f);
    //            Quaternion wallRotation = Quaternion.LookRotation(direction);

    //            // Adjust position to be at half of the wall height
    //            wallPosition += Vector3.up * (wallWidth / 2f);

    //            // Instantiate the wall
    //            GameObject newWall = Instantiate(wallPrefab, wallPosition, wallRotation);

    //            // Set the wall's parent to be the line itself
    //            newWall.transform.SetParent(line.transform);

    //            newWall.transform.SetParent(line.transform);
    //            // Optionally, rename the wall
    //            newWall.name = "Wall";

    //            // Set the scale of the wall to match the distance between points
    //            newWall.transform.localScale = new Vector3(wallWidth, wallWidth, distance);
    //        }
    //    }
    //}
}
