using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TE
{
    [CreateAssetMenu(menuName = "Items/Weapon Item")]
    public class WeaponItem : Item
    {
        public GameObject modelPrefab;
        public bool isUnarmed;

        [Header("Idle Animations")]
        public string right_hand_idle;
        public string left_hand_idle;

        [Header("Attack Animations")]
        public string OH_Normal_Attack1;
        public string OH_Normal_Attack2;
        public string OH_Light_Attack;
        public string OH_Heavy_Attack;
    }
}
