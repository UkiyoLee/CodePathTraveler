public class PlayerInteractor : MonoBehaviour
{
    private CharacterIdentity _characterIdentity;
    private InteractionBase _target;

    private void Awake()
    {
        _characterIdentity = GetComponent<CharacterIdentity>();
    }

    void Update()
    {
        var input = InputSystemController.Instance;

        if (input is null) return;

        if (input.GetPlayerConfirmPressed() && _target is not null)
        {
            _target.Interact(_characterIdentity.CharacterDefinition as AllyDefinitionSO);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out InteractionBase interactionBase))
        {
            _target = interactionBase;
            interactionBase.OnFocus(_characterIdentity.CharacterDefinition as AllyDefinitionSO);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out InteractionBase interactionBase))
        {
            interactionBase.OnUnfocus(_characterIdentity.CharacterDefinition as AllyDefinitionSO);
            _target = null;
        }
    }
}
