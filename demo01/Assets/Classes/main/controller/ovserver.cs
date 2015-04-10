using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

interface IObserver
{
	//Subjectへの通知メソッド
	void Notify(NotifyStruct notifySubject);
}

//Subject（通知する側）のインターフェース

interface ISubject
{
	
	// Observerを登録
	void Attach(IObserver observer);

	// Observerを削除
	void Detach(IObserver observer);

	// Observerに通知
	void Notify();
	
}

//イベント通知に利用する構造体
public struct NotifyStruct
{
	public int from;
	public int to;
	public string subject;
	
}

class ScoreObserver : IObserver {

	public void Notify(NotifyStruct notifySubject)
	{
/*
		Console.WriteLine("*** Student ***");
		Console.WriteLine("送信元 = " + notifySubject.from.ToString());
		Console.WriteLine("受信先 = " + notifySubject.to.ToString());
		Console.WriteLine("内容 = " + notifySubject.subject);
*/		
	}
}


class PanelObserver : IObserver {
	
	//このオブジェクトのID
	private const int ID = 20;

	public void Notify(NotifyStruct notifySubject)
	{
/*
		Console.WriteLine("*** Teacher ***");
		Console.WriteLine("送信元 = " + notifySubject.from.ToString());
		Console.WriteLine("受信先 = " + notifySubject.to.ToString());
		Console.WriteLine("内容 = " + notifySubject.subject);
*/
	}
	
}




/*


// フレームワーク部
public static class ObjectExtensions
{
	// オブジェクトの指定された名前のプロパティの値を取得
	public static object Eval(this object item, string propertyName)
	{
		var propertyInfo = item.GetType().GetProperty(propertyName);
		return propertyInfo == null ? null : propertyInfo.GetValue(item, null);
	}

	// Expression からメンバー名を取得 (「Expression を使ってラムダ式のメンバー名を取得する」で作成)
	public static string GetMemberName<ObjectType, MemberType>(this ObjectType @this, Expression<Func<ObjectType, MemberType>> expression)
	{
		return ((MemberExpression)expression.Body).Member.Name;
	}
	
	// Expression からメンバー名を取得 (「Expression を使ってラムダ式のメンバー名を取得する」で作成)
	public static string GetMemberName<MemberType>(Expression<Func<MemberType>> expression)
	{
		return ((MemberExpression)expression.Body).Member.Name;
	}
}

abstract class Observable // 更新を監視される側
{
	public event List<string> Update;
	
	protected void RaiseUpdate<PropertyType>(Expression<Func<PropertyType>> propertyExpression)
	{
		RaiseUpdate(ObjectExtensions.GetMemberName(propertyExpression));
	}
	
	void RaiseUpdate(string propertyName)
	{
		if (Update != null)
			Update(propertyName);
	}
}

abstract class Observer // 更新を監視する側
{
	Dictionary<string, List<object>> updateExpressions = new Dictionary<string, List<object>>();
	Observable dataSource = null;
	
	public Observable DataSource
	{
		set {
			dataSource = value;
			value.Update += Update;
		}
	}
	
	protected void AddAction<PropertyType>(Expression<Func<ObservableType, PropertyType>> propertyExpression, List<object> updateAction)
	{
		AddAction(dataSource.GetMemberName(propertyExpression), updateAction);
	}
	
	void AddAction(string propertyName, List<object> aAction)
	{
		updateExpressions[propertyName] = aAction;
	}
	
	void Update(string propertyName)
	{
		List<object> updateAction;
		if (updateExpressions.TryGetValue(propertyName, out updateAction))
			updateAction(dataSource.Eval(propertyName));
	}
}

*/