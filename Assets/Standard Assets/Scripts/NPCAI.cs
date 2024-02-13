using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;

public class NPCAI : RoleAI
{
    public enum Camp
    {
        ENEMY,
        NEUTRAL
    }
    public Camp camp;
    public bool Offensive { get { return m_Offensive; } set { } }
    [SerializeField] bool m_Offensive;
    [SerializeField] float m_WanderRadius;
    [SerializeField] float m_WanderBeat = 3.0f;
    float m_IdleDuring;
    public override void Init(RoleControl role)
    {
        base.Init(role);
        switch (camp)
        {
            case NPCAI.Camp.ENEMY:
                m_role.gameObject.layer = LayerMask.NameToLayer("Enemy");
                EnemyDetector.layers = LayerMask.GetMask("Player");
                InteractDetector.layers = LayerMask.GetMask("Interactable");
                break;
            case NPCAI.Camp.NEUTRAL:
                m_role.gameObject.layer = LayerMask.NameToLayer("Interactable");
                EnemyDetector.layers = LayerMask.GetMask("Noting");
                InteractDetector.layers = LayerMask.GetMask("Interactable", "Player");
                m_Offensive = false;
                break;
            default:
                break;
        }
    }
    protected override void OnRoleEvent(GameObject obj, string eventName)
    {
        if (eventName == "roleEvent_OnIdling")
        {
            m_IdleDuring += Time.deltaTime;
            if (m_IdleDuring > m_WanderBeat && m_WanderRadius > 0)
            {
                Wandering();
                m_IdleDuring = 0;
            }
        }
        if (eventName == "roleEvent_OnDamage")
        {
            if (!m_Offensive) m_Offensive = true;
            if (!m_role.CurrentEnemy) m_role.CurrentEnemy = EnemyDetector.GetNearest()?.GetComponent<CharacterData>();
            if (EnemyDetector.Radius < 10) EnemyDetector.Radius = 10;
        }
        if (eventName == "roleEvent_OnPursuing" && Offensive)
        {
            m_role.Pursuing();
        }
        // UIRoleHud hud = m_role.GetComponentInChildren<UIRoleHud>();
        // if (hud != null) hud.Bubble(eventName);
    }

    void Wandering()
    {
        float randomX = Random.Range(0f, m_WanderRadius);
        float randomZ = Random.Range(0f, m_WanderRadius);

        Vector3 randomPos = new Vector3(m_role.BirthPos.x + randomX, m_role.BirthPos.y, m_role.BirthPos.z + randomZ);
        m_role.MoveTo(randomPos);
        //Debug.Log($"{m_role}-Wandering => {randomPos}");
    }
}
