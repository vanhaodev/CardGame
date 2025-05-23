using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Utils
{
    public class StepSlider : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private int[] _steps; // Mảng các bước
    private UnityAction<int> _onStepChanged;
    [SerializeField] private int _currentStepValue;

    private void OnValidate()
    {
        _slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        _slider.onValueChanged.AddListener(OnValueChanged);
        SetSlideMaxValue();
    }

    private void OnDisable()
    {
        _slider.onValueChanged.RemoveListener(OnValueChanged);
    }

    // Thêm listener mới
    public void AddListener(UnityAction<int> action)
    {
        _onStepChanged += action;
    }

    // Xóa listener
    public void RemoveListener(UnityAction<int> action)
    {
        _onStepChanged -= action;
    }

    // Xóa tất cả listeners
    public void RemoveAllListeners()
    {
        _onStepChanged = null;
    }

    // Cập nhật mảng các bước và maxValue của slider
    public void SetStep(int[] steps)
    {
        _steps = steps;
        SetSlideMaxValue();
    }

    // Thiết lập giá trị slider từ bước (step)
    public void SetStepValue(int stepValue)
    {
        // Tìm chỉ số của bước trong mảng _steps
        int index = Array.IndexOf(_steps, stepValue);
        if (index >= 0)
        {
            // Cập nhật giá trị của slider theo chỉ số của bước
            //Debug.LogError(index);
            _slider.value = index;
            _currentStepValue = stepValue;

            // Gọi sự kiện listener với bước hiện tại
            _onStepChanged?.Invoke(_currentStepValue);
        }
    }

    // Lấy chỉ số của bước gần nhất từ giá trị slider
    private int GetStepIndex(float value)
    {
        int stepIndex = (int)value; // Làm tròn giá trị thành chỉ số bước gần nhất
        return Mathf.Clamp(stepIndex, 0, _steps.Length - 1); // Đảm bảo giá trị trong phạm vi hợp lệ
    }

    // Lấy giá trị bước hiện tại
    public int GetStepValue()
    {
        return _currentStepValue;
    }

    // Thiết lập maxValue cho slider bằng số lượng bước - 1 (để đảm bảo slider có giá trị hợp lệ)
    private void SetSlideMaxValue()
    {
        if (_steps != null && _steps.Length > 0)
        {
            // Cập nhật maxValue của slider bằng số lượng bước - 1
            _slider.maxValue = _steps.Length - 1;
        }
    }

    // Xử lý sự kiện thay đổi giá trị slider
    private void OnValueChanged(float value)
    {
        // Tìm chỉ số bước gần nhất từ giá trị slider
        int closestStepIndex = GetStepIndex(value);

        // Cập nhật bước hiện tại
        _currentStepValue = _steps[closestStepIndex];

        // Gọi sự kiện listener với giá trị của bước
        _onStepChanged?.Invoke(_currentStepValue);
    }
}
}