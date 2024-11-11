using System;
using UnityEngine;

// Add �����ϰ�, Multiple�ϰ�, �������� Override�ϴ� ������
// enum���� ���� ������ ���εǾ��ֱ� ������ ���� (0, 1, 2,...) 
// => ���Ŀ� ���� Ȱ���ϸ� ������������ ����Ȱ���ؼ� ü�������� ����ȿ�� ������� ���� ����
// Q: Override�� �������� ������ ���� ȿ���� �������?
// A: ���ݷ��� �����ؾ��ϴ� Ư�� �����̳� �⺻ ���ݷ� ���뿡 Ȱ�� ����
public enum StatsChangeType
{
	Add,
	Multiple,
	Override,
}

// Ŭ������ Serializable�� ����θ� �����Ǿ� ������ [Serializable]�� �ٿ� �����Ϳ��� Ȯ�� �����ؿ�!
[Serializable]
public class CharacterStat
{
	public StatsChangeType statsChangeType;
	[Range(0, 100)] public int maxHealth;
	[Range(0f, 20f)] public float speed;
	public AttackSO attackSO;
}