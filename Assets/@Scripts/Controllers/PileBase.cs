using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PileBase : MonoBehaviour
{
	[SerializeField]
	private int _row = 2;

	[SerializeField]
	private int _column = 2;

	[SerializeField]
	private Vector3 _size = new Vector3(0.5f, 0.1f, 0.5f);

	[SerializeField]
	private float _dropInterval = 0.05f;

	protected Stack<GameObject> _objects = new Stack<GameObject>();

	public int ObjectCount => _objects.Count;

	// TEST
	public GameObject prefab;

	void Start()
    {
		StartCoroutine(CoSpawnBurger());
	}

	// TEST
	IEnumerator CoSpawnBurger()
	{
		for (int i = 0; i < 20; i++)
		{
			yield return new WaitForSeconds(0.2f);

			GameObject go = GameObject.Instantiate(prefab);
			AddToPile(go);
		}

		for (int i = 0; i < 20; i++)
		{
			yield return new WaitForSeconds(0.1f);

			GameObject go = RemoveFromPile();
			if (go != null)
				GameObject.Destroy(go);
		}
	}

	public void AddToPile(GameObject go)
	{
		_objects.Push(go);
		go.transform.position = GetPositionAt(_objects.Count - 1);
	}

	public GameObject RemoveFromPile()
	{
		if (_objects.Count == 0)
			return null;

		return _objects.Pop();
	}

	private Vector3 GetPositionAt(int pileIndex)
	{
		Vector3 offset = new Vector3((_row - 1) * _size.x / 2, 0, (_column - 1) * _size.z / 2);
		Vector3 startPos = transform.position - offset;

		int row = (pileIndex / _row) % _column;
		int column = pileIndex % _row;
		int height = pileIndex / (_row * _column);

		float x = startPos.x + column * _size.x;
		float y = startPos.y + height * _size.y;
		float z = startPos.z + row * _size.z;

		return new Vector3(x, y, z);
	}

	#region Editor
#if UNITY_EDITOR
	void OnDrawGizmosSelected()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = Color.yellow;

		Vector3 offset = new Vector3((_row - 1) * _size.x / 2, 0, (_column - 1) * _size.z / 2);
		// Vector3 startPos = transform.position - offset;

		Vector3 startPos = -offset; // 로컬 공간 기준 0번 칸의 위치.

		for (int r = 0; r < _row; r++)
		{
			for (int c = 0; c < _column; c++)
			{
				Vector3 center = startPos + new Vector3(r * _size.x, _size.y / 2, c * _size.z);
				Gizmos.DrawWireCube(center, _size);
			}
		}

		Gizmos.matrix = Matrix4x4.identity;
	}
#endif
	#endregion
}
