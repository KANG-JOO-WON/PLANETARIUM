using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Linq;

public class Ball_Handler : Event_Handler_Core
{
	readonly int _maxBallCount = 250;

	public GameObject Ball_Prefab;
	public Transform Parent;

	Color _ballColor;

	List<Ball> _ballList;

	int _shootBallCount;
	int _arrivedBallCount;

	public void Init()
	{
		if(_ballList != null)
		{
			foreach (var ball in _ballList)
				Destroy(ball.gameObject);
		}

		_ballList = new List<Ball>();

		_ballColor = new Color(128f, 128f, 128f, 128f);

		Event_Registration(EventAction);
	}

	void AddBall()
	{
		var ballPower = StaticVariables.s_ballCount;

		if (ballPower > _maxBallCount)
		{
			var power = ballPower / _maxBallCount;
			var ultraPowerBallCount = ballPower % _maxBallCount;
			var firstBall = _ballList.First();
			int ultraPower = power + 1;

			if(ultraPowerBallCount > 0)
			{
				var ball = _ballList.Last(v => v.BallProperty.Power != ultraPower);
				ball.SetProperty(ultraPower);
			}
			else if(ultraPowerBallCount == 0)
			{
				if (firstBall.BallProperty.Power != ultraPower)
					firstBall.SetProperty(ultraPower);
			}
		}
		else
		{
			var ball = Instantiate(Ball_Prefab).GetComponent<Ball>();
			ball.name = _ballList.Count.ToString();
			ball.transform.SetParent(Parent);
			ball.transform.localScale *= StaticVariables.s_brickYRatio;

			if (_ballList.Count <= 0)
			{
				var point = Calculate_Screen.GetScreenBottomLocalPostion();
				var sprite = ball.GetComponent<SpriteRenderer>();
				point.y = Calculate_Screen.GetScreenBottomLocalPostion().y + (sprite.bounds.size.y * 0.5f);
				StaticVariables.s_spotPoint = point;
			}

			ball.transform.localPosition = StaticVariables.s_spotPoint;
			ball.SetProperty(1);

			_ballList.Add(ball);
		}

		StaticVariables.s_ballCount++;
	}

	void EventAction(Event_Type type, object value)
	{
		switch (type)
		{
			case Event_Type.Shoot:
				_shootBallCount = 0;
				_arrivedBallCount = 0;

				if (_ballShoot_Coroutine != null)
					StopCoroutine(_ballShoot_Coroutine);
				_ballShoot_Coroutine = StartCoroutine(BallShoot());

				break;
			case Event_Type.AddBall:
				AddBall();
				break;
			case Event_Type.BallStop:
				if (_ballShoot_Coroutine != null)
					StopCoroutine(_ballShoot_Coroutine);

				foreach (var v in _ballList)
				{
					v.Event_Call(Event_Type.BallStop);
				}
				break;
		}
	}

	void EventAction_Callback(object sender, Event_Callback_Type type, object value)
	{
		switch(type)
		{
			case Event_Callback_Type.Shooting:
				_shootBallCount++;
				break;
			case Event_Callback_Type.Arrived:
				_arrivedBallCount++;

				var ball = (Ball)sender;
				if(ball != null)
				{
					if(_arrivedBallCount == 1)
					{
						var point = ball.transform.localPosition;
						var sprite = ball.GetComponent<SpriteRenderer>();
						point.y = Calculate_Screen.GetScreenBottomLocalPostion().y + (sprite.bounds.size.y * 0.5f);
						StaticVariables.s_spotPoint = point;
					}

					ball.transform.localPosition = new Vector3(ball.transform.localPosition.x, StaticVariables.s_spotPoint.y, ball.transform.localPosition.z);
					ball.transform.DOLocalMove(StaticVariables.s_spotPoint, 0.2f);
				}

				if (_shootBallCount == _arrivedBallCount)
					Event_Callback(this, Event_Callback_Type.AllArrived);

				break;
			case Event_Callback_Type.Success_BallStop:
				_arrivedBallCount++;
				if (_shootBallCount == _arrivedBallCount)
					Event_Callback(this, Event_Callback_Type.AllArrived);
				break;
		}
	}

	Coroutine _ballShoot_Coroutine;
	IEnumerator BallShoot()
	{
		var waitForFixedUpdate = new WaitForFixedUpdate();
		int shootCount = 0;

		foreach (var v in _ballList)
		{
			v.Event_Registration_Callback(EventAction_Callback);
			v.Event_Call(Event_Type.Shoot);
			shootCount += v.BallProperty.Power;
			Event_Callback(this, Event_Callback_Type.RemainBallCount, shootCount);

			yield return waitForFixedUpdate;
		}
	}
}
