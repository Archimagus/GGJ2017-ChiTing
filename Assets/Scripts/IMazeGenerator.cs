using UnityEngine;
public abstract class MazeGenerator : MonoBehaviour
{
	[SerializeField]
	protected Size _mapSize = new Size { Width = 200, Height = 200 };
	[SerializeField]
	protected int _minRunLength = 3;

	public int Seed = -1;


	public abstract void GenerateMaze();
	public abstract void GenerateMaze(int seed);

}
