using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	[SerializeField] private Transform _coreParent;
    [SerializeField] private Transform _corePrefab;
    [Space]
    [SerializeField] private Transform _grassParent;

	[Space] [SerializeField] private string[] _horizontalLetters;

	[Space]
	[SerializeField] private Transform _playerPrefab;
	[SerializeField] private Transform _playersParent;
    
    private Sprite[] _coreSprites;
    private Sprite[] _grassSprites;
    private Sprite[] _grassSprites1;
    private Sprite[] _grassSprites2;
    private Sprite[] _grassSprites3;

	private List<PlayerController> _players;
	private int[] _playerStartPositions;

	private void Awake()
	{
		_coreSprites = Resources.LoadAll<Sprite>("terra_tiled_core");
		_grassSprites3 = Resources.LoadAll<Sprite>("terra_tiled_grass_01");
		_grassSprites2 = Resources.LoadAll<Sprite>("terra_tiled_grass_02");
		_grassSprites1 = Resources.LoadAll<Sprite>("terra_tiled_grass_03");
		_grassSprites = Resources.LoadAll<Sprite>("terra_tiled_grass_04");
		_playerStartPositions = new int[40];
		for (var i = 0; i < 10; ++i)
		{
			_playerStartPositions[i] = i+1;
			_playerStartPositions[i + 10] = i + 1 + 12 * 11;
			_playerStartPositions[i + 20] = 12 + i * 12;
			_playerStartPositions[i + 30] = 23 + i * 12;
		}
		_players = new List<PlayerController>();
	}

    private void Start()
    {
        for (var i = 0; i < 12; ++i)
        {
            var pos = Utils.GetPositionFromIdx(i);
            var pos2 = Utils.GetPositionFromIdx(i+12*11);
            var text = i == 0 || i == 11 ? "" : _horizontalLetters[i-1];
            CreateImage(_coreParent, pos, _coreSprites[i], text);
            CreateImage(_coreParent, pos2, _coreSprites[i + 12 * 11], text);
            if (i < 10)
            {
                pos = Utils.GetPositionFromIdx(12+i*12);
                pos2 = Utils.GetPositionFromIdx(23+i*12);
	            text = (i + 1).ToString();
                CreateImage(_coreParent, pos, _coreSprites[12 + i * 12], text);
                CreateImage(_coreParent, pos2, _coreSprites[23 + i * 12], text);
            }
        }

        const int grassStartIdx = 13;
        for (var i = 0; i < 10; ++i)
        {
            for (var j = 0; j < 10; ++j)
            {
	            var idx = grassStartIdx + i + j * 12;
	            var pos = Utils.GetPositionFromIdx(idx);
                var sprite = _grassSprites[idx];
                CreateImage(_grassParent, pos, sprite, "");
            }
        }
    }

    private Transform CreateImage(Transform parent, Vector2 pos, Sprite sprite, string text)
    {
        var g = Instantiate(_corePrefab, parent);
        g.localPosition = pos;
        g.GetComponent<Image>().sprite = sprite;
        g.GetComponentInChildren<Text>().text = text;

        return g;
    }

	public void PlayerJoin()
	{
		var position = 0;
		var placed = false;
		if (_players.Count == 0)
		{
			position = _playerStartPositions[0];
			placed = true;
		}
		else
		{
			foreach (var playerStartPosition in _playerStartPositions)
			{
				var taken = _players.Any(player => player.Position == playerStartPosition);
				if (taken) continue;
				position = playerStartPosition;
				placed = true;
				break;
			}
		}
		if (!placed)
			Debug.Log("There is no more space for new player");
		else
		{
			var p = Instantiate(_playerPrefab, _playersParent);
			p.transform.localPosition = Utils.GetPositionFromIdx(position);

			var playerController = p.GetComponent<PlayerController>();
			playerController.Position = position;
			playerController.Setup();

			_players.Add(playerController);
		}
	}
}
