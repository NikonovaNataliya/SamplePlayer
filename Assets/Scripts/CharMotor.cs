using UnityEngine;

public class CharMotor : MonoBehaviour
{

    //public static CharMotor instance;
    private Transform _transform;
    //Ссылка на компонент - CharacterController
    private static CharacterController _unityController;
    
    //#region Constant
    //Скорость персонажа
    private const float _RUN_SPEED = 3f;
    private const float _FORWARD_SPEED = 2.0f;
    private const float _BACKWARD_SPEED = 1.0f;
    private const float _STRAFF_SPEED = 2.0f;

    //Вектора для масштабирования вектора движения в соответствии со скоростью в данном направлении
    private static readonly Vector3 _fSpeed = new Vector3(_STRAFF_SPEED, 1, _FORWARD_SPEED);
    private static readonly Vector3 _bSpeed = new Vector3(_STRAFF_SPEED, 1, _BACKWARD_SPEED);
    private static readonly Vector3 _rSpeed = new Vector3(_STRAFF_SPEED, 1, _RUN_SPEED);

    //Начальная скорость прыжка
    private const float _JUMP_SPEED = 5f;
    //Сила гравитации
    private const float _GRAVITY = 25f;
    //Максимальная вертикальная скорость
    private const float _MAX_VERT_SPEED = 20f;
    //Минимальное время между прыжками
    private const float _JUMP_REPEAT_TIME = 0.2f;
//#endregion

    //Последнее время прыжка
    private float _lastJumpTime = -1.0f;
    //Где было последнее столкновение. Применяется для определения на земле ли мы
    private CollisionFlags _collisionFlags;
    //Вертикальная скорость
    private float _verticalVelocity;
    public float _horizontalVelocity;

    void Awake()
    {
        //кешируем трансформ
        _transform = transform;
        //Находим компонент - CharacterController
        _unityController = GetComponent("CharacterController") as CharacterController;

    }

    public void _DidMove(Vector3 move)
    {
        //Поворачиваем персонаж соответственно камере
        _RotateChar(move);
        //Двигаем персонаж
        _ProcessMotion(move);
    }

    public void _ProcessMotion(Vector3 moveVector)
    {
        //Масштабируем вектор движения в соответствии со скоростью в данном направлении
        //Если двигаемся вперед используем для масштабирования _fSpeed, назад - _bSpeed
        moveVector = Vector3.Scale(moveVector, moveVector.z > 0 ? _fSpeed : _bSpeed);

        if (Input.GetKey(KeyCode.X)) {
            moveVector = Vector3.Scale(moveVector, _rSpeed);
        }
        
        //Добавляем вертикальную составляющую
        moveVector = new Vector3(moveVector.x, _verticalVelocity, moveVector.z);
        //Преобразуем вектор движения в мировое пространство.
        moveVector = _transform.TransformDirection(moveVector);
        //Применяем гравитацию
        moveVector = _ApplyGravity(moveVector);
        //Сохраняем вертикальную составляющую на будущее
        _verticalVelocity = moveVector.y;
        //Двигаем!
        _collisionFlags = _unityController.Move(moveVector * Time.deltaTime);
        //Получаем текущую горизонтальную скорость
        _horizontalVelocity = _unityController.velocity.magnitude;
    }

    private void _RotateChar(Vector3 move)
    {
        //Проверяем - двигается ли персонаж?
        if (move.x != 0 || move.z != 0)
        {
            //Если двигается - уставливаем его поворот в соответствии с поворотом камеры. Т.к. это нужно сделать только вокруг оси Y,
            //а вокруг X и Z оставить неизменным, то используем метод, который конструирует вращение из трех углов
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, transform.eulerAngles.z);
        }
    }

    private Vector3 _ApplyGravity(Vector3 move)
    {
        //Если вертикальная скорость меньше максимальной
        if (move.y > -_MAX_VERT_SPEED)
            //добавляем к ней влияние гравитации
            move.y -= _GRAVITY * Time.deltaTime;
        //Если столкновение снизу (значит это земля - мы на земле) и вертикальная скорость велика
        if (_collisionFlags == CollisionFlags.CollidedBelow && move.y < -1)
            //Ограничиваем ее, оставляя некий небольшой "прижимной эффект"
            move.y = -1;
        //Возвращаем скорректированный вектор движения
        return move;
    }

    private void _DidJump()
    {
        //Если с предыдущего прыжка пршло слишком мало времени
        if (_lastJumpTime + _JUMP_REPEAT_TIME > Time.time) return;
        //Если не на земле
        if (!_unityController.isGrounded) return;
        //Нужно сделать прыжок - устанавливаем начальную вертикальную скорость
        _verticalVelocity = _JUMP_SPEED;
        //Запоминаем время прыжка
        _lastJumpTime = Time.time;
    }
}
