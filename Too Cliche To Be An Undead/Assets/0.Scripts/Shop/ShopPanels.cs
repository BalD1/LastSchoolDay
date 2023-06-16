using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanels : MonoBehaviour
{
    [field: SerializeField] public TextMeshProUGUI SkillName { get; private set; }
    [field: SerializeField] public TextMeshProUGUI SkillDescription { get; private set; }
    [field: SerializeField] public Image SkillIcon { get; private set; }

    public SCRPT_LevelData currentSelectedLevelData;
    public ShopLevel currentSelectedLevel;

    public void OnClickRelay(int idx) => currentSelectedLevel?.OnUnlockButtonClick();
}
