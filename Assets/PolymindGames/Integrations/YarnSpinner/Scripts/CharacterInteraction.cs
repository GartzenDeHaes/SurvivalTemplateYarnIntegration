using System.Collections;

using PolymindGames;
using PolymindGames.InputSystem;
using PolymindGames.UISystem;
using PolymindGames.WorldManagement;

using Portland.YarnSpinner;

using UnityEngine;

using Yarn.Unity;

namespace Portland.YarnSpinner
{
	/// <summary>
	/// Goes on the character
	/// </summary>
	public class CharacterInteraction : Interactable, ISaveableComponent, IInteractableCharacter
	{
		[SerializeField]
		InputContextGroup m_InputContext;

		[SerializeField] DialogueRunner _yarnRunner;

		//private DataIdReference<ItemDefinition> m_Item = new(0);
		[SerializeField] string _yarnStartNode = "Start";

		bool m_IsYarnRunning;

		public override void OnInteract(ICharacter character)
		{
			base.OnInteract(character);

			if (!InteractionEnabled)
			{
				return;
			}

			if (m_IsYarnRunning)
			{
				Debug.LogWarning($"Yarn already running in {name}");
				return;
			}
			//MessageDisplayerUI.PushMessage($"Interacted with {name}", Color.white);

			//if (character.TryGetModule(out ISleepHandler sleepHandler))
			//{
			//	base.OnInteract(character);

			//	sleepHandler.Sleep(this);
			//}

			CursorLocker.AddCursorUnlocker(this);
			InputManager.PushContext(m_InputContext);

			m_IsYarnRunning = true;
			_yarnRunner.onDialogueComplete.AddListener(OnDialogComplete);

			_yarnRunner.StartDialogue(_yarnStartNode);
		}

		void OnDialogComplete()
		{
			CursorLocker.RemoveCursorUnlocker(this);
			InputManager.PopContext(m_InputContext);

			m_IsYarnRunning = false;
			_yarnRunner.onDialogueComplete.RemoveListener(OnDialogComplete);
		}

		#region Save & Load
		public void LoadMembers(object[] members)
		{
		}

		public object[] SaveMembers()
		{
			object[] members = {
				  //AttachedItem
			  };

			return members;
		}
		#endregion

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();
		}
#endif
	}
}