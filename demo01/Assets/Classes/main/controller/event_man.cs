using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;




//Subjectの実体
class ControlManager : MonoBehaviour ,ISubject
{


	private List<IObserver> observerList;
	private NotifyStruct notifySubject;

	public ControlManager()
	{
		observerList = new List<IObserver>();
	}

	//Observerを登録
	public void Attach(IObserver observer)
	{
		observerList.Add(observer);
	}

	//Observerを削除
	public void Detach(IObserver observer)
	{
		observerList.Remove(observer);
	}

	//Observerに送信
	public void Notify()
	{
		foreach (IObserver observer in observerList)
		{
			observer.Notify(notifySubject);
		}
	}

/*
	private Dictionary<string, DelegateObject> delegateList; // = new Dictionary<string, int>();
	public void Attach( string aCommand , DelegateObject aObject , Func<GameObject, EventStruct> aFunc )
	{
		aObject.onEvent += aFunc;
		delegateList.Add( aCommand, aObject );
	}

	public void Dettach( string aCommand )
	{
		delegateList.Remove( aCommand );
	}
*/

	public void SetChanged(int from, int to, string subject)
	{
		NotifyStruct notifySubject;
		notifySubject.from = from;
		
		notifySubject.to = to;
		notifySubject.subject = subject;

		this.SetChanged(notifySubject);
		
	}

	public void SetChanged(NotifyStruct notifySubject) {
		
		this.notifySubject.from = notifySubject.from;
		this.notifySubject.to = notifySubject.to;
		this.notifySubject.subject = notifySubject.subject;

		Notify();
		
	}
	
}
