using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace CBLib
{
    public class GameResourceSpriteAtlas : GameResource<SpriteAtlas>
    {
        protected Dictionary<string, Sprite> _spriteResources = new Dictionary<string, Sprite>();

        public GameResourceSpriteAtlas(string pathName, string extension = "") : base(pathName, extension)
        {
        }

        public virtual void Init()
        {
            if (_spriteResources.Count > 0) return;
            
            AllResourceLoad();
            Dictionary<string, SpriteAtlas>.Enumerator enumerator = GetAllResource().GetEnumerator();
            while (enumerator.MoveNext())
            {
                SpriteAtlas spriteAtlas = enumerator.Current.Value;
                Sprite[] sprites = new Sprite[spriteAtlas.spriteCount];
                spriteAtlas.GetSprites(sprites);
                for (int index = 0; index < sprites.Length; ++index)
                {
                    int length = sprites[index].name.IndexOf("(Cl");
                    if (length > 0)
                    {
                        _spriteResources.Add(sprites[index].name.Substring(0, length), sprites[index]);
                    }
                }
            }
        }

        public Sprite GetSprite(string _strFileName)
        {
            Sprite sprite;
            return _spriteResources.TryGetValue(_strFileName, out sprite) ? sprite : null;
        }
    }
}
