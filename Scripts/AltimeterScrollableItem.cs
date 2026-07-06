using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace KulibinSpace.SimpleAltimeterWidget {

	public class AltimeterScrollableItem : MonoBehaviour {

		public TextMeshProUGUI text;
		float cachedValue = 0.1f; // любое дробное значение, чтобы гарантированно изменить при первом появлении

		public void SetHeight (float h) {
			if (cachedValue != h) {
				cachedValue = h;
				text.text = h.ToString("0.#");
			}
		}

	}

}
