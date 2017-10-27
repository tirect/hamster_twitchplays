using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] private Sprite _hero;
	[SerializeField] private Sprite _nonHero;

	[Serializable]
	private struct CardInfo
	{
		public GameObject go;
		public int x;
		public int y;
		public Rect bounds;
	}

	[SerializeField] private CardInfo[] field;
	private CardInfo player;

	private int _fieldWidth = 3;

	private void Awake()
	{
		field = new CardInfo[_fieldWidth * _fieldWidth];
	}

	// Use this for initialization
	private void Start ()
	{
		var playerPos = new Vector2(0, 0);
		player = new CardInfo
		{
			go = CreateGameObjectWithSprite("Hero", playerPos, _hero),
			x = 1,
			y = 1,
			bounds = new Rect(playerPos - new Vector2(1.5f, 1.5f), new Vector2(3, 3))
		};

		var player_coord = player.x * _fieldWidth + player.y;

		var go = new GameObject("Non Heroes");
		int x = 0;
		int y = 0;
		for(var i = 0; i < 9; ++i)
		{
			var coord = x * _fieldWidth + y;
			var pos = new Vector2(x * 3 - 3, y * 3 - 3);
			field[coord] = new CardInfo
			{
				go = CreateGameObjectWithSprite("nonHero", pos, _nonHero, go.transform),
				x = x,
				y = y,
				bounds = new Rect(pos - new Vector2(1.5f, 1.5f), new Vector2(3, 3))
			};

			x++;
			if (x >= _fieldWidth)
			{
				y++;
				x = 0;
			}
		}
	}

	private static GameObject CreateGameObjectWithSprite(string name, Vector2 pos, Sprite sprite, Transform parent = null)
	{
		var go = new GameObject(name);
		go.transform.position = pos;
		go.transform.parent = parent;
		var s = go.AddComponent<SpriteRenderer>();
		s.sprite = sprite;

		return go;
	}

	private void Update()
	{
		foreach (var cardInfo in field)
		{
			var bounds_point1 = new Vector3(cardInfo.bounds.xMin, cardInfo.bounds.yMin, 0);
			var bounds_point2 = new Vector3(cardInfo.bounds.xMax, cardInfo.bounds.yMax, 0);
			var bounds_point3 = new Vector3(cardInfo.bounds.xMin, cardInfo.bounds.yMax, 0);
			var bounds_point4 = new Vector3(cardInfo.bounds.xMax, cardInfo.bounds.yMin, 0);

			Debug.DrawLine(bounds_point1, bounds_point3, Color.green);
			Debug.DrawLine(bounds_point3, bounds_point2, Color.green);
			Debug.DrawLine(bounds_point2, bounds_point4, Color.green);
			Debug.DrawLine(bounds_point1, bounds_point4, Color.green);
		}

		if (Input.GetMouseButtonUp(0))
		{
			var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			foreach (var cardInfo in field)
			{
				if (cardInfo.bounds.Contains(mousePos))
				{
					var x_change = Math.Abs(player.x - cardInfo.x);
					var y_change = Math.Abs(player.y - cardInfo.y);

					if ((x_change == 0 && y_change == 1) || (x_change == 1 && y_change == 0))
					{
						var card_idx = cardInfo.x * _fieldWidth + cardInfo.y;

						player.go.transform.position = field[card_idx].go.transform.position - Vector3.forward;
						player.x = cardInfo.x;
						player.y = cardInfo.y;
					}
				}
			}
		}
	}
}
