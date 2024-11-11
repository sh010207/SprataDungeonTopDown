using UnityEngine;

public class TopDownAimRotation : MonoBehaviour
{
    [SerializeField] private SpriteRenderer armRenderer;
    [SerializeField] private Transform armPivot;

    [SerializeField] private SpriteRenderer characterRenderer;

    private TopDownController _controller;

    private void Awake()
    {
        _controller = GetComponent<TopDownController>();
    }

    void Start()
    {
        // ���콺�� ��ġ�� ������ OnLookEvent�� ����ϴ� ��
        // ���콺�� ��ġ�� �޾Ƽ� ���� ������ �� Ȱ���� ����.
        _controller.OnLookEvent += OnAim;
    }

    public void OnAim(Vector2 newAimDirection)
    {
        // OnLook
        RotateArm(newAimDirection);
    }

    private void RotateArm(Vector2 direction)
    {
        // Atan2�� �����ﰢ���� �ִٰ� �� �� ���ΰ� y, ���ΰ� x�� �� �� ������ ���� [-Pi,Pi]�� ��Ÿ���� �Լ���
        // ������ -Pi�� -180��, Pi�� 180�� �̹Ƿ� Mathf.Rad2Deg�� �� 57.29�� (180 / 3.14)
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // [1. ĳ���� ������]
        // �̶� ������ ������(1,0 ����)�� 0���̹Ƿ�,
        // -90~90�������� �������� �ٶ󺸴� �� ������, -90�� �̸� 90�� �ʰ���� ������ �ٶ󺸴� ����.
        characterRenderer.flipX = Mathf.Abs(rotZ) > 90f;
        // �÷��̾� ����� ���ϴ�Ī�̶� ���������� ���� ����� ���ϴ�Ī�� �ƴ϶� �����������.
        armRenderer.flipY = characterRenderer.flipX;
        
        // [2. �� ������]
        // ���� ���� ���� ���� ������ �״�� �����ϴµ�, �̶� ����Ƽ ���ο��� ����ϴ� ���ʹϾ����� ��ȯ�Ѵ�.
        // ���ʹϾ����� �����ϴ� ��� �� ����
        // 1) Vector3�� Quaternion���� ��ȯ�ؼ� �ִ� ���
        //    Quaternion.Euler(x ȸ��, y ȸ��, z ȸ��) : ���Ϸ� �� �������� ���� ������ ���ʹϾ����� ��ȯ��
        // 2) ������Ƽ�� ���� �ڵ����� ��ȯ�ǰ� �ϴ� ���
        //    Transform.eulerAngles�� ����
        armPivot.rotation = Quaternion.Euler(0, 0, rotZ);
        // (2�� ������� �ϸ�) armPivot.eulerAngles = new Vector3(0, 0, rotZ);
    }
}