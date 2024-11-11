using UnityEngine;

public class TopDownShooting : MonoBehaviour
{
    private TopDownController contoller;
    private Vector2 aimDirection = Vector2.right;

    [SerializeField] private Transform projectileSpawnPosition;
    [SerializeField] private AudioClip ShootingClip;

    private void Awake()
    {
        contoller = GetComponent<TopDownController>();
    }

    void Start()
    {
        contoller.OnAttackEvent += OnShoot;
        // OnLookEvent�� ���� �ΰ��� ��ϵǴ� ��(�ϳ��� ���� �ð��� ����߾���? TopDownAimRotation.OnAim(Vec2)
        // �� ���� ��������Ʈ�� ���� ���� �Լ��� ��ϵǾ��ִ� ���� multicast delegate��� ��.
        // Action�̳� Func�� ��������Ʈ�� ������ �� ����Ͻ���..?
        contoller.OnLookEvent += OnAim;
    }

    private void OnAim(Vector2 newAimDirection)
    {
        aimDirection = newAimDirection;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void OnShoot(AttackSO attackSO)
    {
        RangedAttackSO RangedAttackSO = attackSO as RangedAttackSO;
        float projectilesAngleSpace = RangedAttackSO.multipleProjectilesAngel;
        int numberOfProjectilesPerShot = RangedAttackSO.numberofProjectilesPerShot;

        float minAngle = -(numberOfProjectilesPerShot / 2f) * projectilesAngleSpace + 0.5f * RangedAttackSO.multipleProjectilesAngel;


        for (int i = 0; i < numberOfProjectilesPerShot; i++)
        {
            float angle = minAngle + projectilesAngleSpace * i;
            float randomSpread = Random.Range(-RangedAttackSO.spread, RangedAttackSO.spread);
            angle += randomSpread;
            CreateProjectile(RangedAttackSO, angle);
        }
    }

    private void CreateProjectile(RangedAttackSO rangedAttackSo, float angle)
    {
        // ������Ʈ Ǯ�� Ȱ���� �������� ����
        GameObject obj = GameManager.Instance.ObjectPool.SpawnFromPool(rangedAttackSo.bulletNameTag);
        
        // �߻�ü �⺻ ����
        obj.transform.position = projectileSpawnPosition.position;
        ProjectileController attackController = obj.GetComponent<ProjectileController>();
        attackController.InitializeAttack(RotateVector2(aimDirection, angle), rangedAttackSo);
        
        if(ShootingClip) SoundManager.PlayClip(ShootingClip);
        obj.SetActive(true);
    }

    private static Vector2 RotateVector2(Vector2 v, float degree)
    {
        return Quaternion.Euler(0, 0, degree) * v;
    }
}