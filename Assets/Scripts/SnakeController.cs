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

	private float _time;

	private Transform _head;
	private List<Transform> _bodies;
	private Transform _tail;

	private bool _addBody = false;

	private enum Direction
	{
		Left, Right, Up, Down
	}
	private struct Status
	{
		public int idx;
		public Direction dir;

		public override string ToString()
		{
			return "idx = " + idx + ", direction = " + dir;
		}
	}

	private Status _currentStatus;

	private void Awake()
	{
		_bodies = new List<Transform>();

		_head = Instantiate(_headPrefab, transform);
		_bodies.Add(Instantiate(_bodyPrefab, transform));
		_tail = Instantiate(_tailPrefab, transform);

		var bodyIdx = Utils.GetDownIdx(_startIdx);
		_head.localPosition = Utils.GetPositionFromIdx(_startIdx);
		_bodies[0].localPosition = Utils.GetPositionFromIdx(bodyIdx);
		_tail.localPosition = Utils.GetPositionFromIdx(Utils.GetDownIdx(bodyIdx));
		_currentStatus = new Status
		{
			idx = _startIdx,
			dir = Direction.Up
		};
	}

	private void Update()
	{
		_time += Time.deltaTime;
		if (_time >= _tickTime)
		{
			var moveIdxDirection = GetNextMove();
			Move(moveIdxDirection);
			_currentStatus = moveIdxDirection;
			_time = 0;

			if (Random.Range(0, 100) < 30)
				_addBody = true;
		}
	}

	private void Move(Status moveStatus)
	{
		var moveLastBody = true;
		if (!_addBody)
		{
			_tail.localPosition = _bodies[_bodies.Count - 1].localPosition;
		}
		else
		{
			Debug.Log("adding body");
			var b = Instantiate(_bodyPrefab, transform);
			b.localPosition = _bodies[_bodies.Count - 1].localPosition;
			_bodies.Add(b);
			_addBody = false;
			moveLastBody = false;
		}
		var prevBodyPos = _bodies[0].localPosition;
		_bodies[0].localPosition = _head.localPosition;
		var lastIdx = moveLastBody ? _bodies.Count : _bodies.Count - 1;
		for (var i = 1; i < lastIdx; ++i)
		{
			var pos = _bodies[i].localPosition;
			_bodies[i].localPosition = prevBodyPos;
			prevBodyPos = pos;
		}
		_head.localPosition = Utils.GetPositionFromIdx(moveStatus.idx);
	}

	private Status GetNextMove()
	{
		var possibleMoveIdx = GetPossibleDirections();
		Status moveStatus;
		var checkPossibleMoveIdx = possibleMoveIdx;
		do
		{
			if (checkPossibleMoveIdx.Count == 0)
			{
				moveStatus = FindBodyFromMoveDirection(GetPossibleDirections());
				var bodyIdx = CheckBodyOnIdx(moveStatus.idx);
				if (bodyIdx != -1)
					CutSnake(bodyIdx);
				break;
			}
			var moveRandom = Random.Range(0, checkPossibleMoveIdx.Count);
			moveStatus = checkPossibleMoveIdx[moveRandom];
			checkPossibleMoveIdx.RemoveAt(moveRandom);
		} while (!CheckMovePossibility(moveStatus.idx));

		return moveStatus;
	}

	private List<Status> GetPossibleDirections()
	{
		var possibleMoveIdx = new List<Status>();
		var currentPosition = _currentStatus.idx;
		switch (_currentStatus.dir)
		{
			case Direction.Left:
				possibleMoveIdx.Add(new Status { idx = Utils.GetLeftIdx(currentPosition), dir = Direction.Left });
				possibleMoveIdx.Add(new Status { idx = Utils.GetUpIdx(currentPosition), dir = Direction.Up });
				possibleMoveIdx.Add(new Status { idx = Utils.GetDownIdx(currentPosition), dir = Direction.Down });
				break;
			case Direction.Right:
				possibleMoveIdx.Add(new Status { idx = Utils.GetRightIdx(currentPosition), dir = Direction.Right });
				possibleMoveIdx.Add(new Status { idx = Utils.GetUpIdx(currentPosition), dir = Direction.Up });
				possibleMoveIdx.Add(new Status { idx = Utils.GetDownIdx(currentPosition), dir = Direction.Down });
				break;
			case Direction.Up:
				possibleMoveIdx.Add(new Status { idx = Utils.GetRightIdx(currentPosition), dir = Direction.Right });
				possibleMoveIdx.Add(new Status { idx = Utils.GetUpIdx(currentPosition), dir = Direction.Up });
				possibleMoveIdx.Add(new Status { idx = Utils.GetLeftIdx(currentPosition), dir = Direction.Left });
				break;
			case Direction.Down:
				possibleMoveIdx.Add(new Status { idx = Utils.GetRightIdx(currentPosition), dir = Direction.Right });
				possibleMoveIdx.Add(new Status { idx = Utils.GetLeftIdx(currentPosition), dir = Direction.Left });
				possibleMoveIdx.Add(new Status { idx = Utils.GetDownIdx(currentPosition), dir = Direction.Down });
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
		return possibleMoveIdx;
	}

	private Status FindBodyFromMoveDirection(List<Status> possibleMoveDirections)
	{
		foreach (var possibleMoveDirection in possibleMoveDirections)
		{
			var g = CheckBodyOnIdx(possibleMoveDirection.idx);
			if (g != -1)
				return possibleMoveDirection;
		}
		return new Status();
	}

	private void CutSnake(int idx)
	{
		Debug.Log("Cutting snake on body idx = " + idx);
		for (var i = idx; i < _bodies.Count; ++i)
			Destroy(_bodies[i].gameObject);

		_bodies.RemoveRange(idx, _bodies.Count - idx);
	}

	private bool CheckMovePossibility(int idx)
	{
		var res = true;
		if (!Utils.CheckIdxOnGameField(idx))
		{
			Debug.Log("knocking on border");
			res = false;
		}
		if (_bodies.Any(body => idx == Utils.GetIdxFormPosition(body.localPosition)))
		{
			Debug.Log("trying to move on body");
			res = false;
		}

		return res;
	}

	private int CheckBodyOnIdx(int idx)
	{
		for (var i = 0; i < _bodies.Count; ++i)
			if (idx == Utils.GetIdxFormPosition(_bodies[i].localPosition))
				return i;
		return -1;
	}
}
