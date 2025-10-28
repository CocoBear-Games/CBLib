namespace CBLib
{
    /// <summary>
    /// 모든 매니저가 구현해야 하는 기본 인터페이스
    /// </summary>
    public interface IBaseManager
    {
        /// <summary>
        /// 매니저 초기화
        /// </summary>
        void Init();
        
        /// <summary>
        /// 초기화 완료 여부
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// 씬 전환 후 컴포넌트 재연결
        /// </summary>
        void OnSceneReloaded();
    }
}
