using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Other
{
    public class HandleSlideValueText : MonoBehaviour
    {
        public Slider Slider;
        public Text Text;
        // Start is called before the first frame update
        /*    void Start()
            {

            }
        */
        // Update is called once per frame
        void Update()
        {
            Change();
        }

        private void OnValidate()
        {
            Change();
        }

        public void Change()
        {
            Text.text = (Slider.value / 10).ToString();
        }

    }
}
