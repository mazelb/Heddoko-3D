using UnityEngine.UI;
using UIWidgets;
using UnityEngine;

namespace Assets.Scripts.Body_Data.View
{
  public  class ProtoFrameDataListComponent : ListViewItem, IResizableItem
    {      

        /// <summary>
        /// The icon.
        /// </summary>
        [SerializeField]
        public Text SensorIndex;

        [SerializeField]
        public Text RawQuat;

        [SerializeField]
        public Text MappedQuat;

        [SerializeField]
        public Text RawEuler;

        [SerializeField]
        public Text MappedEuler;

        [SerializeField]
        public Text MagnetometerText;

        [SerializeField]
        public Text AccelerationText;
        /// <summary>
        /// Set icon native size.
        /// </summary>
        public bool SetNativeSize = true;

        ProtoFrameDataViewDescription mItem;

        /// <summary>
        /// Gets the objects to resize.
        /// </summary>
        /// <value>The objects to resize.</value>
        public GameObject[] ObjectsToResize
        {
            get
            {
                return new GameObject[] { SensorIndex.gameObject, RawQuat.gameObject, MappedQuat.gameObject, RawEuler.gameObject, MappedEuler.gameObject, MagnetometerText.gameObject};
            }
        }

        /// <summary>
        /// Gets the current mItem.
        /// </summary>
        /// <value>Current mItem.</value>
        public ProtoFrameDataViewDescription Item
        {
            get
            {
                return mItem;
            }
        }

        /// <summary>
        /// Sets component data with specified mItem.
        /// </summary>
        /// <param name="vNewItem">Item.</param>
        public virtual void SetData(ProtoFrameDataViewDescription vNewItem)
        {
            mItem = vNewItem;
            SensorIndex.text = string.Format("SENSOR {0}",vNewItem.Index );
            RawQuat.text = vNewItem.RawQuat;
            MappedQuat.text = vNewItem.MappedQuat;
            RawEuler.text = vNewItem.RawEuler;
            MappedEuler.text = vNewItem.MappedEuler;
            MagnetometerText.text = vNewItem.Magnetometer;
            AccelerationText.text = vNewItem.Acceleration;
        }


      public void SetColor(Color vColor)
      {
            SensorIndex.color = vColor; 
            RawQuat.color = vColor;
            MappedQuat.color = vColor;
            RawEuler.color = vColor;
            MappedEuler.color = vColor;
        }
    }
}
