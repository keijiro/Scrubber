using UnityEngine;

namespace Scrubber
{
    [System.Serializable]
    public struct Page
    {
        public string videoName;
        public bool autoPlay;
        public bool loop;
        public string text;
        public Texture image;
    }

    public sealed class Deck : ScriptableObject
    {
        [SerializeField] Page[] _pages = null;

        public int pageCount { get { return _pages.Length; } }

        public Page GetPage(int index) { return _pages[index]; }
    }
}
