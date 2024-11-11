using System;
using UnityEngine;

public class TopDownController : MonoBehaviour
{
    // Action�� void�� �޼ҵ带 ����� �� �ִµ� Vector2�� ���ڷ� �޴� �޼ҵ带 ����ϵ��� Action<Vector2>�� ����
    // event Ű���带 �Է��ϸ� public�̴��� ���� Invoke����.
    public event Action<Vector2> OnMoveEvent;
    public event Action<Vector2> OnLookEvent;
    
    // OnAttackEvent�� ������ �� ���� ��������(AttackSO)�� ��� ��
    public event Action<AttackSO> OnAttackEvent;

    private float timeSinceLastAttack = float.MaxValue;
    protected bool isAttacking;

    protected CharacterStatHandler stats { get; private set; }

    protected virtual void Awake()
    {
        stats = GetComponent<CharacterStatHandler>();
    }

    protected virtual void Update()
    {
        HandleAttackDelay();
    }

    private void HandleAttackDelay()
    {
        if (timeSinceLastAttack <= stats.CurrentStat.attackSO.delay)
        {
            timeSinceLastAttack += Time.deltaTime;
        }

        if (isAttacking && timeSinceLastAttack > stats.CurrentStat.attackSO.delay)
        {
            timeSinceLastAttack = 0;
            // ���� ������ ������ attackSO����
            CallAttackEvent(stats.CurrentStat.attackSO);
        }
    }

    public void CallMoveEvent(Vector2 direction)
    {
        // onMoveEvent�� public�̾ TopDownMovement���� �޼ҵ���� ����س���(a.k.a. ����)
        OnMoveEvent?.Invoke(direction);
    }

    public void CallLookEvent(Vector2 direction)
    {
        // ���� �ý��ۿ��� �������.
        OnLookEvent?.Invoke(direction);
    }

    public void CallAttackEvent(AttackSO attackSO)
    {
        // TopDownShooting���� ����� OnShoot �޼ҵ尡 �����Ǿ� ����.
        OnAttackEvent?.Invoke(attackSO);
    }
}
