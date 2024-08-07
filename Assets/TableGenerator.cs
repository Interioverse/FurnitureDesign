using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TableGenerator : MonoBehaviour
{
    public int numRows = 10;
    public int numColumns = 5;
    public GameObject cellPrefab;

    // Expose cell size for modification in the Inspector
    public Vector2 cellSize = new Vector2(100, 50);

    void Start()
    {
        GenerateTable();
    }

    void GenerateTable()
    {
        GridLayoutGroup gridLayout = gameObject.AddComponent<GridLayoutGroup>();

        // Use the public cellSize variable
        gridLayout.cellSize = cellSize;

        gridLayout.spacing = new Vector2(10, 10);   // Adjust the spacing between cells

        for (int col = 0; col < numColumns; col++)
        {
            GameObject columnObject = new GameObject("Column " + (col + 1));
            columnObject.transform.SetParent(transform);
            columnObject.AddComponent<GridLayoutGroup>();
            columnObject.GetComponent<GridLayoutGroup>().cellSize = cellSize;

            float yPos = -col * (cellSize.y + gridLayout.spacing.y);
            columnObject.transform.localPosition = new Vector3(0, yPos, 0);
            columnObject.transform.localScale = Vector3.one;

            for (int row = 0; row < numRows; row++)
            {
                GameObject cell = Instantiate(cellPrefab, columnObject.transform);
                TextMeshProUGUI cellText = cell.GetComponentInChildren<TextMeshProUGUI>();
                cellText.text = $"Row {row + 1}, Col {col + 1}";
            }
        }
    }
}