using UnityEngine;

public class TopDownContactEnemyController : TopDownEnemyController
{
    [SerializeField][Range(0f, 100f)] private float followRange;
    [SerializeField] private string targetTag = "Player";
    private bool isCollidingWithTarget;

    [SerializeField] private SpriteRenderer characterRenderer;

    private HealthSystem healthSystem;
    private HealthSystem collidingTargetHealthSystem;
    private TopDownMovement collidingMovement;

    protected override void Start()
    {
        base.Start();
    
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.OnDamage += OnDamage;
    }

    private void OnDamage()
    {
        followRange = 100f;
    }

    protected override void FixedUpdate()
    {
        // �ܰŸ����� �÷��̾�ó�� �Է��� �޾Ƽ� �����̴� ���� �ƴմϴ�.
        // �׷��� �ܰŸ����� � ������ ������ �� ������ ���� ���� �����ؾ� �մϴ�.

        base.FixedUpdate();

        if(isCollidingWithTarget)
        {
            ApplyHealthChange();
        }
        
        Vector2 direction = Vector2.zero;
        if (DistanceToTarget() < followRange)
        {
            direction = DirectionToTarget();
        }

        CallMoveEvent(direction);
        Rotate(direction);
    }

    private void Rotate(Vector2 direction)
    {
        // TopDownAimRotation���� �߾���? 
        // Atan2�� ���ο� ������ ������ �������� -����~����(-180��~180���� ����, * Rad2Deg�� �� ���)�ϴ� ���� ��Ÿ���ִ� �Լ����ٴ� �� ����Ͻ���?
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        characterRenderer.flipX = Mathf.Abs(rotZ) > 90f;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject receiver = collision.gameObject;

        if(!receiver.CompareTag(targetTag))
        {
            return;
        }

        collidingTargetHealthSystem = receiver.GetComponent<HealthSystem>();
        if(collidingTargetHealthSystem != null )
        {
            isCollidingWithTarget = true;
        }

        collidingMovement = receiver.GetComponent<TopDownMovement>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag(targetTag))
        {
            return;
        }

        isCollidingWithTarget = false;
    }

    private void ApplyHealthChange()
    {
        AttackSO attackSO = stats.CurrentStat.attackSO;
        bool hasBeenChanged = collidingTargetHealthSystem.ChangeHealth(-attackSO.power);
        if(attackSO.isOnKnockback && collidingMovement != null )
        {
            collidingMovement.ApplyKnockback(transform, attackSO.knockbackPower, attackSO.knockbackTime);
        }
    }
}