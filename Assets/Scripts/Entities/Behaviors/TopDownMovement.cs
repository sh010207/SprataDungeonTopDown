using UnityEngine;

// TopDownMovement�� ĳ���Ϳ� ������ �̵��� ���� �����Դϴ�.
public class TopDownMovement : MonoBehaviour
{
    private TopDownController movementController;
    private CharacterStatHandler characterStatHandler;
    private Rigidbody2D movementRigidbody;

    private Vector2 _movementDirection = Vector2.zero;

    private Vector2 _knockback = Vector2.zero;
    private float knockbackDuration = 0.0f;

    private void FixedUpdate()
    {
        ApplyMovement(_movementDirection);
        // �˹鿡 Ÿ�̸� ��� ����
        if(knockbackDuration > 0.0f)
        {
            knockbackDuration -= Time.fixedDeltaTime;
        }
    }
    
    private void Awake()
    {
    // ���� ���ӿ�����Ʈ�� TopDownController, Rigidbody�� ������ �� 
        movementController = GetComponent<TopDownController>();
        movementRigidbody = GetComponent<Rigidbody2D>();
        characterStatHandler = GetComponent<CharacterStatHandler>();
    }

    private void Start()
    {
        // OnMoveEvent�� Move�� ȣ���϶�� �����
        movementController.OnMoveEvent += Move;
    }

    private void Move(Vector2 direction)
    {
        // �̵����⸸ ���صΰ� ������ ���������� ����.
        // �����̴� ���� ���� ������Ʈ���� ����(rigidbody�� �����ϱ�)
        _movementDirection = direction;
    }

    public void ApplyKnockback(Transform other, float power, float duration)
    {
        // TODO : ���� ȣ�� �κ��� ���� ����. ���� ������ �ǰ� �����ϸ鼭 ����
        knockbackDuration = duration;
        _knockback = -(other.position - transform.position).normalized * power;
    }

    private void ApplyMovement(Vector2 direction)
    {
        direction = direction * characterStatHandler.CurrentStat.speed;

        // �˹� �޴� ��� �̵��� �ݿ�
        if(knockbackDuration > 0.0f)
        {
            direction += _knockback;
        }
        movementRigidbody.velocity = direction;
    }
}