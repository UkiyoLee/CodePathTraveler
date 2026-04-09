public class CharacterIdentity : MonoBehaviour
{
    [SerializeField] private CharacterDefinitionSO _characterDefinition;
    public CharacterDefinitionSO CharacterDefinition => _characterDefinition;

    public void SetCharacterDefinition(CharacterDefinitionSO characterDefinition)
    {
        _characterDefinition = characterDefinition;
    }
}
