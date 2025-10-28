using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace CBLib
{
    /// <summary>
    /// Health의 최소 책임만 가진 공용 베이스 컴포넌트
    /// - 전투 판정/속성/버프는 모름. 최종 수치만 적용.
    /// - 이벤트는 도메인 무의미한 순수 값만 발행.
    /// </summary>

    public class BaseCreatureHealth : MonoBehaviour, IDamageable
    {
        public float MaxHP { get; protected set; }
        public float CurrentHP { get; protected set; }
        public float NormalizedHP => MaxHP > 0f ? Mathf.Clamp01(CurrentHP / MaxHP) : 0f; // UI HP Bar 처리
        public bool IsDead { get; protected set; }

        /// <summary>실제 깎인 체력</summary>
        public event Action<float> Damaged;
        /// <summary>실제 회복된 체력</summary>
        public event Action<float> Healed;
        /// <summary>사망 시 1회</summary>
        public event Action Died;

        protected virtual void Awake()
        {
            // Awake에서 자동 초기화하지 않음 (외부에서 Initialize 호출)
        }

        /// <summary>최대/현재 체력 초기화</summary>
        public virtual void Initialize(float maxHP, float? startHP = null)
        {
            MaxHP = Mathf.Max(1f, maxHP);
            CurrentHP = Mathf.Clamp(startHP ?? MaxHP, 0f, MaxHP);
            IsDead = CurrentHP <= 0f;
        }

        /// <summary>최종 피해 수치만 적용</summary>
        public virtual void TakeDamage(float damage)
        {
            if (IsDead) return;

            float dmg = Mathf.Max(0f, damage);
            if (dmg <= 0f) return;

            float prevHP = CurrentHP;
            CurrentHP = Mathf.Max(0f, CurrentHP - dmg);
            float applied = prevHP - CurrentHP;

            if (applied > 0f) Damaged?.Invoke(applied);

            if (CurrentHP <= 0f)
            {
                IsDead = true;
                Died?.Invoke();
            }
        }

        public virtual void Heal(float amount)
        {
            if (IsDead) return;

            float heal = Mathf.Max(0f, amount);
            if (heal <= 0f) return;

            float prev = CurrentHP;
            CurrentHP = Mathf.Min(MaxHP, CurrentHP + heal);
            float applied = CurrentHP - prev;

            if (applied > 0f) Healed?.Invoke(applied);
        }

        public virtual void Kill()
        {
            if (IsDead) return;

            float prev = CurrentHP;
            CurrentHP = 0f;
            IsDead = true;

            if (prev > 0f) Damaged?.Invoke(prev);
            Died?.Invoke();
        }

        public virtual void Revive(float hp)
        {
            float reviveHp = Mathf.Clamp(hp, 1f, Mathf.Max(1f, MaxHP));
            CurrentHP = reviveHp;
            IsDead = false;
            Healed?.Invoke(reviveHp);
        }
        
        #if UNITY_EDITOR
        [ContextMenu("DEBUG/Take 10 Damage")] protected void DebugTake10() => TakeDamage(10f);
        [ContextMenu("DEBUG/Heal 10")]       protected void DebugHeal10() => Heal(10f);
        [ContextMenu("DEBUG/Kill")]          protected void DebugKill() => Kill();
        [ContextMenu("DEBUG/Revive Half")]   protected void DebugReviveHalf() => Revive(Mathf.Ceil(MaxHP * 0.5f));
        #endif
    }
}
