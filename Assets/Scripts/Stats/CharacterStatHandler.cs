using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CharacterStatHandler : MonoBehaviour
{
    // �⺻ ���Ȱ� ���� ���ȵ��� �ɷ�ġ�� �����ؼ� ������ ����ϴ� ������Ʈ
    [SerializeField] private CharacterStat baseStats;
    public CharacterStat CurrentStat { get; private set; } = new();
    public List<CharacterStat> statsModifiers = new List<CharacterStat>();

    private readonly float MinAttackDelay = 0.03f;
    private readonly float MinAttackPower = 0.5f;
    private readonly float MinAttakSize = 0.4f;
    private readonly float MinAttackSpeed = .1f;

    private readonly float MinSpeed = 0.8f;

    private readonly int MinMaxHealth = 5;

    private void Awake()
    {
        UpdateCharacterStat();

        if (baseStats.attackSO != null)
        {
            baseStats.attackSO = Instantiate(baseStats.attackSO);
            CurrentStat.attackSO = Instantiate(baseStats.attackSO);
        }
    }

    private void UpdateCharacterStat()
    {
        ApplyStatModifier(baseStats);

        foreach (CharacterStat stat in statsModifiers.OrderBy(o => o.statsChangeType))
        {
            ApplyStatModifier(stat);
        }

    }
    private void ApplyStatModifier(CharacterStat stat)
    {
        Func<float, float, float> operation = stat.statsChangeType switch
        {
            StatsChangeType.Add => (current, change) => current + change,
            StatsChangeType.Multiple => (current, change) => current * change,
            _ => (current, change) => change
        };

        UpdateBasicStats(operation, stat);
        UpdateAttackStats(operation, stat);
        if (CurrentStat.attackSO is RangedAttackSO currentRanged && stat.attackSO is RangedAttackSO newRanged)
        {
            UpdateRangedAttackStats(operation, currentRanged , newRanged);
        }
    }
    private void UpdateRangedAttackStats(Func<float, float, float> operation, RangedAttackSO currentRanged, RangedAttackSO newRanged)
    {
        currentRanged.multipleProjectilesAngel = operation(currentRanged.multipleProjectilesAngel, newRanged.multipleProjectilesAngel);
        currentRanged.spread = operation(currentRanged.spread, newRanged.spread);
        currentRanged.duration = operation(currentRanged.duration, newRanged.duration);
        currentRanged.numberofProjectilesPerShot = Mathf.CeilToInt(operation(currentRanged.numberofProjectilesPerShot, newRanged.numberofProjectilesPerShot));
        currentRanged.projectileColor = UpdateColor(operation,currentRanged.projectileColor, newRanged.projectileColor);
    }
    private Color UpdateColor(Func<float, float, float> operation, Color current, Color stat)
    {
        return new Color(
            operation(current.r, stat.r),
            operation(current.g, stat.g),
            operation(current.b, stat.b),
            operation(current.a, stat.a));
    }
    private void UpdateAttackStats(Func<float, float, float> operation, CharacterStat stat)
    {
        if(CurrentStat.attackSO == null || stat.attackSO == null) {return;}

        var currentAttack = CurrentStat.attackSO;
        var newAttack = stat.attackSO;
        
        // 변경을 적용하되, 최소값을 적용한다.
        currentAttack.delay = Mathf.Max(operation(currentAttack.delay, newAttack.delay), MinAttackDelay);
        currentAttack.power = Mathf.Max(operation(currentAttack.power, newAttack.power), MinAttackPower);
        currentAttack.size = Mathf.Max(operation(currentAttack.size, newAttack.size),MinAttakSize);
        currentAttack.speed = Mathf.Max(operation(currentAttack.speed, newAttack.speed), MinAttackSpeed);

    }
    private void UpdateBasicStats(Func<float, float, float> operation, CharacterStat stat)
    {
        CurrentStat.maxHealth = Mathf.Max((int)operation (CurrentStat.maxHealth, stat.maxHealth), MinMaxHealth);
        CurrentStat.speed = Mathf.Max((int)operation (CurrentStat.speed, stat.speed), MinSpeed);
    }

    public void AddStatModifiers(CharacterStat stat)
    {
        statsModifiers.Add(stat);
        UpdateCharacterStat();
    }

    public void RemoveStatModifiers(CharacterStat stat)
    {
        statsModifiers.Remove(stat);
        UpdateCharacterStat();
    }
}