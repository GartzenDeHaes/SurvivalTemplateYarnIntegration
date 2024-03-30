using System;
using System.Collections.Generic;

using PolymindGames;
using PolymindGames.InventorySystem;
using PolymindGames.MovementSystem;
using PolymindGames.UISystem;

using UnityEngine;

using Yarn.Unity;

namespace Portland.YarnSpinner
{
	public class CharacterYarnSyncBehaviour : CharacterBehaviour, ISaveableComponent
	{
		[Title("Stat Variable Names")]
		[SerializeField] string _yarnHealthVarName = "$StatHealth";
		[SerializeField] string _yarnHungerVarName = "$StatHunger";
		[SerializeField] string _yarnStaminaVarName = "$StatStaminia";
		[SerializeField] string _yarnThirstVarName = "$StatThirst";

		DialogueRunner _yarnspinner;

		IHealthManager _healthManager;
		IEnergyManager _energyManager;
		IHungerManager _hungerManager;
		//ITemperatureManager _temperatureManager;
		IThirstManager _thirstManager;
		//IWieldablesController _armsManager;
		IInventory _inventoryManager;

		protected override void OnBehaviourEnabled()
		{
			GetModule(out _healthManager);
			GetModule(out _energyManager);
			GetModule(out _hungerManager);
			//GetModule(out _temperatureManager);
			GetModule(out _thirstManager);
			//GetModule(out _armsManager);
			GetModule(out _inventoryManager);
		}

		/// <summary>
		/// Update all variables before dialogue runs.
		/// </summary>
		void OnDialogueStart()
		{
			_yarnspinner.VariableStorage.SetValue(_yarnHealthVarName, _healthManager.Health);
			_yarnspinner.VariableStorage.SetValue(_yarnHungerVarName, _hungerManager.Hunger);
			_yarnspinner.VariableStorage.SetValue(_yarnStaminaVarName, _energyManager.Energy);
			_yarnspinner.VariableStorage.SetValue(_yarnThirstVarName, _thirstManager.Thirst);

			_yarnspinner.VariableStorage.SetValue(_yarnHealthVarName + "Pct", _healthManager.Health / _healthManager.MaxHealth);
			_yarnspinner.VariableStorage.SetValue(_yarnHungerVarName + "Pct", _hungerManager.Hunger / _hungerManager.MaxHunger);
			_yarnspinner.VariableStorage.SetValue(_yarnStaminaVarName + "Pct", _energyManager.Energy / _energyManager.MaxEnergy);
			_yarnspinner.VariableStorage.SetValue(_yarnThirstVarName + "Pct", _thirstManager.Thirst / _thirstManager.MaxThirst);
		}

		public void SetYarnSpinner(DialogueRunner yarnSpinner)
		{
			_yarnspinner = yarnSpinner;

			_yarnspinner.onDialogueStart.AddListener(OnDialogueStart);
			_yarnspinner.AddFunction<bool>("is_crouching", IsCrouching);
			_yarnspinner.AddFunction<string, int>("get_itemcount", GetTimeCount);
			_yarnspinner.AddFunction<string, bool>("has_item", HasItem);
			_yarnspinner.AddCommandHandler<string, int>("cmd_removeitem", RemoveItemFromInventory);

			// When loading a game, LoadMembers happens before SetYarnSpinner
			if (_pendingLoad != null)
			{
				LoadMembers(_pendingLoad);
				_pendingLoad = null;
			}
		}

		bool IsCrouching()
		{
			return base.Character.GetMovementController().ActiveState == MovementStateType.Crouch;
		}

		bool TryGetItemId(string name, out int itemId)
		{
			var itemDef = ItemDefinition.GetWithName(name);
			if (itemDef == null)
			{
				itemId = 0;
				return false;
			}
			itemId = itemDef.Id;
			return true;
		}

		int GetTimeCount(string itemName)
		{
			if (TryGetItemId(itemName, out int itemId))
			{
				return _inventoryManager.GetItemsWithIdCount(itemId);
			}
			return 0;
		}

		bool HasItem(string itemName)
		{
			if (TryGetItemId(itemName, out int itemId))
			{
				return _inventoryManager.GetItemsWithIdCount(itemId) > 0;
			}
			return false;
		}

		void RemoveItemFromInventory(string itemName, int count = 1)
		{
			int numRemoved = _inventoryManager.RemoveItemsWithName(itemName, count);
			MessageDisplayerUI.PushMessage($"Removed {numRemoved} {itemName}s", Color.white);
		}

		void OnDisable()
		{
			_yarnspinner?.onDialogueStart.RemoveListener(OnDialogueStart);
			_yarnspinner?.RemoveFunction("is_crouching");
			_yarnspinner?.RemoveFunction("get_itemcount");
			_yarnspinner?.RemoveFunction("has_item");
			_yarnspinner?.RemoveCommandHandler("cmd_removeitem");
		}

		public struct SaveVar
		{
			public string Name;
			public string TypeName;
			public string Value;
		}

		#region Save & Load

		object[] _pendingLoad;

		public void LoadMembers(object[] members)
		{
			if (_yarnspinner == null)
			{
				_pendingLoad = members;
				return;
			}
			for (int i = 0; i < members.Length; i++)
			{
				var syncvar = (SaveVar)members[i];
				if (syncvar.TypeName.Equals("Single"))
				{
					_yarnspinner.VariableStorage.SetValue(syncvar.Name, Single.Parse(syncvar.Value));
				}
				else if (syncvar.TypeName.Equals("Boolean"))
				{
					_yarnspinner.VariableStorage.SetValue(syncvar.Name, Boolean.Parse(syncvar.Value));
				}
				else
				{
					if (! syncvar.TypeName.Equals("String"))
					{
						Debug.Log($"Unexpected type of {syncvar.TypeName}");
					}
					_yarnspinner.VariableStorage.SetValue(syncvar.Name, syncvar.Value);
				}
			}
		}

		public object[] SaveMembers()
		{
			List<object> items = new();

			if (_yarnspinner.VariableStorage is InMemoryVariableStorage storage)
			{
				foreach (var syncvar in storage)
				{
					items.Add(new SaveVar { Name = syncvar.Key, TypeName = syncvar.Value.GetType().Name, Value = syncvar.Value.ToString() });
				}
			}

			return items.ToArray();
		}
		#endregion
	}
}