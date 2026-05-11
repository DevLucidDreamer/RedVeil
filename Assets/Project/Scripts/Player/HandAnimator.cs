using UnityEngine;

namespace RedVeil.Player
{
    /// <summary>
    /// WeaponController의 OnFired 이벤트를 듣고 Animator의 Fire 트리거를 발동.
    /// HandSprite GameObject(Animator를 가진)에 부착하고, Inspector에서 WeaponController를 연결.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class HandAnimator : MonoBehaviour
    {
        [SerializeField] private WeaponController weapon;
        [SerializeField] private string fireTriggerName = "Fire";

        private Animator _animator;
        private int _fireTriggerHash;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _fireTriggerHash = Animator.StringToHash(fireTriggerName);
        }

        private void OnEnable()
        {
            if (weapon != null) weapon.OnFired += HandleFired;
        }

        private void OnDisable()
        {
            if (weapon != null) weapon.OnFired -= HandleFired;
        }

        private void HandleFired()
        {
            _animator.SetTrigger(_fireTriggerHash);
        }
    }
}