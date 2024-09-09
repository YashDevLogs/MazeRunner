using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private MazeCell mazeCellPrefab;
    [SerializeField] private GameObject gasCanisterPrefab;
    [SerializeField] private int mazeWidth;
    [SerializeField] private int mazeDepth;
    private MazeCell[,] mazeGrid;
    [SerializeField] private int gasCanisterCount = 8;


    void Start()
    {
        mazeGrid = new MazeCell[mazeWidth, mazeDepth];

        for (int x = 0; x < mazeWidth; x++)
        {
            for (int z = 0; z < mazeDepth; z++)
            {
                mazeGrid[x, z] = Instantiate(mazeCellPrefab, new Vector3(x,0,z), Quaternion.identity);
            }
        }

        // Set starting point at (0,0)
        MazeCell startCell = mazeGrid[0, 0];
        startCell.ClearLeftWall(); // Remove left wall

        // Set finish point at (9,9)
        MazeCell finishCell = mazeGrid[mazeWidth - 1, mazeDepth - 1];
/*        finishCell.ClearRightWall(); // Remove right wall*/
        finishCell.ClearFrontWall(); // Remove front wall

        GenerateMaze(null, mazeGrid[0, 0]);
        PlaceGasCanisters();
       
    }

    private void PlaceGasCanisters()
    {
        for (int i = 0; i < gasCanisterCount; i++)
        {
            Vector3 randomPosition = GetRandomMazePosition();
            Instantiate(gasCanisterPrefab, randomPosition, Quaternion.identity);
        }
    }

    private Vector3 GetRandomMazePosition()
    {
        int randomX = Random.Range(0, mazeWidth);
        int randomZ = Random.Range(0, mazeDepth);

        // Cast a ray downwards from a high point to find the exact ground level
        Vector3 spawnPosition = new Vector3(randomX, 10f, randomZ); // Start raycasting from above
        RaycastHit hit;

        if (Physics.Raycast(spawnPosition, Vector3.down, out hit, Mathf.Infinity))
        {
            return new Vector3(randomX, hit.point.y + 0f, randomZ); // Adjust height slightly above ground
        }

        // Default to a fixed Y position if raycast fails
        return new Vector3(randomX, 0f, randomZ);
    }


    private void GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        MazeCell nextCell;

        do 
        {
            nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                 GenerateMaze(currentCell, nextCell);
            }
        }while (nextCell != null);

         


    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell) 
    {
        var unvisitedCell = GetUnvisitedCells(currentCell);

        return unvisitedCell.OrderBy(_ =>Random.Range(1,10)).FirstOrDefault();

    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {   
        int x = (int)currentCell.transform.position.x;
        int z = (int)currentCell.transform.position.z;

        if(x + 1 < mazeWidth)
        {
            var CellToRight = mazeGrid[x + 1, z];
            if(CellToRight.IsVisited == false)
            {
                yield return CellToRight;
            }
        }

        if(x - 1 >= 0)
        {
            var CellToLeft = mazeGrid[x - 1, z];
            if(CellToLeft.IsVisited == false)
            {
                yield return CellToLeft;
            }
        }

        if (z + 1 < mazeDepth)
        {
            var CellToFront= mazeGrid[x, z + 1];
            if (CellToFront.IsVisited == false)
            {
                yield return CellToFront;
            }
        }

        if (z - 1 >= 0)
        {
            var CellToBack = mazeGrid[x, z - 1];
            if (CellToBack.IsVisited == false)
            {
                yield return CellToBack;
            }
        }

    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null)
        {
            return;
        }

        if(previousCell.transform.position.x < currentCell.transform.position.x)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        }
        if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }

        if (previousCell.transform.position.z < currentCell.transform.position.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
        }
        if (previousCell.transform.position.z > currentCell.transform.position.z)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
        }

    }
}
