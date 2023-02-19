using UnityEngine;

namespace GameDatabase.Model
{
    public struct AudioClipData
    {
        public AudioClipData(byte[] data)
        {
            AudioClip = OpenWavParser.ByteArrayToAudioClip(data);
            Bytes = data;
        }
        public byte[] Bytes { get; }
        public AudioClip AudioClip { get; }

        public static AudioClipData Empty = new AudioClipData();
    }
}
