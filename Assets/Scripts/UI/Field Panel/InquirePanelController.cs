using UnityEngine.UI;
using TMPro;
using System;

public class InquirePanelController : PanelController
{
    [Header("Inquire Panel")]
    [SerializeField] private TMP_Text npcNameText;
    [SerializeField] private Image npcAvatarImage;
    [SerializeField] private TMP_Text npcTitleText;
    [SerializeField] private TMP_Text npcContentText;

    private InquireAction _currentAction;
    private int _currentIndex = -1;

    public override Type PanelActionType => typeof(InquireAction);

    [Header("Buttons")]
    [SerializeField] private Button confirmButton;

    public override void SetupPanel(ActionBase action)
    {
        base.SetupPanel(action);

        // Set up the buttons
        FirstButton = confirmButton;
        BindButtons();
        SetDefaultSelection();

        _currentAction = (InquireAction)CurrentAction;

        ApplyMessage(_currentAction.PickRandomMessageIndex());
    }

    private void ApplyMessage(int messageIndex)
    {
        _currentAction.GetInquireActionData(messageIndex, out InquireActionData inquireActionData);
        _currentIndex = messageIndex;

        npcNameText.text = inquireActionData.personName;
        npcAvatarImage.sprite = inquireActionData.portraitOverride;
        npcTitleText.text = inquireActionData.title;
        npcContentText.text = inquireActionData.message;
    }

    private void BindButtons()
    {
        RebindButtons(confirmButton, OnCancel);
    }
}
