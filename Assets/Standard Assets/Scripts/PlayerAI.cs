using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAI : RoleAI
{
    public override void Init()
    {
        base.Init();
        m_role.gameObject.layer = LayerMask.NameToLayer("Player");
        m_role.EnemyDetector.layers = LayerMask.GetMask("Enemy");
        m_role.Interactor.layers = LayerMask.GetMask("Interactable", "Player", "Neutral");
    }
}
