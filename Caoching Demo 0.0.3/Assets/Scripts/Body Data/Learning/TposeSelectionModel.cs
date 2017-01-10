using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts.Body_Data.Learning
{
 public   class TposeSelectionModel : MonoBehaviour
    {
        public GameObject trouvemoi;
        public int StartInterval;
        public int EndInterval;
        public bool IsTpose;

        public InputField StartField;
        public InputField EndField;
        public Toggle IsTPoseToggle; 


        void Start()
        {
            StartField.onValueChange.AddListener(ChangeStartInterval);
            EndField.onValueChange.AddListener(ChangeEndInterval);
            IsTPoseToggle.onValueChanged.AddListener(ChangeTposeToggle);
        }

        void ChangeStartInterval(string vVal)
        {
            StartInterval = int.Parse(vVal);
        }
        void ChangeEndInterval(string vVal)
        {
            EndInterval = int.Parse(vVal);

        }
        void ChangeTposeToggle(bool vVal)
        {
            IsTpose = vVal;
        }


    }
}
