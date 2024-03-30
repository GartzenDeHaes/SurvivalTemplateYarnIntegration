using System.Collections;
using System.Collections.Generic;

using PolymindGames;

using UnityEngine;

using Yarn.Unity;

namespace Portland.YarnSpinner
{
	public class YarnSoloSurvivalGameMode : SoloSurvivalGameMode
	{
		[SerializeField] DialogueRunner _yarnspinner;

		protected override void OnPlayerInitialized()
		{
			base.OnPlayerInitialized();

			var sync = player.gameObject.GetComponentInFirstChildren<CharacterYarnSyncBehaviour>();
			sync.SetYarnSpinner(_yarnspinner);
		}
	}
}
