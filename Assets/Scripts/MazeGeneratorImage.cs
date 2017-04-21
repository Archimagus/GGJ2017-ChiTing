using System;
using UnityEngine;
using UnityEngine.UI;

public enum PathType
{
	Wall,
	Path,
	Start1,
	Start2,
	Exit,
	Tresure,
	Enemy
}
[RequireComponent(typeof(Image))]
public class MazeGeneratorImage : MazeGenerator
{
	[SerializeField]
	private Texture2D _mapTexture;


	private Image _image;
	private Maze _maze = new Maze();

	private Image TheImage
	{
		get
		{
			if (_image == null)
				_image = GetComponent<Image>();
			return _image;
		}
	}

	void Start()
	{
		GenerateMaze();
	}


	void Update()
	{
		if (Input.GetKeyDown(KeyCode.G))
		{
			GenerateMaze();
		}
	}
	public override void GenerateMaze()
	{
		_maze.Generate(_mapSize, _minRunLength, Seed, 1);
		SetTexture();
	}
	public override void GenerateMaze(int seed)
	{
		Seed = seed;
		GenerateMaze();
	}

	private void SetTexture()
	{
		var t = _mapTexture;
		t.Resize(_mapSize.Width, _mapSize.Height);
		var pixels = t.GetPixels(0);

		int p = 0;
		foreach (var i in _maze)
		{
			switch (i)
			{
				case PathType.Wall:
					pixels[p] = Color.black;
					break;
				case PathType.Path:
					pixels[p] = Color.white;
					break;
				case PathType.Start1:
					pixels[p] = Color.green;
					break;
				case PathType.Start2:
					pixels[p] = Color.red;
					break;
				case PathType.Tresure:
					pixels[p] = Color.yellow;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			p++;
		}
		t.SetPixels(pixels);
		t.Apply();
		TheImage.sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f), 100);
	}
}