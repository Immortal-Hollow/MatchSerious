using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleGridLayout : LayoutGroup
{
    public enum FitType
    {
        Uniform,
        Width,
        Height,
        FixedRows,
        FixedColumns
    }

    public FitType fitType;
    public int rows;
    public int columns;
    public Vector2 cellSize;
    public Vector2 spacing;

    public bool fitX;
    public bool fitY;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        if (fitType == FitType.Width || fitType == FitType.Height || fitType == FitType.Uniform)
        {
            fitX = true;
            fitY = true;

            // Adjust grid size based on child count
            float sqrRt = Mathf.Sqrt(transform.childCount);
            rows = Mathf.CeilToInt(sqrRt);
            columns = Mathf.CeilToInt(sqrRt);
        }

        if (fitType == FitType.Width || fitType == FitType.FixedColumns)
        {
            // Calculate rows based on column count
            rows = Mathf.CeilToInt(transform.childCount / (float)columns);
        }
        if (fitType == FitType.Height || fitType == FitType.FixedRows)
        {
            // Calculate columns based on row count
            columns = Mathf.CeilToInt(transform.childCount / (float)rows);
        }

        // Get the parent width and height dynamically
        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        // Calculate cell width and height based on parent size, rows, and columns
        float cellWidth = (parentWidth / columns) - spacing.x * (columns - 1) - (padding.left + padding.right) / columns;
        float cellHeight = (parentHeight / rows) - spacing.y * (rows - 1) - (padding.top + padding.bottom) / rows;

        // Ensure fitX and fitY work as expected
        cellSize.x = fitX ? cellWidth : cellSize.x;
        cellSize.y = fitY ? cellHeight : cellSize.y;

        // Position each item in the grid
        int columnCount = 0;
        int rowCount = 0;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            rowCount = i / columns;
            columnCount = i % columns;

            var item = rectChildren[i];

            // Calculate the position based on the cell size and spacing
            var xPos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
            var yPos = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;

            // Set position and size for the item
            SetChildAlongAxis(item, 0, xPos, cellSize.x);
            SetChildAlongAxis(item, 1, yPos, cellSize.y);
        }
    }

    public override void CalculateLayoutInputVertical()
    {
        // This is where vertical layout calculations happen if needed. We can leave it empty for now.
    }

    public override void SetLayoutHorizontal()
    {
        // This is where you would implement any custom horizontal layout logic.
    }

    public override void SetLayoutVertical()
    {
        // This is where you would implement any custom vertical layout logic.
    }
}
