using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    public GameObject linePrefab;
    private LineRenderer currentLine;
    private Vector3 lastMousePos;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartLine();
        }
        if (Input.GetMouseButton(0))
        {
            UpdateLine();
        }
    }

    void StartLine()
    {
        GameObject newLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        currentLine = newLine.GetComponent<LineRenderer>();
        currentLine.positionCount = 0; // Clear default points
    }

    void UpdateLine()
    {
        Vector3 mousePos = Input.mousePosition; // Convert mouse position to world space
        mousePos.z = 10f; // Distance from camera to 2D plane
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos); // Translates pixels into x/y units used by sprites
        worldPos.z = -1; // Force it onto 2D plane

        // Only add point once mouse has moved enough, preventing adding points at exact same location if mouse isn't moving
        if (Vector3.Distance(worldPos, lastMousePos) > 0.1f)
        {
            currentLine.positionCount++;
            currentLine.SetPosition(currentLine.positionCount - 1, worldPos);
            lastMousePos = worldPos;
        }
    }
}
