using UnityEngine;

[System.Serializable]
public class MazeCell
{
    public bool visited = false;
    public bool startCell = false;
    public bool endCell = false;
    public CellType cellType;

    public GameObject cellParentAndLocation;
    public GameObject topWall;
    public GameObject bottomWall;
    public GameObject rightWall;
    public GameObject leftWall;

    public MazeCell topCell;
    public MazeCell bottomCell;
    public MazeCell rightCell;
    public MazeCell leftCell;

    public override string ToString()
    {
        return $"Top Wall: {topWall != null} \n" +
            $"Bottom Wall: {bottomWall != null} \n" +
            $"Left Wall: {leftWall != null} \n" +
            $"Right Wall: {rightWall != null} \n" +
            $"Top Cell: {topCell != null} \n" +
            $"Bottom Cell: {bottomCell != null} \n" +
            $"Left Cell: {leftCell != null} \n" +
            $"Right Cell: {rightCell != null} \n";
    }
}
