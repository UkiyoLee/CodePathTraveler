/// <summary>
/// 实现人物的基本移动，同步实现动画效果
/// </summary>
public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;
    [SerializeField] private float speed = 5.0f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundSnapForce = -2f;
    [SerializeField] private float maxGravity = -30f;
    private float verticalVelocity;

    private Vector2 movementInput;

    private bool isMoving;

    // Unity的Awake方法，在对象实例化时自动调用，比Start更早执行
    private void Awake()
    {
        // 获取当前游戏对象上的CharacterController组件，并赋值给characterController变量
        // 这样就可以在脚本的其他方法中使用这个组件来控制角色
        characterController = GetComponent<CharacterController>();
        // 获取当前游戏对象上的Animator组件，并赋值给animator变量
        // 这样就可以在脚本的其他方法中使用这个组件来控制动画
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        Movement();
        SetAnimation();
    }

    private void Movement()
    {
        // 获取输入系统控制器的实例
        var input = InputSystemController.Instance;

        // 如果输入系统控制器实例不存在，则直接返回
        if (input == null) return;

        // 获取移动输入值
        movementInput = input.GetMovementInput();
        // Debug.Log(movementInput); // 调试日志，输出移动输入值（已注释）

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
            // 限制最大下落速度
            if (verticalVelocity < maxGravity)
            {
                verticalVelocity = maxGravity;
            }
        }

        // 计算角色速度，包括水平和垂直方向
        Vector3 velocity = new Vector3(movementInput.x, 0, movementInput.y) * speed;
        velocity.y = verticalVelocity;

        // 使用角色控制器移动角色，考虑时间增量以保持运动平滑
        characterController.Move(velocity * Time.deltaTime);
        // Debug.Log(characterController);
    }

    private void SetAnimation()
    {
        if (!animator) return;

        isMoving = movementInput.magnitude > 0.1;
        animator.SetBool("isMoving", isMoving);

        if (isMoving)
        {
            animator.SetFloat("moveX", movementInput.x);
            animator.SetFloat("moveY", movementInput.y);
        }
    }
}