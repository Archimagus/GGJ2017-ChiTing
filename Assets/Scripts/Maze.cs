
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Maze : IEnumerable<PathType>
{
	private PathType[,] _mazeData;

	private Size _mapSize;
	private int _minRunLength;

	private int _dir;
	private int _traveled;
	private int _depth;
	private readonly bool _shortcut = false;
	private float _enemyPercent;

	public PathType this[int row, int col]
	{
		get { return _mazeData[row, col]; }
	}

	public void Generate(Size mapSize, int minRunLength, int seed, float enemyPercent)
	{
		_enemyPercent = enemyPercent;
		_mapSize = mapSize;
		_minRunLength = minRunLength;
		Random.InitState(seed);
		_mazeData = new PathType[_mapSize.Height, _mapSize.Width];
		int r;
		do
		{
			r = Random.Range(1, _mapSize.Height - 1);
		} while ((r % 2) == 0);

		int c;
		do
		{
			c = Random.Range(1, _mapSize.Width - 1);
		} while ((c % 2) == 0);

		_depth = 9;
		_traveled = _minRunLength;
		createPath(r, c, _minRunLength, 0);
		for (int i = 0; i < _mapSize.Height; i++)
		{
			if (_mazeData[i, 1] == PathType.Path || _mazeData[i, 1] == PathType.Enemy)
			{
				_mazeData[i, 1] = PathType.Start1;
				break;
			}
		}
		for (int i = 0; i < _mapSize.Height; i++)
		{
			if (_mazeData[i, _mapSize.Width - 2] == PathType.Path || _mazeData[i, _mapSize.Width - 2] == PathType.Enemy)
			{
				_mazeData[i, _mapSize.Width - 2] = PathType.Start2;
				break;
			}
		}
		int midle = _mapSize.Width / 2;
		for (int i = 0; i < _mapSize.Width / 2; i++)
		{
			if (_mazeData[_mapSize.Height - 2, midle + i] == PathType.Path || _mazeData[_mapSize.Height - 2, midle + i] == PathType.Enemy)
			{
				_mazeData[_mapSize.Height - 2, midle + i] = PathType.Exit;
				break;
			}
			if (_mazeData[_mapSize.Height - 2, midle - i] == PathType.Path || _mazeData[_mapSize.Height - 2, midle - i] == PathType.Enemy)
			{
				_mazeData[_mapSize.Height - 2, midle - i] = PathType.Exit;
				break;
			}
		}
		//var sb = new StringBuilder();
		//for (int row = 0; row < mapSize.Height; row++)
		//{
		//	for (int col = 0; col < mapSize.Width; col++)
		//	{
		//		var d = _mazeData[row, col];
		//		sb.Append((int)d);
		//	}
		//	sb.AppendLine();
		//}
		//File.WriteAllText(@"d:\temp\map.txt", sb.ToString());
	}
	private void createPath(int r, int c, int dist, int dir2)
	{
		_depth++;
		if (_shortcut || _depth > 1000000)
			return;

		if (r == 0 || r == _mapSize.Height - 1 || c == 0 || c == _mapSize.Width - 1)
			return;

		int walls = 0;
		try
		{
			walls = CountWalls(r, c);
		}
		catch (Exception)
		{
			Debug.LogError("Out of range " + r + "," + c);
		}

		if (3 == walls || 4 == walls)
		{
			//	if (walls == 4)
			//		_mazeData[r, c] = PathType.Start;
			//	else
			_mazeData[r, c] = PathType.Path;

			++_traveled;
		}
		else
		{
			return;
		}

		if (_traveled >= dist)
		{
			_dir = Random.Range(0, 4);
			dir2 = _dir;
			_traveled = 0;
		}

		while (true)
		{
			switch (_dir)
			{
				case 0:
					if (_mazeData[r + 1, c] == PathType.Wall)
						createPath(r + 1, c, dist, dir2);
					break;
				case 1:
					if (_mazeData[r - 1, c] == PathType.Wall)
						createPath(r - 1, c, dist, dir2);
					break;
				case 2:
					if (_mazeData[r, c + 1] == PathType.Wall)
						createPath(r, c + 1, dist, dir2);
					break;

				case 3:
					if (_mazeData[r, c - 1] == PathType.Wall)
						createPath(r, c - 1, dist, dir2);
					break;
			}
			++_dir;
			if (_dir > 3)
				_dir = 0;
			_traveled = 0;
			if (_dir == dir2)
			{
				if (_mazeData[r, c] == PathType.Path)
				{
					walls = CountWalls(r, c);
					if (3 == walls)
					{
						var val = Random.value;
						if (val < _enemyPercent)
							_mazeData[r, c] = PathType.Enemy;
					}
				}
				return;
			}
		}
	}

	private int CountWalls(int r, int c)
	{
		int walls = 0;
		if (_mazeData[r + 1, c] == PathType.Wall)
			walls++;
		if (_mazeData[r - 1, c] == PathType.Wall)
			walls++;
		if (_mazeData[r, c + 1] == PathType.Wall)
			walls++;
		if (_mazeData[r, c - 1] == PathType.Wall)
			walls++;
		return walls;
	}

	public IEnumerator<PathType> GetEnumerator()
	{
		foreach (var pathType in _mazeData)
		{
			yield return pathType;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}

[Serializable]
public struct Size
{
	public int Width;
	public int Height;
}