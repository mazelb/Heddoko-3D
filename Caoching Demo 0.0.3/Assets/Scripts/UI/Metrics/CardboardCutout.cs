

using Assets.Scripts.Body_Data;
using Assets.Scripts.Body_Data.View;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Metrics
{
    public class CardboardCutout : MonoBehaviour
    {
        public Sprite Regular;
        public Sprite Straining;
        public Image RenderingPlane;
        public RenderedBody RenderedBody;
        private bool mCheatCodeActivated = false;
        private int mIndex = 0;
        public AudioSource AudioSource;
        private string[] mCheatCode = new[]
        {"i", "n", "z", "a", "n", "e", "i", "n", "t", "h","e","m", "e", "m", "b", "r", "a", "n", "e"};

        void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
        }
        public void Update()
        {
            if (mCheatCodeActivated)
            {
                if (RenderedBody != null)
                {
                    if (RenderedBody.AssociatedBodyView != null)
                    {
                        if (RenderedBody.AssociatedBodyView.AssociatedBody != null)
                        {
                            if (RenderedBody.AssociatedBodyView.AssociatedBody.LeftLegAnalysis != null)
                            {
                                if (
                                    Mathf.Abs(
                                        RenderedBody.AssociatedBodyView.AssociatedBody.LeftLegAnalysis.AngleKneeFlexion) >
                                    80)
                                {
                                    RenderingPlane.sprite = Straining;
                                }
                                else
                                {
                                    RenderingPlane.sprite = Regular;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                VerifyCheatCode();
            }
        }

        void VerifyCheatCode()
        {
            if (Input.anyKeyDown)
            {
                // Check if the next key in the code is pressed
                if (Input.GetKeyDown(mCheatCode[mIndex]))
                { 
                    // Add 1 to index to check the next key in the code
                    mIndex++;
                }
                // Wrong key entered, we reset code typing
                else {
                    mIndex = 0;
                }
            }

            // If index reaches the length of the cheatCode string, 
            // the entire code was correctly entered
            if (mIndex == mCheatCode.Length)
            {
                mCheatCodeActivated = true;
                AudioSource.Play();
                RenderingPlane.gameObject.SetActive(true);
                
            }
        }

    }
}
