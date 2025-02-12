using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleGridLayout : LayoutGroup
{
    // Enum to define different types of layout fitting options
    public enum FitType
    {
        Uniform,      // Both rows and columns are adjusted to fit equally
        Width,        // Fit based on the width of the container
        Height,       // Fit based on the height of the container
        FixedRows,    // Keep a fixed number of rows, and calculate columns accordingly
        FixedColumns  // Keep a fixed number of columns, and calculate rows accordingly
    }

    // Public fields to control the grid layout behavior
    public FitType fitType;         // Type of fitting strategy
    public int rows;                // Number of rows in the grid
    public int columns;             // Number of columns in the grid
    public Vector2 cellSize;        // Size of each individual cell in the grid
    public Vector2 spacing;         // Spacing between the cells in the grid

    // Flags to control whether the layout will fit horizontally or vertically
    public bool fitX;
    public bool fitY;

    // Calculate layout input for the horizontal axis (width)
    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        // Handle the case where fitType is Width, Height or Uniform
        if (fitType == FitType.Width || fitType == FitType.Height || fitType == FitType.Uniform)
        {
            fitX = true;
            fitY = true;

            // Calculate square root of child count to adjust rows and columns based on available space
            float sqrRt = Mathf.Sqrt(transform.childCount);
            rows = Mathf.CeilToInt(sqrRt);
            columns = Mathf.CeilToInt(sqrRt);
        }

        // Handle fixed column or width fitting
        if (fitType == FitType.Width || fitType == FitType.FixedColumns)
        {
            // Calculate the number of rows based on the total child count and the number of columns
            rows = Mathf.CeilToInt(transform.childCount / (float)columns);
        }

        // Handle fixed row or height fitting
        if (fitType == FitType.Height || fitType == FitType.FixedRows)
        {
            // Calculate the number of columns based on the total child count and the number of rows
            columns = Mathf.CeilToInt(transform.childCount / (float)rows);
        }

        // Dynamically get the width and height of the parent container
        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        // Calculate the width and height of each cell, taking into account the number of rows and columns
        float cellWidth = (parentWidth / columns) - spacing.x * (columns - 1) - (padding.left + padding.right) / columns;
        float cellHeight = (parentHeight / rows) - spacing.y * (rows - 1) - (padding.top + padding.bottom) / rows;

        // If fitX or fitY are enabled, adjust the cell size accordingly
        cellSize.x = fitX ? cellWidth : cellSize.x;
        cellSize.y = fitY ? cellHeight : cellSize.y;

        // Position each child element in the grid based on calculated positions
        int columnCount = 0;
        int rowCount = 0;

        // Loop through all the child elements and calculate their position and size
        for (int i = 0; i < rectChildren.Count; i++)
        {
            // Calculate row and column based on child index
            rowCount = i / columns;
            columnCount = i % columns;

            var item = rectChildren[i];

            // Calculate the X and Y position for each child item
            var xPos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
            var yPos = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;

            // Set the position and size for each child element
            SetChildAlongAxis(item, 0, xPos, cellSize.x); // Positioning child in horizontal axis (X)
            SetChildAlongAxis(item, 1, yPos, cellSize.y); // Positioning child in vertical axis (Y)
        }
    }

    // Calculate layout input for the vertical axis (height)
    public override void CalculateLayoutInputVertical()
    {
        // This method is not used in this layout, so it's empty.
    }

    // Custom method for setting the horizontal layout (not implemented in this case)
    public override void SetLayoutHorizontal()
    {
        // Custom horizontal layout logic can go here if needed.
    }

    // Custom method for setting the vertical layout (not implemented in this case)
    public override void SetLayoutVertical()
    {
        // Custom vertical layout logic can go here if needed.
    }
}
