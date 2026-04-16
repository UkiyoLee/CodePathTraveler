public class RecruitAction : ActionBase
{
    public CharacterDefinitionSO CurrentCharacter { get; private set; }

    public void Awake()
    {
        CurrentCharacter = GetComponent<CharacterIdentity>().CharacterDefinition;
    }

    public override void TriggerAction(AllyDefinitionSO interactor)
    {
        EventBus.Publish(new PanelRequestEvent(this));
    }

    public override void Execute(object contextData = null)
    {
        PartyManager.Instance.RecruitMember(CurrentCharacter);
    }

    private void HideActionNPC()
    {
        this.gameObject.SetActive(false);
    }
}
