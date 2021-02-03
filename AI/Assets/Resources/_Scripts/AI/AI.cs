using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AI : MonoBehaviour
{
	[Header("Wander")]
	public Vector2 TimeWander;
	public float DistanceWander;
	public float WanderSpeed;

	[Header("Aggro")]
	public float AggroRange;
	public float AggroSpeed;
	GameObject _player;
	bool _waitingAggro;
	IEnumerator _aggroWait;

	Vector3 _spawnPoint;
	Rect _wanderArea;
	Rect _aggroArea;

	protected Vector2 _destination;
	protected float _moveSpeed;

	public enum States
	{
		wandering,
		aggro,
	}

	States _currentState;


	public virtual void Start()
	{
		_spawnPoint = transform.position;
		_wanderArea = new Rect(new Vector2(_spawnPoint.x - DistanceWander / 2, _spawnPoint.y - DistanceWander / 2), Vector2.one * DistanceWander);
		_player = GameObject.FindGameObjectWithTag("Player");
		_aggroWait = WaitingAggro();

		StartCoroutine(Wander());
	}

	public virtual void Update()
	{
		Move();
		CheckAggro();
	}

	public virtual IEnumerator Wander()
	{
		if (_currentState != States.wandering)
			SwitchState(States.wandering);

		yield return new WaitForSeconds(Random.Range(TimeWander.x, TimeWander.y));

		_destination = new Vector2(Random.Range(_wanderArea.xMin, _wanderArea.xMax), Random.Range(_wanderArea.yMin, _wanderArea.yMax));
		_moveSpeed = WanderSpeed;

		StartCoroutine(Wander());
	}


	public virtual void Move()
	{
		if (_destination.x < transform.position.x)
			transform.localScale = new Vector3(-1, 1, 1);
		else
			transform.localScale = new Vector3(1, 1, 1);


		transform.position = Vector3.MoveTowards(transform.position, _destination, Time.deltaTime *_moveSpeed);
	}


	public virtual void CheckAggro()
	{
		_aggroArea = new Rect(new Vector2(transform.position.x - AggroRange / 2, transform.position.y - AggroRange / 2), Vector2.one * AggroRange);
		if (_aggroArea.Contains(_player.transform.position))
		{
			if(_currentState != States.aggro)
			{
				SwitchState(States.aggro);
			}

			_destination = _player.transform.position;
			_moveSpeed = AggroSpeed;

			_waitingAggro = false;
			StopCoroutine(_aggroWait);
			_aggroWait = WaitingAggro();
		}
		if(_currentState == States.aggro && !_waitingAggro)
		{
			_waitingAggro = true;
			StartCoroutine(_aggroWait);
		}
	}

	IEnumerator WaitingAggro()
	{
		yield return new WaitForSeconds(3);

		SwitchState(States.wandering);
	}

	void SwitchState(States state)
	{
		StopAllCoroutines();

		_currentState = state;
		if (_currentState == States.wandering)
			StartCoroutine(Wander());
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(_spawnPoint, Vector2.one * DistanceWander);

		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, Vector2.one * AggroRange);
	}
}
