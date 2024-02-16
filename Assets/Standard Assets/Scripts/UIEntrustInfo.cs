using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEntrustInfo : UIWindow
{
    Entrustment m_entrust;
    public void Show(Entrustment data)
    {
        m_entrust = data;
        gameObject.SetActive(true);
    }
}
