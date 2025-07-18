using UnityEngine;

public interface ISkill
{
    bool CanCast();
    void Cast(Transform origin);
}
