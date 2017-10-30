using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private Sprite[] _avatars;

	public int Position { get; set; }

	public void Setup()
	{
		GetComponent<Image>().sprite = _avatars[Random.Range(0, _avatars.Length)];
	}

}
