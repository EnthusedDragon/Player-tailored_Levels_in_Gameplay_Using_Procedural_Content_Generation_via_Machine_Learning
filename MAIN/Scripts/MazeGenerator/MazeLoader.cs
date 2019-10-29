using UnityEngine;
using System.Collections;

public class MazeLoader : MonoBehaviour {

    public Camera gameCam;

    public GameObject Academy;
    public GameObject Player;

    public bool randomSize;
	public int mazeRows, mazeColumns;
    public GameObject Wall;
    public GameObject StartCell;
    public GameObject EndCell;
    public GameObject wall;
	public float CellSize = 5.4f;
    public bool randomSeed = false;
    public string seed = "12341234123412341234";

	private MazeCell[,] mazeCells;

	// Use this for initialization
	public void Start ()
    {
		InitializeMaze ();

        //MazeAlgorithm ma = new HuntAndKillMazeAlgorithm (mazeCells, seed);
		//if(ma.CreateMaze ())
  //      {
  //          Academy.SetActive(true);
  //          Player.SetActive(true);
  //      }

	}
	
	private void InitializeMaze()
    {

        if (randomSize)
        {
            mazeRows = Random.Range(1, 20) + 1;
            mazeColumns = Random.Range(1, 20) + 1;
        }

        mazeCells = new MazeCell[mazeRows, mazeColumns];

        for (int r = 0; r < mazeRows; r++)
        {
            for (int c = 0; c < mazeColumns; c++)
            {
                mazeCells[r, c] = new MazeCell();

                if (c == 0)
                {
                    mazeCells[r, c].leftWall = Instantiate(Wall, new Vector3(r * CellSize, 0, (c * CellSize) - (CellSize / 2f)), Quaternion.identity, transform) as GameObject;
                    mazeCells[r, c].leftWall.name = "West Wall " + r + "," + c;
                }

                mazeCells[r, c].rightWall = Instantiate(Wall, new Vector3(r * CellSize, 0, (c * CellSize) + (CellSize / 2f)), Quaternion.identity, transform) as GameObject;
                mazeCells[r, c].rightWall.name = "East Wall " + r + "," + c;

                if (r == 0)
                {
                    mazeCells[r, c].topWall = Instantiate(Wall, new Vector3((r * CellSize) - (CellSize / 2f), 0, c * CellSize), Quaternion.identity, transform) as GameObject;
                    mazeCells[r, c].topWall.name = "North Wall " + r + "," + c;
                    mazeCells[r, c].topWall.transform.Rotate(Vector3.up * 90f);
                }

                mazeCells[r, c].bottomWall = Instantiate(Wall, new Vector3((r * CellSize) + (CellSize / 2f), 0, c * CellSize), Quaternion.identity, transform) as GameObject;
                mazeCells[r, c].bottomWall.name = "South Wall " + r + "," + c;
                mazeCells[r, c].bottomWall.transform.Rotate(Vector3.up * 90f);
            }
        }

        var startLocation = mazeCells[0, 0].bottomWall.transform.position;
        startLocation.x -= 2.7f;
        startLocation.y = -3;
        var endLocation = mazeCells[mazeRows - 1, mazeColumns - 1].bottomWall.transform.position;
        endLocation.x -= 2.7f;
        endLocation.y = -3;

        var middle = Vector3.Lerp(startLocation, endLocation, 0.5f);

        gameCam.transform.position = new Vector3(middle.x-1, 10, middle.z);
        gameCam.orthographicSize = ((mazeRows + mazeColumns) / 2) * 3;

        StartCell.transform.position = startLocation;
        EndCell.transform.position = endLocation;

        //Debug.Log("Initialize Done");
        
        if (randomSeed)
        {
            var newSeed = "";
            while(newSeed.Length < 20)
            {
                newSeed += (Random.Range(0, 4)+1).ToString();

                if(newSeed.Length == 20)
                {
                    // validate seed
                    //// Contains at least one of each [1,2,3,4]
                    if (!newSeed.Contains("1") || !newSeed.Contains("2") || !newSeed.Contains("3") || !newSeed.Contains("4"))
                    {
                        newSeed = "";
                        //Debug.Log($"Invalid Seed");
                    }
                }
            }
            seed = newSeed;
            //Debug.Log($"Random Seed: {seed}");
        }
    }
}
