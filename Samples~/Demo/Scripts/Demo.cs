using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KulibinSpace.SimpleAltimeterWidget.Demo {
    
	public class Demo : MonoBehaviour {

		public SimpleAltimeterWidget altimeter;

		void Update () {
			altimeter.heightValue = transform.position.y;
		}
	}

}

