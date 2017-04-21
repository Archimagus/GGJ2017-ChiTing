using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class MazeGenerator3D : MazeGenerator
{
	[SerializeField]
	private GameObject _levelExitPrefab;
	[SerializeField]
	private GameObject _enemyPrefab;

	[SerializeField]
	private float _scaleFactor = 1.0f;

	[SerializeField]
	[Range(0, 1)]
	private float _enemyPercent = 0.2f;

	private Maze _maze = new Maze();

	private MeshFilter _filter;
	private MeshFilter MeshFilter
	{
		get
		{
			if (_filter == null)
				_filter = GetComponent<MeshFilter>();
			return _filter;
		}
	}
	private MeshCollider _collider;
	private MeshCollider Collider
	{
		get
		{
			if (_collider == null)
				_collider = GetComponent<MeshCollider>();
			return _collider;
		}
	}
	void Awake()
	{
		if (_levelExitPrefab == null)
			Debug.LogError("No level exit prefab", this);
	}
	public override void GenerateMaze()
	{
		var s = Seed;
		if (s == -1)
			s = Random.Range(0, int.MaxValue);
		_maze.Generate(_mapSize, _minRunLength, s, _enemyPercent);
		GenerateMesh();
	}
	public override void GenerateMaze(int seed)
	{
		Seed = seed;
		GenerateMaze();
	}

	private void GenerateMesh()
	{
		while (transform.childCount > 0)
		{
			var c = transform.GetChild(0);
			c.SetParent(null);
			DestroyImmediate(c.gameObject);
		}

		List<Vector3> verts = new List<Vector3>();
		List<Vector2> uVs = new List<Vector2>();
		List<int> triangles = new List<int>();

		int height = 1;
		for (int r = 0; r < _mapSize.Height; r++)
		{
			for (int c = 0; c < _mapSize.Width; c++)
			{
				var block = _maze[r, c];
				if (block == PathType.Wall)
				{
					// Create the top
					CreateSide(c, height, r, block, Vector3.forward, Vector3.right, verts, uVs, triangles);

					// Create the front
					if (GetPathType(r + 1, c) != PathType.Wall)
						CreateSide(c, height, r + 1, block, -Vector3.up, Vector3.right, verts, uVs, triangles);

					// Create the back
					if (GetPathType(r - 1, c) != PathType.Wall)
						CreateSide(c + 1, height, r, block, -Vector3.up, -Vector3.right, verts, uVs, triangles);

					// Create the left
					if (GetPathType(r, c - 1) != PathType.Wall)
						CreateSide(c, height, r, block, -Vector3.up, Vector3.forward, verts, uVs, triangles);

					// Create the right
					if (GetPathType(r, c + 1) != PathType.Wall)
						CreateSide(c + 1, height, r + 1, block, -Vector3.up, -Vector3.forward, verts, uVs, triangles);
				}
				else
				{
					// Create the Bottom
					CreateSide(c, 0, r, block, Vector3.forward, Vector3.right, verts, uVs, triangles);
					switch (block)
					{
						case PathType.Wall:
							break;
						case PathType.Path:
							break;
						case PathType.Tresure:
							break;
						case PathType.Start1:
							{
								var go = new GameObject("Player 1 Start", typeof(NetworkStartPosition));
								go.transform.position = new Vector3((c + 0.5f) * _scaleFactor, 0.5f * _scaleFactor, (r + 0.5f) * _scaleFactor);
								go.transform.SetParent(transform);
								break;
							}
						case PathType.Start2:
							{
								var go = new GameObject("Player 2 Start", typeof(NetworkStartPosition));
								go.transform.position = new Vector3((c + 0.5f) * _scaleFactor, 0.5f * _scaleFactor, (r + 0.5f) * _scaleFactor);
								go.transform.SetParent(transform);
								break;
							}
						case PathType.Exit:
							{
								Instantiate(_levelExitPrefab, new Vector3((c + 0.5f) * _scaleFactor, 0, (r + 0.5f) * _scaleFactor), Quaternion.identity, transform);
								break;
							}
						case PathType.Enemy:
							{
								Instantiate(_enemyPrefab, new Vector3((c + 0.5f) * _scaleFactor, 0, (r + 0.5f) * _scaleFactor), Quaternion.identity, transform);

								break;
							}
						default:
							throw new System.ArgumentOutOfRangeException();
					}
				}
			}
		}
		var mesh = new Mesh
		{
			name = "The world",
			vertices = verts.ToArray(),
			uv = uVs.ToArray(),
			triangles = triangles.ToArray()
		};

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		MeshFilter.mesh = mesh;
		Collider.sharedMesh = mesh;
	}

	private PathType GetPathType(int r, int c)
	{
		if (r < 0 || r >= _mapSize.Height || c < 0 || c >= _mapSize.Width)
			return PathType.Path;
		return _maze[r, c];
	}

	private void CreateSide(float x, float y, float z, PathType block, Vector3 up, Vector3 right, List<Vector3> verts, List<Vector2> uVs, List<int> triangles)
	{
		x *= _scaleFactor;
		y *= _scaleFactor;
		z *= _scaleFactor;
		int first = verts.Count;
		verts.Add(new Vector3(x, y, z));
		verts.Add(new Vector3(x, y, z) + up * _scaleFactor);
		verts.Add(new Vector3(x, y, z) + up * _scaleFactor + right * _scaleFactor);
		verts.Add(new Vector3(x, y, z) + right * _scaleFactor);


		triangles.Add(first);
		triangles.Add(first + 1);
		triangles.Add(first + 2);
		triangles.Add(first);
		triangles.Add(first + 2);
		triangles.Add(first + 3);


		float uvBlockSize = 0.5f;
		float uvOffset = (int)0 * uvBlockSize;
		uVs.Add(new Vector2(uvOffset, 1));
		uVs.Add(new Vector2(uvOffset, 0));
		uVs.Add(new Vector2(uvOffset + uvBlockSize, 0));
		uVs.Add(new Vector2(uvOffset + uvBlockSize, 1));
	}
}
