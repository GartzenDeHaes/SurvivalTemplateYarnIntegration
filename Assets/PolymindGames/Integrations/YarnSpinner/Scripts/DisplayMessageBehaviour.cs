using System.Collections;
using System.Collections.Generic;
using PolymindGames.UISystem;

using UnityEngine;

namespace Portland.YarnSpinner
{
	public class DisplayMessageBehaviour : MonoBehaviour
	{
		[SerializeField] Color _color = Color.white;

		public void DisplayNotification(string message)
		{
			MessageDisplayerUI.PushMessage(message, _color);
		}
	}
}
