using System;
using System.Collections.Generic;
using UnityEngine;

using Yarn.Unity;

namespace Portland.YarnSpinner
{
	public class SetDialogueVarBehaviour : MonoBehaviour
	{
		public enum YarnVarType
		{
			Bool,
			Float,
			String,
		}

		[SerializeField] DialogueRunner _yarnRunner;

		[Title("Variable")]

		[SerializeField] string _name;
		[SerializeField] string _value;
		[SerializeField] YarnVarType _type;

		public void DoSetYarVariable()
		{
			if (_type == YarnVarType.Bool)
			{
				_yarnRunner.VariableStorage.SetValue(_name, Boolean.Parse(_value));
			}
			else if ( _type == YarnVarType.Float)
			{
				_yarnRunner.VariableStorage.SetValue(_name, Single.Parse(_value));
			}
			else
			{
				_yarnRunner.VariableStorage.SetValue(_name, _value);
			}
		}
	}
}
