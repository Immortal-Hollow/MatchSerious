using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleGridLayout : LayoutGroup
{
    // Enum to define different layout fitting types
    public enum FitType
    {
        Uniform,      // Both rows and columns have equal sizing
        Width,        // Fit based on the width of the parent
        Height,       // Fit based on the height of the parent
        FixedRows,    // Fixed number of rows
        FixedColumns  // Fixed number of columns
    }

    public FitType fitType;           // Determines the fitting type for the layout
    public int rows;                  // Number of rows in the grid layout
    public int columns;               // Number of columns in the grid layout
    public Vector2 cellSize;          // Size of each grid cell
    public Vector2 spacing;           // Spacing between the cells

    public bool fitX;                 // Boolean to control horizontal fit
    public bool fitY;                 // Boolean to control vertical fit

    // Override the method to calculate horizontal layout input
    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        // If the fit type is Width, Height, or Uniform, adjust both X and Y fit options
        if (fitType == FitType.Width || fitType == FitType.Height || fitType == FitType.Uniform)
        {
            fitX = true;
            fitY = true;

            // Calculate square root of child count to set rows and columns to a square grid
            float sqrRt = Mathf.Sqrt(transform.childCount);
            rows = Mathf.CeilToInt(sqrRt);   // Round up the number of rows
            columns = Mathf.CeilToInt(sqrRt); // Round up the number of columns
        }

        // Adjust rows if fitting by width or setting fixed columns
        if (fitType == FitType.Width || fitType == FitType.FixedColumns)
        {
            rows = Mathf.CeilToInt(transform.childCount / (float)columns);
        }

        // Adjust columns if fitting by height or setting fixed rows
        if (fitType == FitType.Height || fitType == FitType.FixedRows)
        {
            columns = Mathf.CeilToInt(transform.childCount / (float)rows);
        }

        // Get the parent container's width and height
        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        // Calculate the width and height for each cell
        float cellWidth = (parentWidth / (float)columns) - ((spacing.x / (float)columns) * 2) - (padding.left / (float)columns) - (padding.right / (float)columns);
        float cellHeight = (parentHeight / (float)rows) - ((spacing.y / (float)rows) * 2) - (padding.top / (float)rows) - (padding.bottom / (float)rows);

        // If the X or Y fit option is enabled, adjust the cell size accordingly
        cellSize.x = fitX ? cellWidth : cellSize.x;
        cellSize.y = fitY ? cellHeight : cellSize.y;

        // Variables to track the current row and column during layout
        int columnCount = 0;
        int rowCount = 0;

        // Loop through all child elements and position them based on grid layout
        for (int i = 0; i < rectChildren.Count; i++)
        {
            rowCount = i / columns; // Calculate the current row
            columnCount = i % columns; // Calculate the current column

            var item = rectChildren[i]; // Get the current UI element

            // Calculate the X and Y position based on row and column
            var xPos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
            var yPos = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;

            // Set the position of the child element along the X and Y axes
            SetChildAlongAxis(item, 0, xPos, cellSize.x);  // Set X position
            SetChildAlongAxis(item, 1, yPos, cellSize.y);  // Set Y position
        }
    }

    // Helper method to return an anchor position based on the TextAnchor enum
    private Vector2 GetAnchor(TextAnchor alignment)
    {
        // Switch statement to return appropriate anchor position for each TextAnchor value
        switch (alignment)
        {
            case TextAnchor.UpperLeft:
                return new Vector2(0, 1);
            case TextAnchor.UpperCenter:
                return new Vector2(0.5f, 1);
            case TextAnchor.UpperRight:
                return new Vector2(1, 1);
            case TextAnchor.MiddleLeft:
                return new Vector2(0, 0.5f);
            case TextAnchor.MiddleCenter:
                return new Vector2(0.5f, 0.5f);
            case TextAnchor.MiddleRight:
                return new Vector2(1, 0.5f);
            case TextAnchor.LowerLeft:
                return new Vector2(0, 0);
            case TextAnchor.LowerCenter:
                return new Vector2(0.5f, 0);
            case TextAnchor.LowerRight:
                return new Vector2(1, 0);
            default:
                return new Vector2(0.5f, 0.5f);  // Default center alignment
        }
    }

    // Override the method for vertical layout calculation (not used here but required by LayoutGroup)
    public override void CalculateLayoutInputVertical()
    {
        // No custom implementation for vertical layout calculation in this case
    }

    // Override the method to set horizontal layout (empty for now)
    public override void SetLayoutHorizontal()
    {
        // No custom implementation for horizontal layout in this case
    }

    // Override the method to set vertical layout (empty for now)
    public override void SetLayoutVertical()
    {
        // No custom implementation for vertical layout in this case
    }
}
