using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAI : RoleAI
{
    public override void Init(RoleControl role)
    {
        base.Init(role);
        m_role.gameObject.layer = LayerMask.NameToLayer("Player");
        EnemyDetector.layers = LayerMask.GetMask("Enemy");
        InteractDetector.layers = LayerMask.GetMask("Interactable");
    }
}
