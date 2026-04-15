using System;

public class FieldFollower : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Animator animator;

    [Header("Animator Params")]
    [SerializeField] private string isMovingParam = "isMoving";
    [SerializeField] private string moveXParam = "moveX";
    [SerializeField] private string moveYParam = "moveY";

    [Header("最小位移阈值")]
    [SerializeField] private float movementThreshold = 0.001f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundSnapForce = -2f;
    [SerializeField] private float maxGravity = -30f;
    private float verticalVelocity;


    # region 对外接口
    public void SetupFollower(CharacterDefinitionSO definition)
    {
        characterController = GetComponent<CharacterController>();
        animator.runtimeAnimatorController = definition.fieldAnimator;
    }

    public void MoveTo(Vector3 targetPosition, float speed)
    {
        // 计算目标方向向量，并确保Y轴为0，使角色只能在水平面上移动
        Vector3 toTarget = targetPosition - transform.position;
        toTarget.y = 0;

        // 限制水平移动的最大速度，确保角色不会超过设定的移动速度
        Vector3 horizontalStep = Vector3.ClampMagnitude(toTarget, Mathf.Max(0, speed));

        // 检查角色是否在地面上
        bool isGrounded = characterController.isGrounded;

        // 如果角色在地面上且垂直速度小于0（防止轻微下落）
        if (isGrounded && verticalVelocity < 0)
        {
            // 将垂直速度设置为地面吸附力，使角色稳定在地面上
            verticalVelocity = groundSnapForce;
        }
        else
        {
            // 如果不在地面上，应用重力效果
            verticalVelocity += gravity * Time.deltaTime;
            // 限制最大下落速度，防止下落过快
            if (verticalVelocity < maxGravity)
            {
                verticalVelocity = maxGravity;
            }
        }

        // 将水平移动和垂直速度合并为最终的移动向量
        Vector3 movement = horizontalStep;
        movement.y = verticalVelocity * Time.deltaTime;

        // 使用角色控制器应用最终的移动
        characterController.Move(movement);

        UpdateAnimation(horizontalStep);
    }
    # endregion

    private void UpdateAnimation(Vector3 step)
    {
        bool isMoving = step.magnitude > movementThreshold * movementThreshold;

        if (isMoving)
        {
            animator.SetFloat(moveXParam, step.x);
            animator.SetFloat(moveYParam, step.y);
        }
    }
}
