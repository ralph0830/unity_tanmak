using UnityEngine;
using MobileTanmak.Core;

namespace MobileTanmak.Bullet.Patterns
{
    /// <summary>
    /// 탄막 패턴의 기본 인터페이스
    /// 모든 패턴은 이 인터페이스를 구현해야 함
    /// </summary>
    public interface IBulletPattern
    {
        /// <summary>
        /// 패턴 초기화
        /// </summary>
        void Initialize(BulletSpawner spawner);

        /// <summary>
        /// 패턴 실행
        /// </summary>
        void Execute();

        /// <summary>
        /// 패턴 중지
        /// </summary>
        void Stop();

        /// <summary>
        /// 패턴이 실행 중인지 확인
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// 패턴 이름 (디버깅용)
        /// </summary>
        string PatternName { get; }
    }
}
