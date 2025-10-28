using System;
using TMPro;
using UnityEngine;

namespace CBLib
{
    /// <summary>
    /// 리소스 매니저의 기본 구현을 제공하는 추상 클래스
    /// </summary>
    public abstract class BaseResourceManager<U> : GenericSingleton<U>, IResourceManager, IBaseManager where U : BaseResourceManager<U>
    {
        // Prefab ============================
        protected GameResource<GameObject> resourcePrefab;
        
        // UI ============================
        protected GameResource<GameObject> resourceUIPrefab;
        
        // Effect ============================
        protected GameResource<GameObject> resourceEffect;
        
        // Sprite ============================
        protected GameResource<Sprite> resourceSprite;
        protected GameResourceSpriteAtlas resourceSpriteAtlas;
        
        // Audio ============================
        protected GameResource<AudioClip> resourceAudio;
        
        // Material ============================
        protected GameResource<Material> resourceMaterial;
        
        // TextAsset ============================
        protected GameResource<TextAsset> resourceTextAsset;
        
        // TMP_FontAsset ============================
        protected GameResource<TMP_FontAsset> resourceFontAsset;

        public bool IsInitialized { get; set; } = false;

        public void Init()
        {
            NewGameResource();
            InitGameResource();
            
            Debug.Log($"🟢 - [ {GetType().Name} ] Initialize Completed!");
            IsInitialized = true;
        }

        /// <summary>
        /// IBaseManager 인터페이스 구현 - 씬 전환 후 컴포넌트 재연결
        /// </summary>
        public void OnSceneReloaded()
        {
            Debug.Log($"🔄 {GetType().Name} 씬 전환 후 리소스 재연결 중...");
            
            // 리소스 관련 컴포넌트들이 null이 되었을 수 있으므로 재초기화
            if (resourceSpriteAtlas != null)
            {
                resourceSpriteAtlas.Init();
            }
            
            Debug.Log($"✅ {GetType().Name} 리소스 재연결 완료!");
        }

        protected virtual void NewGameResource()
        {
            // Prefab ============================
            resourcePrefab = new GameResource<GameObject>("Prefabs, Prefabs/UI, Prefabs/Effect", "prefab");
            
            // UI ============================
            resourceUIPrefab = new GameResource<GameObject>("Prefabs/UI/Prefab, Prefabs/UI/Map", "prefab");
            
            // Effect ============================
            resourceEffect = new GameResource<GameObject>("Effect, Effect/Base Game");
            
            // Sprite ============================
            resourceSprite = new GameResource<Sprite>("Sprites");
            resourceSpriteAtlas = new GameResourceSpriteAtlas("SpriteAtlas");
            
            // Audio ============================
            resourceAudio = new GameResource<AudioClip>("Sound, Sound/SFX, Sound/BGM", "ogg, wav");
            
            // Material ============================
            resourceMaterial = new GameResource<Material>("Materials");
            
            // TextAsset ============================
            resourceTextAsset = new GameResource<TextAsset>("TextAsset");
            
            // TMP_FontAsset ============================
            resourceFontAsset = new GameResource<TMP_FontAsset>("FontAsset");
        }

        private void InitGameResource()
        {
            resourceSpriteAtlas.Init();
            resourceAudio.AllResourceLoad();
        }

        #region << =========== Prefab =========== >>

        public GameObject GetPrefab(string fileName) => resourcePrefab.GetResource(fileName);
        public void DelPrefab(string fileName) => resourcePrefab.DelResource(fileName);

        #endregion

        #region << =========== UI =========== >>

        public GameObject GetUIPrefab(string fileName) => resourceUIPrefab.GetResource(fileName);
        public void DelUIPrefab(string fileName) => resourceUIPrefab.DelResource(fileName);

        #endregion

        #region << =========== Effect =========== >>

        public GameObject GetEffectPrefab(string fileName) => resourceEffect.GetResource(fileName);
        public void DelEffectPrefab(string fileName) => resourceEffect.DelResource(fileName);

        #endregion
        
        #region << =========== Sprite =========== >>
        
        public Sprite GetSprite(string fileName) => resourceSprite.GetResource(fileName);
        public void DelSprite(string fileName) => resourceSprite.DelResource(fileName);

        public Sprite GetSpriteAtlas(string fileName) => resourceSpriteAtlas.GetSprite(fileName);
        public void DelSpriteAtlas(string fileName) => resourceSpriteAtlas.DelResource(fileName);
        
        #endregion
        
        #region << =========== Audio =========== >>

        public AudioClip GetAudioClip(string fileName) => resourceAudio.GetResource(fileName);
        public void DelAudioClip(string fileName) => resourceAudio.DelResource(fileName);
        
        #endregion
        
        #region << =========== Material =========== >>
        
        public Material GetMaterial(string fileName) => resourceMaterial.GetResource(fileName);
        public void DelMaterial(string fileName) => resourceMaterial.DelResource(fileName);
        
        #endregion
        
        #region << =========== TextAsset =========== >>
        
        public TextAsset GetTextAsset(string fileName) => resourceTextAsset.GetResource(fileName);
        public void DelTextAsset(string fileName) => resourceTextAsset.DelResource(fileName);
        
        #endregion
        
        #region << =========== TMP_FontAsset =========== >>
        
        public TMP_FontAsset GetFontAsset(string fileName) => resourceFontAsset.GetResource(fileName);
        public void DelFontAsset(string fileName) => resourceFontAsset.DelResource(fileName);
        
        #endregion
    }
}
