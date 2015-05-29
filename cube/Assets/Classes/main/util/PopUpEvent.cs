using System;

public class PopUpEvent : GameEventDynamic
{
	// 表示時のメッセージ
	string m_message_text;

	// 閉じる際にオブジェクトを破棄するかどうか
	public enum enmPopUpDestroyType
	{
		ONE_SHOT,
		PERMANET,
		ENTRY_MAX
	}
	enmPopUpDestroyType m_destroy_type;

	// 表示するボタンの種類
	public enum enmPopUpButtonType
	{
		YES_AND_NO,
		CLOSE_ONRY,
		ENTRY_MAX
	}
	enmPopUpButtonType m_button_type;

	public PopUpEvent (string p_message, enmPopUpButtonType p_button_type, enmPopUpDestroyType p_destroy_type)
	{
		m_message_text = p_message;
		m_button_type = p_button_type;
		m_destroy_type = p_destroy_type;
	}

}
