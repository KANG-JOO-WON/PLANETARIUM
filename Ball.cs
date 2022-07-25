using System;
using UnityEngine;
using DG.Tweening;

public class Ball : Event_Handler_Core
{
	public class Ball_Property
	{
		public float Speed;
		public int Power;
	}

	public Ball_Property BallProperty { get; set; }

	Rigidbody2D _rb;
	RaycastHit2D _hit;
	TrailRenderer _tr;
	float _angleLimit = 10f;

	private void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
		_rb.Sleep();
		_rb.isKinematic = true;

		_tr = GetComponent<TrailRenderer>();

		BallProperty = new Ball_Property();
		BallProperty.Speed = 30f * StaticVariables.s_brickYRatio;
		BallProperty.Power = 1;

		Event_Registration(EventAction);
	}

	public void SetProperty(int power)
	{
		BallProperty.Power = power;

		_tr.startWidth = transform.localScale.x;
		_tr.endWidth = _tr.startWidth * 0.1f;
	}

	void EventAction(Event_Type type, object value)
	{
		switch (type)
		{
			case Event_Type.Shoot:
				_rb.isKinematic = false;
				Shoot();
				break;
			case Event_Type.BallStop:
				_rb.Sleep();
				_rb.isKinematic = true;
				transform.DOLocalMove(StaticVariables.s_spotPoint, 0.2f).OnComplete(()=>
				{
					//_rb.isKinematic = false;
					Event_Callback(this, Event_Callback_Type.Success_BallStop);
				});
				break;
		}
	}

	private void Shoot()
	{
		var pos = StaticVariables.s_mousePoint;
		var dir = (pos - transform.position).normalized;
		dir = new Vector2(dir.y >= 0 ? dir.x : dir.x >= 0f ? 1f : -1f, Mathf.Clamp(dir.y, StaticVariables.s_angleLimit, 1f));
		_rb.velocity = dir * BallProperty.Speed;
		//_rb.AddForce(dir * BallProperty.Speed);

		Event_Callback(this, Event_Callback_Type.Shooting);
	}

	private void Update()
	{
		if (_rb == null)
			return;

		if (_rb.isKinematic)
			return;

		float angle = ((Mathf.Atan2(_rb.velocity.y, _rb.velocity.x) * Mathf.Rad2Deg) + 360f) % 360f;
		if (0f <= angle && angle < _angleLimit)
			angle = _angleLimit;
		else if (180f - _angleLimit < angle && angle <= 180f)
			angle = 180f - _angleLimit;
		else if (180f < angle && angle < 180f + _angleLimit)
			angle = 180f + _angleLimit;
		else if (360f - _angleLimit <= angle && angle < 360f)
			angle = 360f - _angleLimit;

		float forceForward = Mathf.Cos(angle * Mathf.Deg2Rad);
		float forceUp = Mathf.Sin(angle * Mathf.Deg2Rad);
		Vector2 newDir = new Vector2(forceForward, forceUp);
		Vector3 nextVelocity = newDir * BallProperty.Speed;
		_rb.velocity = nextVelocity;
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		switch(col.transform.tag)
		{
			case "DEAD_ZONE":
				_rb.Sleep();
				if(!_rb.isKinematic)
					Event_Callback(this, Event_Callback_Type.Arrived);

				_rb.isKinematic = true;
				break;
			case "BRICK":
				var brick = col.transform.parent.GetComponent<Brick>();
				if (brick != null)
					brick.Event_Call(Event_Type.AttackBrick, BallProperty.Power);
					
				break;
		}
	}
}
