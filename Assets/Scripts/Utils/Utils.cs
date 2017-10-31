using UnityEngine;

public static class Utils
{
	public static Vector2 ZeroPos = new Vector2(-480, 352);
    public const int Offset = 64;

	public static Vector2 GetPositionFromIdx(int idx)
	{
		var x = idx % 12;
		var y = idx / 12;
		return ZeroPos + Vector2.down * Offset * y + Vector2.right * Offset * x;
	}

	public static int GetIdxFormPosition(Vector2 pos)
	{
		var x = (pos.x - ZeroPos.x) / Offset;
		var y = -(pos.y - ZeroPos.y) / Offset;
		//Debug.Log("x = " + x + ", y = " + y + ", x + y*12 = " + (x+y*12));
		return (int)(x + y*12);
	}

	//todo: checks need to be here
	public static int GetDownIdx(int idx)
	{
		var res = idx + 12;
		if (!CheckIdx(res))
			res = -1;
		return res;
	}

	//todo: checks need to be here
	public static int GetUpIdx(int idx)
	{
		var res = idx - 12;
		if (!CheckIdx(res))
			res = -1;
		return res;
	}

	//todo: checks need to be here
	public static int GetLeftIdx(int idx)
	{
		var res = idx - 1;
		if (!CheckIdx(res))
			res = -1;
		return res;
	}

	//todo: checks need to be here
	public static int GetRightIdx(int idx)
	{
		var res = idx + 1;
		if (!CheckIdx(res))
			res = -1;
		return res;
	}

	public static bool CheckIdx(int idx)
	{
		return idx > 0 && idx < 12 * 12;
	}

	public static bool CheckIdxOnGameField(int idx)
	{
		var onGameField = false;
		for (var i = 0; i < 10; ++i)
		{
			for (var j = 0; j < 10; ++j)
			{
				if (idx != 13 + i + j * 12) continue;
				onGameField = true;
				break;;
			}
			if(onGameField)
				break;
		}

		return onGameField;
	}

}
