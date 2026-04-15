using System;
using TMPro;
using UnityEngine.UI;

public class RecruitPanelController : PanelController
{
    [Header("Recruit Panel")]
    [SerializeField] private TMP_Text npcNameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Image characterImage;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    public override Type PanelActionType => typeof(RecruitAction);

    public override void SetupPanel(ActionBase actionBase)
    {
        base.SetupPanel(actionBase);

        RecruitAction recruitAction = (RecruitAction)actionBase;

        npcNameText.text = recruitAction.CurrentCharacter.characterName;
        characterImage.sprite = recruitAction.CurrentCharacter.Portrait;
        levelText.text = "Lv." + recruitAction.CurrentCharacter.BaseLevel;

        BindButtons();
        SetDefaultSelection();
    }

    public void BindButtons()
    {
        // RebindButtons(confirmButton, OnConfirm);
        RebindButtons(cancelButton, OnCancel);
    }
}
