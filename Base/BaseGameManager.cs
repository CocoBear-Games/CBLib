using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CBLib
{
    /// <summary>
    /// BaseGameManager를 상속받는 GameManager를 만들어주세요.
    /// GameManager 오브젝트를 생성하여 모든 씬에 배치해주세요.
    /// 그리고 AddManagers 함수에 필요한 매니저를 등록해주세요.
    /// </summary>
    public abstract class BaseGameManager : MonoBehaviour
    {
        protected List<IBaseManager> _managers = new List<IBaseManager>();

        public bool IsInitialized { get; private set; } = false;

        public static BaseGameManager Instance { get; private set; }

        // 매니저들을 자식으로 생성할 부모 오브젝트
        protected GameObject _managerParent;

        protected virtual void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = 60;

            // ==== Manager ==== 오브젝트 생성 (0,0,0 위치)
            CreateManagerParent();

            AddManagers();
        }

        /// <summary>
        /// 매니저들을 자식으로 생성할 부모 오브젝트 생성
        /// </summary>
        private void CreateManagerParent()
        {
            _managerParent = new GameObject("==== Manager ====");
            _managerParent.transform.position = Vector3.zero;
            DontDestroyOnLoad(_managerParent);
        }

        /// <summary>
        /// 매니저를 부모 오브젝트의 자식으로 생성
        /// </summary>
        protected T CreateManager<T>() where T : Component, IBaseManager
        {
            string managerName = typeof(T).Name;
            GameObject managerObj = new GameObject(managerName);
            managerObj.transform.SetParent(_managerParent.transform);
            managerObj.transform.localPosition = Vector3.zero;

            T manager = managerObj.AddComponent<T>();
            _managers.Add(manager);

            return manager;
        }

          /// <summary>
        /// 모든 매니저들이 초기화되었는지 확인
        /// </summary>
        protected bool AllManagersInitialized()
        {
            foreach (var manager in _managers)
            {
                if (!manager.IsInitialized)
                {
                    return false;
                }
            }
            return true;
        }

        protected abstract void AddManagers();

        private void Start()
        {
            Debug.Log($"🔵 - [ {GetType().Name} ] Initialize Start!");

            StartCoroutine(Initialize());
        }

        private IEnumerator Initialize()
        {
            yield return StartCoroutine(InitializeManagers());
            InitializeManagerForce();
            InitializeCompleted();
        }

        private IEnumerator InitializeManagers()
        {
            yield return null;
            foreach (var manager in _managers)
            {
                manager.Init();
                yield return new WaitUntil(() => manager.IsInitialized);
            }
        }

        protected abstract void InitializeManagerForce();

        private void InitializeCompleted()
        {
            Debug.Log($"🔵 - [ {GetType().Name} ] Initialize Completed!");
            IsInitialized = true;
            OnInit();
        }

        protected abstract void OnInit();

        // 씬 전환 대응
        protected void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        protected void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // 모든 매니저들에게 씬 전환 알림
            foreach (var manager in _managers)
            {
                if (manager != null)
                {
                    manager.OnSceneReloaded();
                }
            }
            
            // 다른 씬에서 돌아왔을 때 추가할 컴포넌트가 있다면 여기서 추가
        }

          /// <summary>
        /// 씬에 있으면 사용, 없으면 생성해서 GameManager 자식으로 붙임
        /// 씬 전환 후에도 안전하게 컴포넌트를 찾거나 생성
        /// </summary>
        protected T EnsureRuntime<T>(ref T field, string goName) where T : Component
        {
            // 이미 유효한 참조가 있다면 반환
            if (field != null && field.gameObject != null) return field;

            // 씬에서 기존 컴포넌트 찾기
            field = FindAnyObjectByType<T>();
            if (field != null)
            {
                Debug.Log($"✅ {goName} 컴포넌트를 씬에서 찾았습니다: {field.name}");
                return field;
            }

            // 씬에 없다면 새로 생성
            var go = new GameObject(goName);
            go.transform.SetParent(transform);
            field = go.AddComponent<T>();
            Debug.Log($"🆕 {goName} 컴포넌트를 새로 생성했습니다: {go.name}");
            
            return field;
        }

        /// <summary>
        /// 씬 전환 후 특정 컴포넌트를 강제로 재찾기
        /// </summary>
        protected T ForceReconnectComponent<T>(ref T field, string componentName) where T : Component
        {
            if (field != null && field.gameObject != null) return field;

            field = FindAnyObjectByType<T>();
            if (field != null)
            {
                Debug.Log($"🔄 {componentName} 컴포넌트 재연결 완료: {field.name}");
            }
            else
            {
                Debug.LogWarning($"⚠️ {componentName} 컴포넌트를 씬에서 찾을 수 없습니다.");
            }
            
            return field;
        }
    }
}
