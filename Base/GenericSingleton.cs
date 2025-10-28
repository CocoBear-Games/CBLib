using UnityEngine;

namespace CBLib
{
    /// <summary>
    /// 제네릭을 사용한 타입 안전한 싱글톤 패턴
    /// MonoBehaviour를 상속받는 클래스에서 사용
    /// </summary>
    /// <typeparam name="T">싱글톤으로 만들 클래스 타입</typeparam>
    public abstract class GenericSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static bool _applicationIsQuitting = false;

        /// <summary>
        /// 싱글톤 인스턴스
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    Debug.LogWarning($"[{typeof(T).Name}] 애플리케이션이 종료 중입니다. null을 반환합니다.");
                    return null;
                }

                if (_instance == null)
                {
                    // 씬에서 기존 인스턴스 찾기
                    T existingInstance = FindFirstObjectByType<T>();
                    
                    if (existingInstance != null)
                    {
                        _instance = existingInstance;
                        
                        // 여러 개의 인스턴스가 있는지 확인하고 정리
                        T[] allInstances = FindObjectsByType<T>(FindObjectsSortMode.None);
                        int totalCount = allInstances.Length;
                        
                        if (totalCount > 1)
                        {
                            Debug.LogError($"[{typeof(T).Name}] 씬에 {totalCount}개의 인스턴스가 있습니다! 첫 번째만 남기고 제거합니다.");
                            
                            // 첫 번째를 제외한 모든 인스턴스 제거
                            for (int i = 1; i < totalCount; i++)
                            {
                                if (allInstances[i] != null)
                                {
                                    Destroy(allInstances[i]);
                                }
                            }
                        }
                    }

                    // 인스턴스가 없다면 새로 생성
                    if (_instance == null)
                    {
                        GameObject singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString();

                        // DontDestroyOnLoad 설정 (모든 싱글톤은 씬 전환 시 유지)
                        DontDestroyOnLoad(singletonObject);

                        Debug.Log($"[{typeof(T).Name}] 싱글톤 인스턴스가 생성되었습니다.");
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// 싱글톤 초기화 시 호출되는 메서드
        /// 하위 클래스에서 오버라이드하여 사용
        /// </summary>
        protected virtual void OnSingletonAwake() { }

        /// <summary>
        /// 싱글톤 시작 시 호출되는 메서드
        /// 하위 클래스에서 오버라이드하여 사용
        /// </summary>
        protected virtual void OnSingletonStart() { }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                
                // 기본적으로 DontDestroyOnLoad 설정
                DontDestroyOnLoad(gameObject);

                OnSingletonAwake();
            }
            else if (_instance != this)
            {
                Debug.LogWarning($"[{typeof(T).Name}] 중복된 인스턴스가 감지되어 제거됩니다.");
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (_instance == this)
            {
                OnSingletonStart();
            }
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _applicationIsQuitting = true;
            }
        }

        private void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }

        /// <summary>
        /// 싱글톤이 유효한지 확인
        /// </summary>
        /// <returns>싱글톤이 유효하면 true</returns>
        public static bool IsValid()
        {
            return _instance != null && !_applicationIsQuitting;
        }

        /// <summary>
        /// 싱글톤 강제 초기화
        /// </summary>
        public static void ForceInit()
        {
            if (_instance == null)
            {
                T temp = Instance;
                Debug.Log($"[{typeof(T).Name}] 싱글톤이 강제 초기화되었습니다. 인스턴스: {temp?.name ?? "null"}");
            }
        }
    }
}
