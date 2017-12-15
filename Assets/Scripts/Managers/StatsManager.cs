using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

namespace Assets.Scripts.Managers
{
    public class StatsManager : MonoBehaviour
    {
        [SerializeField]
        Slider HpBar;
        [SerializeField]
        Slider MpBar;
        [SerializeField]
        Slider ArmorBar;
        [SerializeField]
        Text AttackText;
        [SerializeField]
        Text ResistenceText;
        [SerializeField]
        Text SpeedText;
        [SerializeField]
        Text HitText;
        [SerializeField]
        Text EvadeText;

        // Use this for initialization
        void Start()
        {
            IsActive(false);
        }

        public void IsActive(bool active)
        {
            HpBar.gameObject.SetActive(active);
            MpBar.gameObject.SetActive(active);
            ArmorBar.gameObject.SetActive(active);
            AttackText.gameObject.SetActive(active);
            ResistenceText.gameObject.SetActive(active);
            SpeedText.gameObject.SetActive(active);
            HitText.gameObject.SetActive(active);
            EvadeText.gameObject.SetActive(active); 
        }

        public void SetHpBar(float health)
        {
            HpBar.value = health;
        }
        public void SetMpBar(float mpValue)
        {
            HpBar.value = mpValue;
        }

        public void SetArmorBar(float armorValue)
        {
            ArmorBar.value = armorValue;
        }        
        public void SetAttackText(int attackValue)
        {
            AttackText.text = "Atk: " + attackValue.ToString();
        }
        public void SetResistanceText(float resistanceValue)
        {
            ResistenceText.text = "Res: " + resistanceValue.ToString();
        }
        public void SetSpeedText(int speedValue)
        {
            SpeedText.text = "Spd: " + speedValue.ToString();
        }
        public void SetHitText(float hitValue)
        {
            HitText.text = "Hit: " + hitValue.ToString();
        }
        public void SetEvadeText(float evadeValue)
        {
            EvadeText.text = "Evade: " + evadeValue.ToString();
        }
    }
}
