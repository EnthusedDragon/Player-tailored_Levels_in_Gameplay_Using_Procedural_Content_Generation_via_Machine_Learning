using UnityEngine;
using System.Collections;

public class HuntAndKillMazeAlgorithm : MazeAlgorithm
{

    private int currentRow = 0;
    private int currentColumn = 0;

    private bool courseComplete = false;

    string seed = "";

    public HuntAndKillMazeAlgorithm(MazeCell[,] mazeCells, string seed) : base(mazeCells)
    {
        this.seed = seed;
    }

    public override void CreateMaze()
    {
        HuntAndKill();
    }

    private bool HuntAndKill()
    {
        mazeCells[currentRow, currentColumn].visited = true;

        while (!courseComplete)
        {
            Kill(); // Will run until it hits a dead end.
            Hunt(); // Finds the next unvisited cell with an adjacent visited cell. If it can't find any, it sets courseComplete to true.
        }

        return true;
    }

    private void Kill()
    {
        while (RouteStillAvailable(currentRow, currentColumn))
        {
            int direction = ProceduralNumberGenerator.GetNextNumber(seed);

            if (direction == 1 && CellIsAvailable(currentRow - 1, currentColumn))
            {
                // Up
                if (DestroyWallIfItExists(mazeCells[currentRow, currentColumn].topWall))
                    mazeCells[currentRow, currentColumn].topWall = null;
                if (DestroyWallIfItExists(mazeCells[currentRow - 1, currentColumn].bottomWall))
                    mazeCells[currentRow - 1, currentColumn].bottomWall = null;
                currentRow--;
            }
            else if (direction == 2 && CellIsAvailable(currentRow + 1, currentColumn))
            {
                // Down
                if (DestroyWallIfItExists(mazeCells[currentRow, currentColumn].bottomWall))
                    mazeCells[currentRow, currentColumn].bottomWall = null;
                if (DestroyWallIfItExists(mazeCells[currentRow + 1, currentColumn].topWall))
                    mazeCells[currentRow + 1, currentColumn].topWall = null;
                currentRow++;
            }
            else if (direction == 3 && CellIsAvailable(currentRow, currentColumn + 1))
            {
                // Left
                if (DestroyWallIfItExists(mazeCells[currentRow, currentColumn].rightWall))
                    mazeCells[currentRow, currentColumn].rightWall = null;
                if (DestroyWallIfItExists(mazeCells[currentRow, currentColumn + 1].leftWall))
                    mazeCells[currentRow, currentColumn + 1].leftWall = null;
                currentColumn++;
            }
            else if (direction == 4 && CellIsAvailable(currentRow, currentColumn - 1))
            {
                // Right
                if (DestroyWallIfItExists(mazeCells[currentRow, currentColumn].leftWall))
                    mazeCells[currentRow, currentColumn].leftWall = null;
                if (DestroyWallIfItExists(mazeCells[currentRow, currentColumn - 1].rightWall))
                    mazeCells[currentRow, currentColumn - 1].rightWall = null;
                currentColumn--;
            }

            mazeCells[currentRow, currentColumn].visited = true;
        }
    }

    private void Hunt()
    {
        courseComplete = true; // Set it to this, and see if we can prove otherwise below!

        for (int r = 0; r < mazeRows; r++)
        {
            for (int c = 0; c < mazeColumns; c++)
            {
                if (!mazeCells[r, c].visited && CellHasAnAdjacentVisitedCell(r, c))
                {
                    courseComplete = false; // Yep, we found something so definitely do another Kill cycle.
                    currentRow = r;
                    currentColumn = c;
                    DestroyAdjacentWall(currentRow, currentColumn);
                    mazeCells[currentRow, currentColumn].visited = true;
                    return; // Exit the function
                }
            }
        }
    }


    private bool RouteStillAvailable(int row, int column)
    {
        int availableRoutes = 0;

        if (row > 0 && !mazeCells[row - 1, column].visited)
        {
            availableRoutes++;
        }

        if (row < mazeRows - 1 && !mazeCells[row + 1, column].visited)
        {
            availableRoutes++;
        }

        if (column > 0 && !mazeCells[row, column - 1].visited)
        {
            availableRoutes++;
        }

        if (column < mazeColumns - 1 && !mazeCells[row, column + 1].visited)
        {
            availableRoutes++;
        }

        return availableRoutes > 0;
    }

    private bool CellIsAvailable(int row, int column)
    {
        if (row >= 0 && row < mazeRows && column >= 0 && column < mazeColumns && !mazeCells[row, column].visited)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool DestroyWallIfItExists(GameObject wall)
    {
        if (wall != null)
        {
            GameObject.Destroy(wall);
            return true;
        }
        return false;
    }

    private bool CellHasAnAdjacentVisitedCell(int row, int column)
    {
        int visitedCells = 0;

        // Look 1 row up (north) if we're on row 1 or greater
        if (row > 0 && mazeCells[row - 1, column].visited)
        {
            visitedCells++;
        }

        // Look one row down (south) if we're the second-to-last row (or less)
        if (row < (mazeRows - 2) && mazeCells[row + 1, column].visited)
        {
            visitedCells++;
        }

        // Look one row left (west) if we're column 1 or greater
        if (column > 0 && mazeCells[row, column - 1].visited)
        {
            visitedCells++;
        }

        // Look one row right (east) if we're the second-to-last column (or less)
        if (column < (mazeColumns - 2) && mazeCells[row, column + 1].visited)
        {
            visitedCells++;
        }

        // return true if there are any adjacent visited cells to this one
        return visitedCells > 0;
    }

    private void DestroyAdjacentWall(int row, int column)
    {
        bool wallDestroyed = false;

        while (!wallDestroyed)
        {
            int direction = ProceduralNumberGenerator.GetNextNumber(seed);

            if (direction == 1 && row > 0 && mazeCells[row - 1, column].visited)
            {
                if (DestroyWallIfItExists(mazeCells[row, column].topWall))
                    mazeCells[row, column].topWall = null;
                if (DestroyWallIfItExists(mazeCells[row - 1, column].bottomWall))
                    mazeCells[row - 1, column].bottomWall = null;
                wallDestroyed = true;
            }
            else if (direction == 2 && row < (mazeRows - 2) && mazeCells[row + 1, column].visited)
            {
                if (DestroyWallIfItExists(mazeCells[row, column].bottomWall))
                    mazeCells[row, column].bottomWall = null;
                if (DestroyWallIfItExists(mazeCells[row + 1, column].topWall))
                    mazeCells[row + 1, column].topWall = null;
                wallDestroyed = true;
            }
            else if (direction == 3 && column > 0 && mazeCells[row, column - 1].visited)
            {
                if (DestroyWallIfItExists(mazeCells[row, column].leftWall))
                    mazeCells[row, column].leftWall = null;
                if (DestroyWallIfItExists(mazeCells[row, column - 1].rightWall))
                    mazeCells[row, column - 1].rightWall = null;
                wallDestroyed = true;
            }
            else if (direction == 4 && column < (mazeColumns - 2) && mazeCells[row, column + 1].visited)
            {
                if (DestroyWallIfItExists(mazeCells[row, column].rightWall))
                    mazeCells[row, column].rightWall = null;
                if (DestroyWallIfItExists(mazeCells[row, column + 1].leftWall))
                    mazeCells[row, column + 1].leftWall = null;
                wallDestroyed = true;
            }
        }

    }

}
