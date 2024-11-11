using PlayerManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridManager : MonoBehaviour
{
    private int _rows = 5;
    private int _cols = 5;

    public GameObject squarePrefab;

    // Stores the grid values in memory (0, 1, or 2)
    public int[,] gridValues;
    private GameObject[,] gridTiles;

    private List<Vector2Int> _excludedPositions = new List<Vector2Int>
    {
        new Vector2Int(0, 0),
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(4, 4),
        new Vector2Int(3, 4),
        new Vector2Int(4, 3)
    };

    private void Awake()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        gridValues = new int[_rows, _cols];
        gridTiles = new GameObject[_rows, _cols];

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _cols; col++)
            {
                gridValues[row, col] = 0;
                if (_excludedPositions.Contains(new Vector2Int(row, col)))
                {
                    gridValues[row, col] = -1;
                    continue;  // Skip this position
                }

                // Instantiate the prefab at the calculated position
                GameObject tile = Instantiate(squarePrefab, new Vector3(0.5f * row + col, -(row * 0.9f), 0), Quaternion.identity);
                gridTiles[row, col] = tile;
                tile.transform.parent = this.transform;
                // Turn off the penguin and otter images initially
                Transform penguinLogo = tile.transform.Find("penguin_logo");
                Transform otterLogo = tile.transform.Find("otter_logo");

                if (penguinLogo != null) penguinLogo.gameObject.SetActive(false);
                if (otterLogo != null) otterLogo.gameObject.SetActive(false);
            }
        }
    }


    public void HandleMapPlayerMoved(Vector2Int position, Vector2Int oldpos, bool isPlayer1)
    {
        // Deactivate the images at the old position and reset grid value
        GameObject oldTile = gridTiles[oldpos.x, oldpos.y];
        gridValues[oldpos.x, oldpos.y] = 0;

        Transform oldPenguinLogo = oldTile.transform.Find("penguin_logo");
        Transform oldOtterLogo = oldTile.transform.Find("otter_logo");
        if (oldPenguinLogo != null) oldPenguinLogo.gameObject.SetActive(false);
        if (oldOtterLogo != null) oldOtterLogo.gameObject.SetActive(false);

        // Update the new position with player-specific values and activate the correct image
        GameObject tile = gridTiles[position.x, position.y];
        if (isPlayer1)
        {
            gridValues[position.x, position.y] = 1;

            // Activate the penguin image and deactivate the otter image
            Transform penguinLogo = tile.transform.Find("penguin_logo");
            Transform otterLogo = tile.transform.Find("otter_logo");
            if (penguinLogo != null) penguinLogo.gameObject.SetActive(true);
            if (otterLogo != null) otterLogo.gameObject.SetActive(false);
        }
        else
        {
            gridValues[position.x, position.y] = 2;

            // Activate the otter image and deactivate the penguin image
            Transform penguinLogo = tile.transform.Find("penguin_logo");
            Transform otterLogo = tile.transform.Find("otter_logo");
            if (penguinLogo != null) penguinLogo.gameObject.SetActive(false);
            if (otterLogo != null) otterLogo.gameObject.SetActive(true);
        }
    }

    public bool CheckCoordinateValid(Vector2Int moveCoordinate)
    {
        // Check if the coordinate is within the grid boundaries
        if (moveCoordinate.x < 0 || moveCoordinate.x >= _rows || moveCoordinate.y < 0 || moveCoordinate.y >= _cols)
        {
            return false;
        }

        // Check if the coordinate is in the list of excluded positions
        if (_excludedPositions.Contains(moveCoordinate))
        {
            return false;
        }

        // If both checks pass, the coordinate is valid
        return true;
    }

}
