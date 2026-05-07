using UnityEngine;

[CreateAssetMenu(menuName = "Player Combat/Combo Data")]
public class PlayerComboData : ScriptableObject
{
    [SerializeField] private PlayerComboStep[] steps;

    public int StepCount
    {
        get
        {
            if (steps == null)
            {
                return 0;
            }

            return steps.Length;
        }
    }

    public bool IsEmpty
    {
        get
        {
            return StepCount == 0;
        }
    }

    public PlayerAttackData GetAttackAtStep(int index)
    {
        if (steps == null)
        {
            return null;
        }

        if (index < 0 || index >= steps.Length)
        {
            return null;
        }

        return steps[index].AttackData;
    }

    public AttackInputType GetInputAtStep(int index)
    {
        return steps[index].InputType;
    }

    public bool MatchesInputSequence(AttackInputType[] inputSequence, int sequenceLength)
    {
        if (steps == null)
        {
            return false;
        }

        if (sequenceLength > steps.Length)
        {
            return false;
        }

        for (int i = 0; i < sequenceLength; i++)
        {
            if (steps[i].InputType != inputSequence[i])
            {
                return false;
            }
        }

        return true;
    }
}