/** 
* @file FontAwesomeSpriteContainer.cs
* @brief Contains the FontAwesomeSpriteContainer class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved

*/

using UnityEngine;

namespace Assets.Scripts.Utils.Containers
{
    /// <summary>
    /// A container that has a sprite sheet of FontAwesomeIcons
    /// </summary>

    public class FontAwesomeSpriteContainer : ScriptableObject
    {
        private bool mInitialized=false;
        private Sprite[] mSprites;
        public Texture FontAwesomeSpriteSheet;
        /// <summary>
        /// Total number of sprites
        /// </summary>
        public int Count
        {
            get
            { 
                if (!mInitialized)
                { 
                    mInitialized = true;
                    Initialize();
                }
                return mSprites.Length;
            }
        }

        /// <summary>
        /// Returns a sprite from the Fontawesome sprite sheet
        /// </summary>
        /// <param name="vIndex"></param>
        /// <returns></returns>
        public Sprite GetSpriteAt(int vIndex)
        {
            if (!mInitialized)
            {
                mInitialized = true;
                Initialize();
            }
            if (vIndex < 0 || vIndex >= mSprites.Length)
            {
                return null;
            }
            return mSprites[vIndex];
        }

        /// <summary>
        /// Initializes the container
        /// </summary>
        private void Initialize()
        {
            if (mSprites == null || mSprites.Length == 0)
            {
                mSprites = Resources.LoadAll<Sprite>("ArtAssets/"+FontAwesomeSpriteSheet.name);
            }

        }

        /// <summary>
        /// Array operator overload
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Sprite this[int i]
        {
            get
            {
                Initialize();
                return mSprites[i];
            } 
        }
    }
}