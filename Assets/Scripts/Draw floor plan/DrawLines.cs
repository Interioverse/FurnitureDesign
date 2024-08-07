using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DrawLines : MonoBehaviour
{
    public Transform gridPlane;
    public float lineThickness = 0.1f;
    public Color lineColor = Color.white;
    public float incrementSize = 0.5f; // Adjust this value in the inspector
    public Texture2D pointCursor, handCursor;

    bool isDrawing = false;
    public bool drawEnabled = false;
    public List<LineRenderer> lines = new List<LineRenderer>();
    List<TextMeshProUGUI> lengthTexts = new List<TextMeshProUGUI>();

    Stack<List<LineRenderer>> undoStack = new Stack<List<LineRenderer>>();
    Stack<List<LineRenderer>> redoStack = new Stack<List<LineRenderer>>();
    public WallGenerator wallGenerator;
    public Button generateWallButton;

    void OnEnable()
    {
        FloorDesignManager.OnDKeyPressed += StartDrawing;
        FloorDesignManager.OnMKeyPressed += StopDrawing;
        FloorDesignManager.OnEscapeKeyPressed += StopEverything;
    }

    private void Start()
    {
        generateWallButton.onClick.AddListener(() => wallGenerator.GenerateWallsOnLines(lines));
    }

    void OnDisable()
    {
        FloorDesignManager.OnDKeyPressed -= StartDrawing;
        FloorDesignManager.OnMKeyPressed -= StopDrawing;
        FloorDesignManager.OnEscapeKeyPressed -= StopEverything;
    }

    private void StopEverything()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        drawEnabled = false;
    }

    void StartDrawing()
    {
        if (!drawEnabled)
        {
            Cursor.SetCursor(pointCursor, Vector2.zero, CursorMode.Auto);
            drawEnabled = true;
        }
    }

    void StopDrawing()
    {
        if (drawEnabled)
        {
            Cursor.SetCursor(handCursor, Vector2.zero, CursorMode.Auto);
            drawEnabled = false;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && drawEnabled)
        {
            isDrawing = true;
            CreateNewLine();
        }

        if (Input.GetMouseButton(0) && isDrawing && drawEnabled)
        {
            UpdateCurrentLine();
        }

        if (Input.GetKeyDown(KeyCode.Z) && Input.GetKey(KeyCode.LeftControl))
        {
            UndoAction();
        }

        if (Input.GetKeyDown(KeyCode.Y) && Input.GetKey(KeyCode.LeftControl))
        {
            RedoAction();
        }
    }

    void CreateNewLine()
    {
        GameObject lineObject = new GameObject("Line");
        lineObject.transform.SetParent(transform); // Make the DrawLines object the parent of the line
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = lineThickness;
        lineRenderer.endWidth = lineThickness;
        lineRenderer.positionCount = 1;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // You can use a different material if needed
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.SetPosition(0, GetGridPosition());
        lines.Add(lineRenderer);

        redoStack.Clear();
    }

    //Creating one line for each comple draw
    void UpdateCurrentLine()
    {
        LineRenderer currentLine = lines[lines.Count - 1];
        Vector3 lastPosition = currentLine.GetPosition(currentLine.positionCount - 1);
        Vector3 newPosition = GetGridPosition();

        // Check if the new position is far enough from the last position based on the increment size
        if (Vector3.Distance(lastPosition, newPosition) >= incrementSize)
        {
            currentLine.positionCount++;
            currentLine.SetPosition(currentLine.positionCount - 1, newPosition);
        }
    }

    Vector3 GetGridPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.transform == gridPlane)
            {
                Vector3 gridPos = hit.point;
                gridPos.x = Mathf.Round(gridPos.x / incrementSize) * incrementSize;
                gridPos.y = Mathf.Round(gridPos.y / incrementSize) * incrementSize;
                gridPos.z = Mathf.Round(gridPos.z / incrementSize) * incrementSize; // Round to nearest increment
                return gridPos;
            }
        }

        return Vector3.zero;
    }

    float CalculateLineLength(LineRenderer line)
    {
        float length = 0f;
        for (int i = 0; i < line.positionCount - 1; i++)
        {
            length += Vector3.Distance(line.GetPosition(i), line.GetPosition(i + 1));
        }
        return length;
    }

    void UndoAction()
    {
        if (lines.Count > 0)
        {
            List<LineRenderer> lastLine = lines;
            undoStack.Push(lastLine);
            lines = new List<LineRenderer>(lines);
            lines.RemoveAt(lines.Count - 1);
            lastLine[lastLine.Count - 1].gameObject.SetActive(false);
        }
    }

    void RedoAction()
    {
        if (undoStack.Count > 0)
        {
            lines.Clear();
            List<LineRenderer> nextLine = undoStack.Pop();
            redoStack.Push(nextLine);
            foreach (LineRenderer line in nextLine)
            {
                // Reactivate the line renderer
                line.gameObject.SetActive(true);
                lines.Add(line);
            }
        }
    }
}