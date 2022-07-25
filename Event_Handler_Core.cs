using UnityEngine;

public class Event_Handler_Core : MonoBehaviour, IEvent_Item
{
	public event Event_Action e_eventAction;
	public event Event_Callback e_eventCallback;

	public void Event_Registration(Event_Action e)
	{
		e_eventAction -= e;
		e_eventAction += e;
	}

	public void Event_Registration_Callback(Event_Callback e)
	{
		e_eventCallback -= e;
		e_eventCallback += e;
	}

	public void Event_Call(Event_Type type, object value = null)
	{
		if (e_eventAction != null)
			e_eventAction(type, value);
	}

	public void Event_Callback(object sender, Event_Callback_Type callbackType, object value = null)
	{
		if (e_eventCallback != null)
			e_eventCallback(sender, callbackType, value);
	}
}
