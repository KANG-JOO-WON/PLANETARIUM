
using System;

public enum Event_Type
{
	None = 0,
	Shoot,
	AddBall,
	DrawGuideLine,
	DrawGuideLine_Reset,
	CreateBrick,
	MovingBrick,
	MovingAddBall,
	AttackBrick,
	HitAddBall,
	GetAddBall,
	SetScore,
	SetCoin,
	Enable_BallStopBtn,
	Disable_BallStopBtn,
	BallStop,
	SetBallCount,
	RemainBallCount,
	InGameUI_MoreBtnClose,
	StartGame_SingleMode,
	StartGame_RankingMode,

	GameOver,
	GameOver_Effect,
	ShowGameOver_WebView,
	Reset,
	InGameUI_RemainingTime,
	Active_DarkMode,
	Active_WhiteMode,
	Revive_Effect,
	Revive,
	Check_Combo,
	Special_Brick,
	DimScreen,
	DimScreen_Processing,

	// popup
	Popup_Setting,
	Popup_BackToTheLobby,
	Popup_Revive,
	Popup_TreasureBox,
	Popup_NotEnough,
}

public enum Event_Callback_Type
{
	None =0,
	Shooting,
	Arrived,
	AllArrived,
	HitAddBall,
	GetAddBallCompleted,
	SetScore,
	SetCoin,
	BallStopBtnClick,
	Success_BallStop,
	StartGame_SingleMode,
	StartGame_RankingMode,

	GameOver,
	GameOver_Effect,
	Show_WebView,
	Active_DarkMode,
	Active_WhiteMode,
	Revive_Effect,
	Revive,
	DimScreen,
	DimScreen_Processing,
	RemainBallCount,

	// popup
	Popup_Setting,
	Popup_BackToTheLobby,
	Popup_Revive,
	Popup_TreasureBox,
	Popup_NotEnough,
}

public delegate void Event_Action(Event_Type type, object value = null);
public delegate void Event_Callback(object sender, Event_Callback_Type type, object value = null);

interface IEvent_Item
{
	event Event_Action e_eventAction;
	event Event_Callback e_eventCallback;
}
