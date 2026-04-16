using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyFieldController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform followersParent;
    [SerializeField] private GameObject fieldFollowerPrefab;
    [SerializeField] private Transform playerTrans;

    [Header("Settings")]
    [SerializeField] private float followDistance = 1.2f; //相邻两人的距离
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float zOffset = 0.01f;
    [SerializeField] private float sampleDistance = 0.05f;

    private List<Vector3> trail = new();
    private List<FieldFollower> followers = new();

    public void UpdateFollowers(List<CharacterDefinitionSO> partyMembers)
    {
        int followerCount = partyMembers.Count - 1;
        while (followers.Count < followerCount)
        {
            int index = followers.Count;
            var pos = ApplyFollowOffset(playerTrans.position, index);
            GameObject followerObj = Instantiate(fieldFollowerPrefab, pos, Quaternion.identity, followersParent);
            followers.Add(followerObj.GetComponent<FieldFollower>());
        }

        for (int i = 0; i < followers.Count; i++)
        {
            followers[i].SetupFollower(partyMembers[i + 1]);
        }
    }

    private Vector3 ApplyFollowOffset(Vector3 position, int index)
    {
        position.z += zOffset * (index + 1);
        return position;
    }
}
