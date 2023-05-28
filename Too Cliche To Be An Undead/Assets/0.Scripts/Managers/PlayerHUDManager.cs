using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDManager : MonoBehaviour
{
	private static PlayerHUDManager instance;
	public static PlayerHUDManager Instance
	{
		get
		{
			if (instance == null) Debug.LogError("PlayerHUDManager instance could not be found.");

			return instance;
		}
	}

	[SerializeField] private GridLayoutGroup hudGroup;

	[SerializeField] private PlayerHUD playerHUD_PF;

	private void Awake()
	{
		instance = this;
	}

	public PlayerHUD CreateNewHUD(PlayerCharacter _owner, SpriteRenderer _minimapRenderer, GameManager.E_CharactersNames character, SCRPT_Dash playerDash, SCRPT_Skill playerSkill)
	{
		PlayerHUD newHUD = playerHUD_PF.gameObject.Create(hudGroup.GetComponent<RectTransform>()).GetComponent<PlayerHUD>();
		newHUD.Setup(_owner, _minimapRenderer, character, playerDash, playerSkill);

		return newHUD;
	}
}