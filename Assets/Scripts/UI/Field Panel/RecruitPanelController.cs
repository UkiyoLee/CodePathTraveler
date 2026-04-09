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

    public override void SetupPanel(ActionBase actionBase)
    {
        base.SetupPanel(actionBase);

        RecruitAction recruitAction = (RecruitAction)actionBase;

        npcNameText.text = recruitAction.CurrentCharacter.characterName;
        characterImage.sprite = recruitAction.CurrentCharacter.Portrait;
        //TODO：目前还没有等级相关的变量，所以先用Lv.5代替
        levelText.text = "Lv.5";

        BindButtons();
        SetDefaultSelection();
    }

    public void BindButtons()
    {
        // RebindButtons(confirmButton, OnConfirm);
        RebindButtons(cancelButton, OnCancel);
    }
}
