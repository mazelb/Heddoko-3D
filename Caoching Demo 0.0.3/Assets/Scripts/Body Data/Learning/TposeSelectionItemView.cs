using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Body_Data.Learning
{
    public class TposeSelectionItemView : MonoBehaviour
    {  
        public int StartInterval;
        public int EndInterval;
                
       public int IsTPoseValue
        {
            get
            {
                if(IsTPoseToggle.isOn)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
        

        public InputField StartField;
        public InputField EndField;
        public Toggle IsTPoseToggle;


        void Start()
        {
            StartField.onValueChange.AddListener(ChangeStartInterval);
            EndField.onValueChange.AddListener(ChangeEndInterval);
             gameObject.name = gameObject.GetInstanceID().ToString();
        }

        void ChangeStartInterval(string vVal)
        {
            StartInterval = int.Parse(vVal);
        }
        void ChangeEndInterval(string vVal)
        {
            EndInterval = int.Parse(vVal);

        }
       
    }
}
