using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CBLib
{
    /// <summary>
    /// Google Sheet 기반 데이터 테이블을 관리하는 기본 매니저
    /// 모든 외부 데이터를 로드, 캐싱, 조회하는 기본 기능 제공
    /// </summary>
    public abstract class BaseDataTableManager : MonoBehaviour
    {
        [Header("Google Sheet Settings")]
        [SerializeField] protected string googleSheetUrl = "";
        [SerializeField] protected bool loadOnStart = true;

        protected DataTableLoader _loader;
        protected Dictionary<string, object> _cachedData = new Dictionary<string, object>();

        protected virtual bool ShouldDontDestroyOnLoad()
        {
            return true; // DataTableManager는 씬 전환 시에도 유지되어야 함
        }

        protected virtual void OnSingletonAwake()
        {
            InitializeLoader();
        }

        protected virtual void OnSingletonStart()
        {
            if (loadOnStart)
            {
                LoadAllTables();
            }
        }

        protected virtual void InitializeLoader()
        {
            _loader = gameObject.AddComponent<DataTableLoader>();
            if (!string.IsNullOrEmpty(googleSheetUrl))
            {
                _loader.SetUrl(googleSheetUrl);
            }
        }

        /// <summary>
        /// Google Sheet URL 설정
        /// </summary>
        public virtual void SetGoogleSheetUrl(string url)
        {
            googleSheetUrl = url;
            if (_loader != null)
            {
                _loader.SetUrl(url);
            }
        }

        /// <summary>
        /// 모든 데이터 테이블 로드 (하위 클래스에서 구현)
        /// </summary>
        public abstract void LoadAllTables();

        /// <summary>
        /// 특정 시트의 데이터를 로드하고 캐싱
        /// </summary>
        public virtual IEnumerator LoadTable<T>(string sheetName, Action<T> onLoaded = null) where T : class
        {
            if (_loader == null)
            {
                Debug.LogError("❌ DataTableLoader가 초기화되지 않았습니다.");
                yield break;
            }

            Debug.Log($"🔄 {sheetName} 시트 로딩 중...");

            bool isDone = false;
            T loadedData = null;

            yield return _loader.LoadTable<T>(sheetName, data =>
            {
                loadedData = data;
                _cachedData[sheetName] = data;
                isDone = true;
            });

            yield return new WaitUntil(() => isDone);

            if (loadedData != null)
            {
                Debug.Log($"✅ {sheetName} 시트 로딩 완료!");
                onLoaded?.Invoke(loadedData);
            }
            else
            {
                Debug.LogError($"❌ {sheetName} 시트 로딩 실패!");
            }
        }

        /// <summary>
        /// 캐시된 데이터 가져오기
        /// </summary>
        public virtual T GetCachedData<T>(string sheetName) where T : class
        {
            if (_cachedData.TryGetValue(sheetName, out object data))
            {
                return data as T;
            }
            return null;
        }

        /// <summary>
        /// 특정 시트가 로드되었는지 확인
        /// </summary>
        public virtual bool IsTableLoaded(string sheetName)
        {
            return _cachedData.ContainsKey(sheetName);
        }

        /// <summary>
        /// 캐시된 모든 데이터 초기화
        /// </summary>
        public virtual void ClearCache()
        {
            _cachedData.Clear();
            Debug.Log("🗑️ 데이터 캐시가 초기화되었습니다.");
        }

        /// <summary>
        /// 특정 시트의 캐시만 초기화
        /// </summary>
        public virtual void ClearTableCache(string sheetName)
        {
            if (_cachedData.Remove(sheetName))
            {
                Debug.Log($"🗑️ {sheetName} 시트 캐시가 초기화되었습니다.");
            }
        }

        /// <summary>
        /// 로딩 상태 확인
        /// </summary>
        public virtual bool IsLoading { get; protected set; }

        /// <summary>
        /// 로딩 진행률 (0.0 ~ 1.0)
        /// </summary>
        public virtual float LoadingProgress { get; protected set; }

        #if UNITY_EDITOR
        [ContextMenu("DEBUG/Reload All Tables")]
        private void DebugReloadAllTables()
        {
            LoadAllTables();
        }

        [ContextMenu("DEBUG/Clear All Cache")]
        private void DebugClearAllCache()
        {
            ClearCache();
        }
        #endif
    }
}
