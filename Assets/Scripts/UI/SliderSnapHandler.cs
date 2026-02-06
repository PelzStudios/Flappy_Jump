using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Slider))]
public class SliderSnapHandler : MonoBehaviour, IPointerUpHandler
{
    private Slider m_Slider;

    private void Awake()
    {
        m_Slider = GetComponent<Slider>();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (m_Slider != null)
        {
            // Snap to nearest whole number
            int snappedValue = Mathf.RoundToInt(m_Slider.value);
            m_Slider.value = snappedValue;

            // Update Game Difficulty
            if (DifficultyManager.Instance != null)
            {
                DifficultyManager.Instance.SetDifficulty(snappedValue);
            }
        }
    }
}
