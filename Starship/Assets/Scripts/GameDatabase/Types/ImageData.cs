using UnityEngine;
using UnityEngine.UI;
using DebugLogSetting;

namespace GameDatabase.Model
{
    public struct ImageData
    {
        public ImageData(byte[] data)
        {
            var texture = new Texture2D(2, 2);
            if (texture.LoadImage(data) && texture.width == texture.height)
            {
                Sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), texture.width);
                GIFImageData = GIFImageData.Empty;
            }
            else if (texture.LoadImage(data) && texture.width / texture.height * texture.height == texture.width)
            {
                Sprite = null;
                GIFImageData = new GIFImageData(data);
            }
            else
            {
                Sprite = null;
                GIFImageData = GIFImageData.Empty;
            }
            Bytes = data;
        }
        public byte[] Bytes { get; }
        public Sprite Sprite { get; }
        public GIFImageData GIFImageData { get; }

        public static ImageData Empty = new ImageData();
    }
    public struct GIFImageData
    {
        public GIFImageData(byte[] data)
        {
            var texture = new Texture2D(2, 2);
            texture.LoadImage(data);
            int num = texture.width / texture.height;

            if (OtherDebugLogSetting.GifIconDebugLog)
                UnityEngine.Debug.Log("texture num  = " + num);

            Sprite = new Sprite[num];
            for (int i = 0; i < num; i++)
            {
                var _flame = new Texture2D(texture.height, texture.height);
                for (int j = 0; j < texture.height; j++)
                {
                    for (int k = 0; k < texture.height; k++)
                    {
                        _flame.SetPixel(k, j, texture.GetPixel(k + i * texture.height, j));
                    }
                }
                _flame.Apply();
                Sprite[i] = UnityEngine.Sprite.Create(_flame, new Rect(0, 0, texture.height, texture.height), new Vector2(0.5f, 0.5f), texture.height);
            }

        }
        //private Sprite _sprite;

        public Sprite[] Sprite { get; }

        public static GIFImageData Empty = new GIFImageData();
    }

}
