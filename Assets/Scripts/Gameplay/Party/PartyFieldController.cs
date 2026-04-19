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

    private void LateUpdate()
    {
        UpdateLeaderTrail();

        for (int i = 0; i < followers.Count; i++)
        {
            var follower = followers[i];
            float targetDistance = followDistance * (i + 1);
            Vector3 targetPos = GetPointAtDistance(targetDistance);
            follower.MoveTo(targetPos, followSpeed);
        }
    }

    /// <summary>
    /// 获取指定距离处的点
    /// </summary>
    /// <param name="distance">与Leader的距离</param>
    /// <returns></returns>
    private Vector3 GetPointAtDistance(float distance)
    {
        if (trail.Count == 0) return playerTrans.position;

        float accumulated = 0.0f;

        for (int i = 0; i < trail.Count - 1; i++)
        {
            Vector3 a = trail[i];
            Vector3 b = trail[i + 1];

            float dist = Vector3.Distance(a, b);

            if (accumulated + dist >= distance)
            {
                float t = (distance - accumulated) / dist;
                return Vector3.Lerp(a, b, t);
            }

            accumulated += dist;
        }
        return trail[^1];
    }

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
        RebuildTrailAndSnapFollowers();
    }

    private Vector3 ApplyFollowOffset(Vector3 position, int index)
    {
        position.z += zOffset * (index + 1);
        return position;
    }

    private void UpdateLeaderTrail()
    {
        Vector3 leaderPos = playerTrans.position;

        if (trail.Count == 0)
        {
            trail.Add(leaderPos);
            return;
        }

        float dist = Vector3.Distance(leaderPos, trail[0]);
        if (dist > sampleDistance)
        {
            trail.Insert(0, leaderPos);

            if (trail.Count > 30)
            {
                trail.RemoveAt(trail.Count - 1);
            }
        }
    }

    private void RebuildTrailAndSnapFollowers()
    {
        trail.Clear();

        for (int i = 0; i < followers.Count; i++)
        {
            followers[i].SnapTo(ApplyFollowOffset(playerTrans.position, i));
        }
        UpdateLeaderTrail();
    }
}
