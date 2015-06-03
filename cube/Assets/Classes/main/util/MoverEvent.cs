using UnityEngine;
using System.Collections;

public class MoverEvent : GameEventDynamic {

	public enum enmMoverBehavior
	{
		MOVE,
		SEARCH,
		ATTACK,
		USERCLICK,
		ENTRY_MAX
	}

	public enum enmUserOperationOrder
	{
		MOVE,
		WAKEUP,
		ENTRY_MAX
	}

	enmUserOperationOrder m_user_operation_order;
	enmMoverBehavior m_behavior;
	Vector3 m_now_position;
	Vector3 m_user_ordered_position;

	public MoverEvent(enmMoverBehavior p_mover_behavior, enmUserOperationOrder p_user_operation_order, Vector3 p_now_position, Vector3 p_user_orderd_position )
	{
		m_behavior = p_mover_behavior;
		m_user_operation_order = p_user_operation_order;
		m_now_position = p_now_position;
		m_user_ordered_position = p_user_orderd_position;
	}
}
