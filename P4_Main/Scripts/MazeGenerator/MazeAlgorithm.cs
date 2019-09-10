public abstract class MazeAlgorithm {
	public MazeCell[,] mazeCells;
	public int mazeRows, mazeColumns;

	protected MazeAlgorithm(MazeCell[,] mazeCells) : base() {
		this.mazeCells = mazeCells;
		mazeRows = mazeCells.GetLength(0);
        mazeColumns = mazeCells.GetLength(1);
    }

	public abstract void CreateMaze ();
}
