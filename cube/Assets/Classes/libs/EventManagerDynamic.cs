﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// イベントクラス（インスタンス生成）
/// 1).イベント受け取り側クラスでこのマネージャを生成し
/// 　　GameEventBase派生のイベントクラスで受けるリスナーを登録する。
/// 2).イベント発行側クラスにこのクラスを渡し
/// 　　発行側クラス内でイベントを発行する。
/// 　　その際、GameEventBase派生のイベントクラスを引数に渡す。
/// </summary>

public class GameEventDynamic
{
	private int m_hash;
	public int _hash {
		get { return m_hash;	}
		set { m_hash = value;	}
	}

	public GameEventDynamic()
	{
		float t = Time.realtimeSinceStartup;
		m_hash = t.ToString().GetHashCode();
	}
}

public class EventManagerDynamic
{

	private GameEventDynamic m_Instance = null;

	public delegate void EventDelegate<T> (T e) where T : GameEventDynamic;
	private delegate void EventDelegate (GameEventDynamic e);
	
	private Dictionary<System.Type, EventDelegate> delegates = new Dictionary<System.Type, EventDelegate>();
	private Dictionary<System.Delegate, EventDelegate> delegateLookup = new Dictionary<System.Delegate, EventDelegate>();
	private Dictionary<System.Delegate, bool> onceLookups = new Dictionary<System.Delegate, bool>();
	

	private EventDelegate f_AddDelegate<T>(EventDelegate<T> p_del) where T : GameEventDynamic {
		// Early-out if we've already registered this delegate
		if (delegateLookup.ContainsKey(p_del))
			return null;
		
		// Create a new non-generic delegate which calls our generic one.
		// This is the delegate we actually invoke.
		EventDelegate internalDelegate = (e) => p_del((T)e);
		delegateLookup[p_del] = internalDelegate;
		
		EventDelegate tempDel;
		if (delegates.TryGetValue(typeof(T), out tempDel)) {
			delegates[typeof(T)] = tempDel += internalDelegate; 
		} else {
			delegates[typeof(T)] = internalDelegate;
		}
		
		return internalDelegate;
	}

	// リスナー登録
	public void f_AddListener<T> (EventDelegate<T> p_del) where T : GameEventDynamic {
		f_AddDelegate<T>(p_del);
	}

	// リスナー登録（ワンショット：１度イベントが発行されるとリスナーが削除される）
	public void f_AddListenerOnce<T> (EventDelegate<T> p_del) where T : GameEventDynamic {
		EventDelegate result = f_AddDelegate<T>(p_del);
		
		if(result != null){
			// remember this is only called once
			onceLookups[result] = true;
		}
	}

	// 指定されたイベントクラスに該当するリスナーを削除する
	public void f_RemoveListener<T> (EventDelegate<T> p_del) where T : GameEventDynamic {
		EventDelegate internalDelegate;
		if (delegateLookup.TryGetValue(p_del, out internalDelegate)) {
			EventDelegate tempDel;
			if (delegates.TryGetValue(typeof(T), out tempDel)){
				tempDel -= internalDelegate;
				if (tempDel == null){
					delegates.Remove(typeof(T));
				} else {
					delegates[typeof(T)] = tempDel;
				}
			}
			
			delegateLookup.Remove(p_del);
		}
	}

	// 全てのリスナーを削除する
	public void f_RemoveAll(){
		delegates.Clear();
		delegateLookup.Clear();
		onceLookups.Clear();
	}

	// 現在登録されているリスナーを取得する
	public bool f_HasListener<T> (EventDelegate<T> del) where T : GameEventDynamic {
		return delegateLookup.ContainsKey(del);
	}

	// イベント発行
	public void f_Dispatch(GameEventDynamic p_e) {
		EventDelegate del;
		if (delegates.TryGetValue(p_e.GetType(), out del)) {
			del.Invoke(p_e);
			
			// remove listeners which should only be called once
			foreach(EventDelegate k in delegates[p_e.GetType()].GetInvocationList()){
				if(onceLookups.ContainsKey(k)){
					onceLookups.Remove(k);
				}
			}
		} else {
			Debug.LogWarning("Event: " + p_e.GetType() + " has no listeners");
		}
	}

	// ウォッチドッグタイマーを作成しようと思ったが独立してタイマーが立てられないみたいなので
	// キューはやめ。

}
