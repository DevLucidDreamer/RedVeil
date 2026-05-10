using UnityEngine;

namespace RedVeil.Core
{
    /// <summary>
    /// 데미지를 받을 수 있는 모든 오브젝트가 구현해야 하는 인터페이스.
    /// 늑대, 사냥꾼, 깨지는 오브젝트 등 무엇이든 데미지를 받을 수 있다면 이걸 구현한다.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>현재 살아있는지 여부</summary>
        bool IsAlive { get; }

        /// <summary>데미지를 적용한다.</summary>
        /// <param name="amount">데미지 양 (양수)</param>
        /// <param name="hitPoint">피격된 월드 위치 (이펙트, 사운드 위치 계산용)</param>
        /// <param name="hitDirection">피격 방향 (넉백, 피격 모션용)</param>
        void TakeDamage(float amount, Vector3 hitPoint, Vector3 hitDirection);
    }
}