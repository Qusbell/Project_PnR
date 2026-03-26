    using UnityEngine;

/// <summary>
/// IMover 인터페이스의 기본 구현체
/// </summary>
public class Mover_Test : MonoBehaviour, IDriver
{
    // ==== Component ==== //

    private Rigidbody2D _rigid;
    private Rigidbody2D Rigid
    {
        get
        {
            if (_rigid == null )
            {
                if (!TryGetComponent<Rigidbody2D>(out _rigid))
                { _rigid = gameObject.AddComponent<Rigidbody2D>(); }

                // 물리값 조절
                _rigid.gravityScale = 0;
                _rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            }

            return _rigid;
        }
    }


    // ==== Field ==== //

    /// <summary>
    /// <-- 스크립터블이든 NetVariable이든 뭐든 나중에 따로 빼둬야 할듯?
    /// </summary>
    [field: SerializeField]
    public float MoveSpeed { get; private set; } = 5f;


    // ==== Custom ==== //

    /// <summary>
    /// 입력된 방향으로 오브젝트를 이동시킴 <br/>
    /// direction은 normalized되어 있다고 가정
    /// </summary>
    public void MoveAt(ICompass compass)
    {
        if (compass == null) { Rigid.linearVelocity = Vector2.zero; return; }
        Rigid.linearVelocity = compass.IsActivate ? compass.Direction * MoveSpeed : Vector2.zero;
    }

}