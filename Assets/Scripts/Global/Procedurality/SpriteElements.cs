using System;
using UnityEngine;

namespace Doceragnum
{
    [Serializable]
    public struct SpriteEntry
    {
        public string Name;
        public Sprite Sprite;
    }

    [CreateAssetMenu(fileName = "Elements", menuName = "Procedurality/Sprite Elements")]
    public class SpriteElements : ScriptableObject
    {
        public SpriteEntry[] Hairs;
        public SpriteEntry[] Shirts;
        public SpriteEntry[] Pants;
    }
}