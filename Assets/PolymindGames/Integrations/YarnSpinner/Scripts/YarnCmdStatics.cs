using System.Collections;
using System.Collections.Generic;
using PolymindGames.UISystem;

using UnityEngine;

using Yarn;
using Yarn.Unity;

namespace Portland.YarnSpinner
{
	public static class YarnCmdStatics
	{
		[YarnCommand("cmd_notification")]
		public static void DisplayNotification(string message, string color = "white")
		{
			TryParseColor(color, out var notificationColor);
			MessageDisplayerUI.PushMessage(message, notificationColor);
		}

		static bool TryParseColor(string color, out Color value)
		{
			switch(color)
			{
				case "black":
					value = Color.black;
					break;
				case "blue":
					value = Color.blue;
					break;
				case "clear":
					value = Color.clear;
					break;
				case "cyan":
					value = Color.cyan;
					break;
				case "gray":
				case "grey":
					value = Color.gray;
					break;
				case "green":
					value = Color.green;
					break;
				case "magenta":
					value = Color.magenta;
					break;
				case "red":
					value = Color.red;
					break;
				case "white":
					value = Color.white;
					break;
				case "yellow":
					value = Color.yellow;
					break;
				default:
					value = Color.white;
					return false;
			}
			return true;
		}
	}
}
