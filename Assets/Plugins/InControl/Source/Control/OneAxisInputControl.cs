using System;
using UnityEngine;


namespace InControl
{
	public class OneAxisInputControl : InputControlBase
	{
		internal void CommitWithSides( InputControl negativeSide, InputControl positiveSide, ulong updateTick, float deltaTime )
		{
			LowerDeadZone = Mathf.Max( negativeSide.LowerDeadZone, positiveSide.LowerDeadZone );
			UpperDeadZone = Mathf.Min( negativeSide.UpperDeadZone, positiveSide.UpperDeadZone );
			Raw = negativeSide.Raw || positiveSide.Raw;
			var value = ValueFromSides( negativeSide.RawValue, positiveSide.RawValue );
			CommitWithValue( value, updateTick, deltaTime );
		}


		internal void CommitWithSides( InputControl negativeSide, InputControl positiveSide, ulong updateTick, float deltaTime, bool invertSides )
		{
			if (invertSides)
			{
				CommitWithSides( positiveSide, negativeSide, updateTick, deltaTime );
			}
			else
			{
				CommitWithSides( negativeSide, positiveSide, updateTick, deltaTime );
			}
		}


		float ValueFromSides( float negativeSideValue, float positiveSideValue )
		{
			var nsv = Utility.Abs( negativeSideValue );
			var psv = Utility.Abs( positiveSideValue );

			if (Utility.Approximately( nsv, psv ))
			{
				return 0.0f;
			}

			return nsv > psv ? -nsv : psv;
		}
	}
}

