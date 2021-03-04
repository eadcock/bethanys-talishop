using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpriteLoader
{
    private static Dictionary<string, Texture2D> _sprites;

    private static Dictionary<string, Texture2D> Sprites
    {
        get
        {
            if (_sprites != null) return _sprites;
            _sprites = new Dictionary<string, Texture2D>();
            return _sprites;
        }
    }

    public static void Add(string key, Texture2D sprite)
    {
        if (Sprites.ContainsKey(key))
        {
            Sprites[key] = sprite;
            Debug.LogWarning("Overwriting sprite. This is probably not intended.");
        }
        else
        {
            Sprites.Add(key, sprite);
        }
    }

    public static Texture2D Get(string key) => Sprites.ContainsKey(key) ? Sprites[key] : null;

    public static Texture2D Load(string path)
    {   
        Texture2D s = Get(path);
        if (s != null)
        {
            return s;
        }

        s = Resources.Load(path) as Texture2D;
        if(s != null)
        {
            Add(path, s);
            return s;
        }

        return null;
    }

    public static Sprite LoadAndCreate(string path)
    {
        Texture2D texture = Load(path);
        if (texture == null) return null;

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
    }
}
