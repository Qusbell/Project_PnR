using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

/// <summary>
/// 특정 행동의 모든 버튼을 수집 후, <br/>
/// 해당 버튼들 중 하나라도 눌려있는지(Any Key Down)을 판정 <br/>
/// 키보드 기준으로 제작되었으며, 타 조작기기에서는 다른 방식으로 판정해야 할 것으로 추정됨
/// </summary>
public class ButtonInputValidator
{
     /// <summary>
     /// IsPhysicallyPressed에서 사용할 버튼들 전수조사 저장용 List
     /// </summary>
    private List<ButtonControl> _moveButtons = new();
    public List<ButtonControl> MoveButtons => _moveButtons;

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="controls">특정 행동(controls)의 모든 버튼을 수집</param>
    public ButtonInputValidator (UnityEngine.InputSystem.Utilities.ReadOnlyArray<InputControl> controls)
    {
        foreach (var control in controls)
        {
            if(control is ButtonControl button)
            { MoveButtons.Add(button); }    
        }
    }

    /// <summary>
    /// 키보드에서 ← + → 입력 시 Released 나가는 거 때문에(Vector2.zero를 무조건 canceled로 판정해버림)<br/>
    /// 하다하다 안 돼서 그냥 물리적인 키 입력 자체를 전수조사하는 방법을 채택 <br/>
    /// </summary>
    public bool IsPhysicallyPressed
    {
        get
        {
            // MoveBy 액션에 바인딩된 모든 컨트롤(키) 중 하나라도 눌려있는지 확인
            foreach (var button in MoveButtons)
            {
                if (button.isPressed)
                { return true; }
            }
            return false;
        }
    }
}
