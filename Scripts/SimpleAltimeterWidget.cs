using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KulibinSpace.SimpleAltimeterWidget {

	// простой бесконечный скроллер внутри этого контейнера
	// GetComponent<RectTransform>().rect это габариты относительно точки pivot
	// Надо задавать clone.GetComponent<RectTransform>().anchoredPosition, причём высоту элемента трогать нельзя
	//[ExecuteInEditMode]
	public class SimpleAltimeterWidget : MonoBehaviour {

		RectTransform scrollContainer; // попытка отделить список
		public float heightValue = 154.8f; // значение высотомера, соотв. середине контейнера. Для тестирования ставлю высоту 154.8м.
		public GameObject item; // префаб прокручиваемого элемента
		public float itemSizeWorld = 100f; // Задаётся габарит плашки в мировой координате (зависит от наличной разметки самой плашки), по умолчанию 100 м.
		public RectTransform mark; // метка диапазона высот, просто полоска, накладываемая на шкалу. Мировой масштаб у неё рассчитывается автоматически
		public float markPos; // высота метки диапазона высот, соответствующая середине самой метки (так удобнее настраивать виджет)
		RectTransform rt; // cache
		float kWorld; // коэффициент приведения габаритов к мировой системе координат
		float markSizeWorld;
		bool markStatus = false;
		AltimeterScrollableItem[] items; // кэш элементов
		RectTransform markOff;

		public void MarkOff () {
			mark.gameObject.SetActive(false);
			markOff = mark;
			mark = null;
		}

		private void Awake () {
			rt = GetComponent<RectTransform>();
		}

		void Start () {
			scrollContainer = GetComponent<RectTransform>();
			// загрузим хотя бы один элемент и вычислим всё необходимое
			GameObject clone = Instantiate(item, scrollContainer);
			Rect clonerect = clone.GetComponent<RectTransform>().rect; // прямоугольник плашки
			kWorld = itemSizeWorld / clonerect.height;
			if (mark) {
				markStatus = mark.gameObject.activeInHierarchy;
				markSizeWorld = kWorld * mark.rect.height;
			}
			// загрузим все остальные видимые в экранных габаритах элементы, причём один уже есть
			int viewable = Mathf.FloorToInt(rt.rect.height / clonerect.height) + 1; 
			while (viewable > 0) {
				Instantiate(item, scrollContainer);
				viewable -= 1;
			}
			items = scrollContainer.GetComponentsInChildren<AltimeterScrollableItem>();
		}

		// поставил в LateUpdate, чтобы не мерцала текстура шкалы, но, кажется, не помогло.
		void LateUpdate () {
			float contTop = heightValue + kWorld * rt.rect.height / 2.0f; // у высотомера высота в середине, поэтому нужна половина контейнера
			float itemZeroTop = contTop - (contTop % itemSizeWorld) + itemSizeWorld; // высота точки нулевого элемента, кратная мировому габариту плашки
			float screenTop0 = (itemZeroTop - heightValue) / kWorld; // экранное расстояние от середины контейнера вверх до точки нулевого элемента.
			Vector2 elemPos = new Vector2(0, rt.rect.y + screenTop0);
			float elemHeight = itemZeroTop;
			for (int i = 0; i < items.Length; i++) {
				items[i].SetHeight(elemHeight);
				elemHeight -= itemSizeWorld;
				RectTransform irt = items[i].transform as RectTransform;
				irt.anchoredPosition = elemPos;
				elemPos.y -= irt.rect.height;
			}
			if (mark && markStatus) {
				// включить плашку диапазона высот, если она видна между contTop и contBottom
				float contBottom = heightValue - kWorld * rt.rect.height / 2.0f; // аналогично contTop
				// верх и низ метки в мировых координатах
				float markTop = markPos + markSizeWorld / 2f;
				float markBottom = markPos - markSizeWorld / 2f;
				// определяем видимость в мировых координатах, т.к. плашка ещё не размещена на экране
				if (markTop < contBottom || markBottom > contTop) {
					if (mark.gameObject.activeInHierarchy) mark.gameObject.SetActive(false);
				} else {
					if (!mark.gameObject.activeInHierarchy) mark.gameObject.SetActive(true);
					// текущая высота всегда в одной точке экрана. Поэтому надо рассчитать разность высот и разместить плашку на новом месте
					float dH = markPos - heightValue;
					// pivot контейнера в его середине (0, 0), и pivot плашки тоже в середине плашки, только смещён к левому краю.
					// Поэтому расчёт очень простой, dH это и есть разница между их серединами
					mark.anchoredPosition = new Vector2(0, dH / kWorld);
				}
			}
		}
	}
}
