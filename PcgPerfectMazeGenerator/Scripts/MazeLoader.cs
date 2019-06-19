using System.Linq;
using UnityEngine;

public class MazeLoader : MonoBehaviour {
	public int mazeHeight, mazeWidth;
	public GameObject wall;
	public float size = 2f;
    public bool randomSeed = false;

    public string seed = "123424123342421432233144441212334432121223344";

    private MazeCell[,] mazeCells;

	// Use this for initialization
	void Start () {
        InitializeMaze ();

        if(randomSeed)
        {
            System.Random random = new System.Random();
            seed = new string(Enumerable.Repeat("1234", 45).Select(s => s[random.Next(s.Length)]).ToArray());
        }

		MazeAlgorithm ma = new HuntAndKillMazeAlgorithm (mazeCells, seed);
		ma.CreateMaze ();

        Debug.Log($"The maze loaded in {Time.realtimeSinceStartup} seconds.");
	}

	private void InitializeMaze() {

		mazeCells = new MazeCell[mazeHeight,mazeWidth];

        var mazeParent = Instantiate(new GameObject(), new Vector3(0f, 0f, 0f), Quaternion.identity);
        mazeParent.name = "Maze";

        for (int r = 0; r < mazeHeight; r++) {
			for (int c = 0; c < mazeWidth; c++) {
				mazeCells [r, c] = new MazeCell ();

				// For now, use the same wall object for the floor!
				mazeCells [r, c].floor = Instantiate (wall, new Vector3 (r*size, -(size/2f), c*size), Quaternion.identity) as GameObject;
				mazeCells [r, c].floor.name = "Floor " + r + "," + c;
				mazeCells [r, c].floor.transform.Rotate (Vector3.right, 90f);
                mazeCells[r, c].floor.tag = "Floor";
                mazeCells[r, c].floor.transform.parent = mazeParent.transform;
                if (c == 0) {
					mazeCells[r,c].westWall = Instantiate (wall, new Vector3 (r*size, 0, (c*size) - (size/2f)), Quaternion.identity) as GameObject;
					mazeCells[r, c].westWall.name = "West Wall " + r + "," + c;
                    mazeCells[r, c].westWall.tag = "Wall";
                    mazeCells[r, c].westWall.transform.parent = mazeParent.transform;
                }

				mazeCells[r, c].eastWall = Instantiate (wall, new Vector3 (r*size, 0, (c*size) + (size/2f)), Quaternion.identity) as GameObject;
				mazeCells[r, c].eastWall.name = "East Wall " + r + "," + c;
                mazeCells[r, c].eastWall.tag = "Wall";
                mazeCells[r, c].eastWall.transform.parent = mazeParent.transform;

                if (r == 0) {
					mazeCells[r, c].northWall = Instantiate (wall, new Vector3 ((r*size) - (size/2f), 0, c*size), Quaternion.identity) as GameObject;
					mazeCells[r, c].northWall.name = "North Wall " + r + "," + c;
					mazeCells[r, c].northWall.transform.Rotate (Vector3.up * 90f);
                    mazeCells[r, c].northWall.tag = "Wall";
                    mazeCells[r, c].northWall.transform.parent = mazeParent.transform;
                }

				mazeCells[r,c].southWall = Instantiate (wall, new Vector3 ((r*size) + (size/2f), 0, c*size), Quaternion.identity) as GameObject;
				mazeCells[r, c].southWall.name = "South Wall " + r + "," + c;
				mazeCells[r, c].southWall.transform.Rotate (Vector3.up * 90f);
                mazeCells[r, c].southWall.tag = "Wall";
                mazeCells[r, c].southWall.transform.parent = mazeParent.transform;
            }
		}

        // Set camera
        Vector3 camPos = mazeCells[Mathf.RoundToInt(mazeHeight / 2), Mathf.RoundToInt(mazeWidth / 2)].floor.transform.position;
        camPos.y = Camera.main.transform.position.y;
        Camera.main.transform.position = camPos;
    }
}
