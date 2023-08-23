using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDManager : Singleton<PlayerHUDManager>
{
	[SerializeField] private GridLayoutGroup hudGroup;

	[SerializeField] private PlayerHUD playerHUD_PF;

    protected override void EventsSubscriber()
    {
        PlayerCharacterEvents.OnPlayerSetup += SetupPlayerHUD;
    }

    protected override void EventsUnSubscriber()
    {
        PlayerCharacterEvents.OnPlayerSetup -= SetupPlayerHUD;
    }

    private void SetupPlayerHUD(PlayerCharacter player)
    {
        PlayerHUD hud = CreateNewHUD(player, player.MinimapMarkerSprite, player.GetCharacterName(), player.PlayerDash, player.GetSkill);
        player.SetHUD(hud);
        hud.ForceHPUpdate();
    }

    public PlayerHUD CreateNewHUD(PlayerCharacter _owner, SpriteRenderer _minimapRenderer, GameManager.E_CharactersNames character, SCRPT_Dash playerDash, SCRPT_Skill playerSkill)
	{
		PlayerHUD newHUD = playerHUD_PF.gameObject.Create(hudGroup.GetComponent<RectTransform>()).GetComponent<PlayerHUD>();
		newHUD.Setup(_owner, _minimapRenderer, character, playerDash, playerSkill);

		return newHUD;
	}
}