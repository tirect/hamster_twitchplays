using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform _coreParent;
    [SerializeField] private Vector2 _coreStartPos;
    [SerializeField] private Transform _corePrefab;
    [Space]
    [SerializeField] private Transform _grassParent;
    
    private Sprite[] _coreSprites;
    private Sprite[] _grassSprites;
    private Sprite[] _grassSprites1;
    private Sprite[] _grassSprites2;
    private Sprite[] _grassSprites3;
    private const int _offset = 64;

    private void Awake()
    {
        _coreSprites = Resources.LoadAll<Sprite>("terra_tiled_core");
        _grassSprites3 = Resources.LoadAll<Sprite>("terra_tiled_grass_01");
        _grassSprites2 = Resources.LoadAll<Sprite>("terra_tiled_grass_02");
        _grassSprites1 = Resources.LoadAll<Sprite>("terra_tiled_grass_03");
        _grassSprites = Resources.LoadAll<Sprite>("terra_tiled_grass_04");
    }

    private void Start()
    {
        for (var i = 0; i < 12; ++i)
        {
            var localOffset = Vector2.right * _offset * i;
            var pos = _coreStartPos + localOffset;
            var pos2 = _coreStartPos + Vector2.down * _offset * 11 + localOffset;
            var text = i == 0 || i == 11 ? "" : i.ToString();
            CreateImage(_coreParent, pos, _coreSprites[i], text);
            CreateImage(_coreParent, pos2, _coreSprites[i + 12 * 11], text);
            if (i < 11)
            {
                localOffset = Vector2.down * _offset * (i + 1);
                pos = _coreStartPos + localOffset;
                pos2 = _coreStartPos + Vector2.right * _offset * 11 + localOffset;
                CreateImage(_coreParent, pos, _coreSprites[12 + i * 12], "");
                CreateImage(_coreParent, pos2, _coreSprites[23 + i * 12], "");
            }
        }

        var grassStartPos = _coreStartPos + Vector2.down * _offset + Vector2.right * _offset;
        var grassStartIdx = 13;
        for (var i = 0; i < 10; ++i)
        {
            for (var j = 0; j < 10; ++j)
            {
                var pos = grassStartPos + Vector2.right * _offset * i + Vector2.down * _offset * j;
                var sprite = _grassSprites[grassStartIdx + i + j * 12];
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
}
