using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SnakeController : MonoBehaviour
{
	[SerializeField] private Transform _bodyPrefab;
	[SerializeField] private Transform _headPrefab;
	[SerializeField] private Transform _tailPrefab;

	[SerializeField] private int _startIdx;
	[SerializeField] private float _tickTime;

	private float _time = 0.0f;
	private int _currentPosition;

	private Transform _head;
	private List<Transform> _body;
	private Transform _tail;

	private bool _addBody = false;

	private struct IdxDirection
	{
		public int idx;
		public Direction dir;

		public override string ToString()
		{
			return "idx = " + idx + ", direction = " + dir;
		}
	}

	private enum Direction
	{
		Left, Right, Up, Down
	}

	private Direction _currentDirection;

	private void Awake()
	{
		_body = new List<Transform>();

		_head = Instantiate(_headPrefab, transform);
		_body.Add(Instantiate(_bodyPrefab, transform));
		_tail = Instantiate(_tailPrefab, transform);

		var bodyIdx = Utils.GetDownIdx(_startIdx);
		_head.localPosition = Utils.GetPositionFromIdx(_startIdx);
		_body[0].localPosition = Utils.GetPositionFromIdx(bodyIdx);
		_tail.localPosition = Utils.GetPositionFromIdx(Utils.GetDownIdx(bodyIdx));
		_currentPosition = _startIdx;
		_currentDirection = Direction.Up;
	}

	private void Update()
	{
		_time += Time.deltaTime;
		if (_time >= _tickTime)
		{
			Move();
			_time = 0;

			if (Random.Range(0, 100) < 30)
				_addBody = true;
		}
	}

	private void Move()
	{
		var possibleMoveIdx = new IdxDirection[3];
		switch (_currentDirection)
		{
			case Direction.Left:
				possibleMoveIdx[0] = new IdxDirection {idx = Utils.GetLeftIdx(_currentPosition), dir = Direction.Left};
				possibleMoveIdx[1] = new IdxDirection {idx = Utils.GetUpIdx(_currentPosition), dir = Direction.Up};
				possibleMoveIdx[2] = new IdxDirection {idx = Utils.GetDownIdx(_currentPosition), dir = Direction.Down};
				break;
			case Direction.Right:
				possibleMoveIdx[0] = new IdxDirection { idx = Utils.GetRightIdx(_currentPosition), dir = Direction.Right };
				possibleMoveIdx[1] = new IdxDirection { idx = Utils.GetUpIdx(_currentPosition), dir = Direction.Up };
				possibleMoveIdx[2] = new IdxDirection { idx = Utils.GetDownIdx(_currentPosition), dir = Direction.Down };
				break;
			case Direction.Up:
				possibleMoveIdx[0] = new IdxDirection { idx = Utils.GetRightIdx(_currentPosition), dir = Direction.Right };
				possibleMoveIdx[1] = new IdxDirection { idx = Utils.GetUpIdx(_currentPosition), dir = Direction.Up };
				possibleMoveIdx[2] = new IdxDirection { idx = Utils.GetLeftIdx(_currentPosition), dir = Direction.Left };
				break;
			case Direction.Down:
				possibleMoveIdx[0] = new IdxDirection { idx = Utils.GetRightIdx(_currentPosition), dir = Direction.Right };
				possibleMoveIdx[1] = new IdxDirection { idx = Utils.GetLeftIdx(_currentPosition), dir = Direction.Left };
				possibleMoveIdx[2] = new IdxDirection { idx = Utils.GetDownIdx(_currentPosition), dir = Direction.Down };
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		var moveLastBody = true;
		var moveIdxDirection = possibleMoveIdx[Random.Range(0, possibleMoveIdx.Length)];
		while(!CheckMovePossibility(moveIdxDirection.idx))
			moveIdxDirection = possibleMoveIdx[Random.Range(0, possibleMoveIdx.Length)];
		//if (!CheckMovePossibility(moveIdxDirection.idx)) return;
		if (!_addBody)
		{
			_tail.localPosition = _body[_body.Count - 1].localPosition;
		}
		else
		{
			Debug.Log("adding body");
			var b = Instantiate(_bodyPrefab, transform);
			b.localPosition = _body[_body.Count - 1].localPosition;
			_body.Add(b);
			_addBody = false;
			moveLastBody = false;
		}
		var prevBodyPos = _body[0].localPosition;
		_body[0].localPosition = _head.localPosition;
		var lastIdx = moveLastBody ? _body.Count : _body.Count - 1;
		for (var i = 1; i < lastIdx; ++i)
		{
			var pos = _body[i].localPosition;
			_body[i].localPosition = prevBodyPos;
			prevBodyPos = pos;
		}
		_head.localPosition = Utils.GetPositionFromIdx(moveIdxDirection.idx);
		_currentPosition = moveIdxDirection.idx;
		_currentDirection = moveIdxDirection.dir;
	}

	private bool CheckMovePossibility(int idx)
	{
		var res = true;
		if (!Utils.CheckIdxOnGameField(idx))
		{
			Debug.Log("knocking on border");
			res = false;
		}
		if (_body.Any(body => idx == Utils.GetIdxFormPosition(body.localPosition)))
		{
			Debug.Log("trying to move on body");
			res = false;
		}

		return res;
	}
}
