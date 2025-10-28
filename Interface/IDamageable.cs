namespace CBLib
{
    /// <summary>
    /// 데미지를 받을 수 있는 오브젝트 인터페이스
    /// </summary>
    
    public interface IDamageable
    {
        /// <summary>데미지 받기</summary>
        void TakeDamage(float damage);
    }
}