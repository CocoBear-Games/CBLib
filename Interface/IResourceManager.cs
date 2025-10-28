using UnityEngine;
using TMPro;

namespace CBLib
{
    /// <summary>
    /// 리소스 매니저가 구현해야 하는 기본 인터페이스
    /// </summary>
    public interface IResourceManager
    {
        // 기본 리소스 접근 메서드들
        GameObject GetPrefab(string fileName);
        void DelPrefab(string fileName);
        
        GameObject GetUIPrefab(string fileName);
        void DelUIPrefab(string fileName);
        
        GameObject GetEffectPrefab(string fileName);
        void DelEffectPrefab(string fileName);
        
        Sprite GetSprite(string fileName);
        void DelSprite(string fileName);
        
        Sprite GetSpriteAtlas(string fileName);
        void DelSpriteAtlas(string fileName);
        
        AudioClip GetAudioClip(string fileName);
        void DelAudioClip(string fileName);
        
        Material GetMaterial(string fileName);
        void DelMaterial(string fileName);
        
        TextAsset GetTextAsset(string fileName);
        void DelTextAsset(string fileName);
        
        TMP_FontAsset GetFontAsset(string fileName);
        void DelFontAsset(string fileName);
    }
}
