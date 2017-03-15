using UnityEngine;

class CharTPSCamera : MonoBehaviour
{

    //Кешированное значение трансформа камеры
    private static Transform _tr;
    //Трансформ цели камеры
    private Transform _target;
    //Сюда сохраняем расстояние до ближней плоскости отсечения у найденной камеры
    private float _nearclip;

    //Управляем ли мы персонажем
    private bool _isControlable = true;

    //Текущее расстояние от камеры до персонажа
    public float Distance { get; set; }

//#region Constant
    //Начальное расстояние от камеры до персонажа.
    private const float _START_DISTANCE = 4f;  //2
    //Минимальное расстояние от камеры до персонажа.
    private const float _DISTANCE_MIN = _START_DISTANCE;
    //Максимальное расстояние от камеры до персонажа.
    private const float _DISTANCE_MAX = 12f;  //12
    //Параметр смягчения приближения/удаления камеры
    private const float _DISTANCE_SMOOTH = 0.05f;
    //Параметр смягчения восстановления расстояния от камеры
    private const float _DISTANCE_RESUME_SMOOTH = 0.2f;
    //Значение смягчения смещения мыши по осям X и Y
    private const float _X_SMOOTH = 0.05f;
    private const float _Y_SMOOTH = 0.1f;

    //Константы - чувствительность мыши в горизонтальном, вертикальном направлениях и чувствительность колесика
    private const float _X_MOUSE_SENSITIVITY = 5f;
    private const float _Y_MOUSE_SENSITIVITY = 5f;
    private const float _MOUSE_WHEEL_SENSITIVITY = 5f;
    //"Мертвая зона" внутри которой не реагируем на вращение колесика мышки
    private const float _DEAD_ZONE = 0.01f;

    //Ограничения вращения по вертикали - минимальное и максимальное
    private const float _Y_MIN_LIMIT = -10f;  //-40
    private const float _Y_MAX_LIMIT = 20f;    //80

    //Максимальное количество шагов рассчета окклюжн камеры
    private const int _MAX_OCCLUSION_CHECK = 10;
    //Шаг изменения расстояния при рассчете оклюжн
    private const float _OCCLUSION_STEP = 0.2f;
//#endregion

    //Текущая скорость приближения/удаления камеры
    private float _velDistance;
    //Скорости движения по соответствующим осям
    private float _velX;
    private float _velY;
    private float _velZ;

    //Значение смещения мыши по осям X и Y
    private float _mouseX;
    private float _mouseY;

    //Рассчитанное желаемое расстояние от камеры до персонажа.
    private float _desireDistance;
    //Полученное желаемое положение камеры
    private Vector3 _desirePosition;

    //Последнее время попытки вернуть камеру на расстояние, которое было до окклюжн
    private float _lastTime;
    //переменная - вводится для того чтобы можно было оперативно менять сглаживание изменения расстояния
    private float _distanceSmooth;
    //Расстояние до камеры перед тем как обнаружили объект перекрывающий ее
    private float _preOccludedDistance;

    //Окончательная позиция камеры
    private Vector3 _position;

    void Start()
    {
        //Т.к. Distance -публичный мы можем его установить извне в неверное значение. Здесь мы загоняем его в нужные рамки
        Distance = Mathf.Clamp(Distance, _DISTANCE_MIN, _DISTANCE_MAX);
        //Вызываем метод, устанавливающий начальные значения переменных.
        Reset();
    }

    void LateUpdate()
    {

        //Проверяем - можем ли мы управлять персонажем. Если нет - возврат
        if (!_isControlable) return;
        //Проверяем не исчезла ли цель камеры, если исчезла ничего не делаем.
        if (_target == null) return;

        //Вводим данные с мыши.
        _PlayerInput();
        //Рассчитываем желаемую позицию камеры.
        _CalcDesirePositionClip();
        //Смещаем камеру.
        _UpatePosition();
    }

    private void _PlayerInput()
    {
        //Проверяем - нажата ли правая кнопка мыши
      //  if(Input.GetMouseButton(1))
      //  {
            //Нажата - рассчитываем смещения с учетом чувствительности мыши
            _mouseX += Input.GetAxis("Mouse X") * _X_MOUSE_SENSITIVITY;
            _mouseY -= Input.GetAxis("Mouse Y") * _Y_MOUSE_SENSITIVITY;
       // }
        //Ограничиваем вращение по вертикали с учетом того, что оно может выходить за пределы диапазона [-360;360]
        _mouseY = Helper.ClampAngle(_mouseY, _Y_MIN_LIMIT, _Y_MAX_LIMIT);
        //Данные с колесика мышки
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        //Если вышли за пределы "мертвой зоны"
        if (scroll >= -_DEAD_ZONE && scroll <= _DEAD_ZONE) return;
        //Рассчитываем желаемое расстояние от камеры до персонажа
        //Введенное значение умножаем на чувствительность, вычитаем его из текущего расстояния и ограничиваем сверху и снизу
        _desireDistance = Mathf.Clamp(Distance - scroll*_MOUSE_WHEEL_SENSITIVITY, _DISTANCE_MIN, _DISTANCE_MAX);
        _preOccludedDistance = _desireDistance;
        _distanceSmooth = _DISTANCE_SMOOTH;
    }

    //Рассчитываем желаемую позицию камеры
    private void _CalcDesirePosition()
    {
        if (_desireDistance < _preOccludedDistance && (Time.time - _lastTime) > 0.2f)
        {
            //Пытаемся сбросить расстояние до камеры на то, которое было до окклюжн
            _ResetDesireDistance();
            _lastTime = Time.time;
        }
        //"Смягчаем" приближение/удаление камеры
        Distance = Mathf.SmoothDamp(Distance, _desireDistance, ref _velDistance, _distanceSmooth);
        //Собственно рассчитываем позицию. Обратите внимание на перекрестную передачу параметров _mouseX и _mouseY
        _desirePosition = _CalcPosition(_mouseY, _mouseX, Distance);
    }

    private void _CalcDesirePositionClip()
    {

        bool isOccluded;
        //Счетчик шагов рассчета оклюжн
        var count = 0;
        //Первоначальный рассчет желаемой позиции камеры
        _CalcDesirePosition();

        do
        {
            //Нет препятствия
            isOccluded = false;
            //Рассчитываем ближайшую точку препятствия по ближней плоскости отсечения
            //Начало - в позиции цели камеры, конец - в только что рассчитанной желаемой позиции
            var nearesDist = _CheckClip(_target.position, _desirePosition);
            //Если есть препятствие
            if (nearesDist != -1)
            {
                //и счетчик шагов не исчерпан
                if (count < _MAX_OCCLUSION_CHECK)
                {
                    //флаг - есть препятствие
                    isOccluded = true;
                    //сдвигаем камеру на шаг
                    Distance -= _OCCLUSION_STEP;
                    //Если слишком близко - ограничиваем. Тут не учитывается _DISTANCE_MIN! Камера может зайти внутрь персонажа!
                    //NOTE Чтобы избежать этого можно здесь использовать Bounding персонажа - подумать как...
                    if (Distance < 0.25f)
                        Distance = 0.25f;
                }
                else
                    //Если счетчик шагов исчерпан просто рывком перемещаем камеру на расстояние чуть меньшее чем препятствие
                    Distance = nearesDist - _nearclip;
                //Устанавливаем желаемое расстояние
                _desireDistance = Distance;
                _distanceSmooth = _DISTANCE_RESUME_SMOOTH;
                //Пересчитываем позицию камеры по измененному желаемому расстоянию
                _CalcDesirePosition();
            }

            //Прибавляем шаг
            count++;
        //Повторяем пока есть препятствие
        } while (isOccluded);
    }

    private void _ResetDesireDistance()
    {
        Vector3 pos = _CalcPosition(_mouseY, _mouseX, _preOccludedDistance);
        float nearestDistance = _CheckClip(_target.position, pos);
        if (nearestDistance == -1 || nearestDistance > _preOccludedDistance)
            _desireDistance = _preOccludedDistance;
    }

    private Vector3 _CalcPosition(float rotx, float roty, float distance)
    {
        //Точка прямо позади персонажа на расстоянии камеры
        Vector3 direction = new Vector3(0, 0, -distance);
        //Поворот вокруг персонажа на нужный угол
        Quaternion rotation = Quaternion.Euler(rotx, roty, 0);

        //Возвращаем нужную позицию камеры в мировом пространстве
        return _target.position + rotation * direction;
    }

    private float _CheckClip(Vector3 fromp, Vector3 to)
    {
        //Изначально устанавливаем такое значение, которое в результате рассчетов получиться не может
        float nearestDistance = -1f;

        //Создаем структуру - ближнюю плоскость отсечения
        ClipPlane cp = new ClipPlane(to);

        //Рисуем для отладки конус от персонажа к плоскости отсечения
        //DrawFrustum(fromp, to);

        float dist;
        if (_CheckClipI(fromp, cp.upperLeft, out dist))
            nearestDistance = dist;
        if (_CheckClipI(fromp, cp.lowerLeft, out dist))
            if (dist < nearestDistance || nearestDistance == -1)
                nearestDistance = dist;
        if (_CheckClipI(fromp, cp.lowerRight, out dist))
            if (dist < nearestDistance || nearestDistance == -1)
                nearestDistance = dist;
        if (_CheckClipI(fromp, cp.upperRight, out dist))
            if (dist < nearestDistance || nearestDistance == -1)
                nearestDistance = dist;

        if (_CheckClipI(fromp, to + _tr.forward * -_nearclip, out dist))
            if (dist < nearestDistance || nearestDistance == -1)
                nearestDistance = dist;

        //Возвращаем рассчитанное расстояние
        return nearestDistance;
    }

    private static bool _CheckClipI(Vector3 fr, Vector3 to, out float distance)
    {
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        //Флаг - было ли столкновение
        bool flag;
        //точка начала луча - сначала начальная переданная точка
        Vector3 start = fr;
        //Информация о месте удара
        RaycastHit hit;
        //Делаем
        do
        {
            //Ударились ли во что-то?
            flag = Physics.Linecast(start, to, out hit, layerMask);
            //рассчитываем новую точку старта луча
            start = hit.point + (to - start) * 0.01f;
        //Делаем до тех пор пока ударяемся во что-то и это что-то не плейер и расстояние до конечной точки луча достаточно большое
        } while (flag && hit.collider.tag == "Player" && (to - start).magnitude > 0.01f);
        //Окончательное расстояние до соударения.
        //Это мы возвращаем в out параметре
        distance = hit.distance;
        //Флаг - ударились ли вообще
        return flag;
    }

    public void DrawFrustum(Vector3 fromp, Vector3 to)
    {
        ClipPlane cp = new ClipPlane(to);
        Debug.DrawLine(fromp, to + _tr.forward * -_nearclip, Color.red);
        Debug.DrawLine(fromp, cp.upperRight);
        Debug.DrawLine(fromp, cp.upperLeft);
        Debug.DrawLine(fromp, cp.lowerRight);
        Debug.DrawLine(fromp, cp.lowerLeft);

        Debug.DrawLine(cp.upperRight, cp.upperLeft);
        Debug.DrawLine(cp.upperRight, cp.lowerRight);
        Debug.DrawLine(cp.lowerLeft, cp.lowerRight);
        Debug.DrawLine(cp.lowerLeft, cp.upperLeft);
    }

    public void Reset()
    {
        //Обнуляем данные, введенные с мыши.
        _mouseX = 0;
        _mouseY = 0f;
        //Расстояние от камеры до персонажа равно стартовому.
        Distance = _START_DISTANCE;
        //Желаемое расстояние тоже равно стартовому.
        _desireDistance = Distance;
        //Изначальное расстояние до обнаружения окклюжн
        _preOccludedDistance = Distance;
    }

    private void _UpatePosition()
    {
        //Сглаживаем движения по соответствующим осям
        float posX = Mathf.SmoothDamp(_position.x, _desirePosition.x, ref _velX, _X_SMOOTH);
        float posY = Mathf.SmoothDamp(_position.y, _desirePosition.y, ref _velY, _Y_SMOOTH);
        float posZ = Mathf.SmoothDamp(_position.z, _desirePosition.z, ref _velZ, _X_SMOOTH);

        //Формируем вектор - окончательное положение камеры
        _position = new Vector3(posX, posY, posZ);
        //Перемещаем камеру в рассчитанное положение
        _tr.position = _position;
        //Поворачиваем камеру так, чтобы она глядела на цель
        _tr.LookAt(_target);
    }

    static public void GetCamera()
    {
        //Временная камера
        GameObject tempCamera;

        //Если главная камера в сцене есть
        if (Camera.main != null)
            //берем ссылку на нее и присваиваем временной камере
            tempCamera = Camera.main.gameObject;
        else
        {
            //Иначе - создаем новую камеру
            tempCamera = new GameObject("Main Camera");
            //Добавляем ей компонент - камера
            tempCamera.AddComponent<Camera>();
            //И присваиваем тег - "Главная камера"
            tempCamera.tag = "Main Camera" ;
        }
        //Получаем объект - игрока
        GameObject pl = GameObject.FindGameObjectWithTag("Player");
        //Добавляем ему компонент - этот скрипт
        pl.AddComponent<CharTPSCamera>();
        //Получаем ссылку на добавленный к ГГ скрипт камеры
        CharTPSCamera myCamera = pl.GetComponent<CharTPSCamera>();

        //Находим объект, на который должна смотреть камера по названию
        GameObject targetTemp = GameObject.Find("targetLookAt");
        //Если не нашли
        if(targetTemp == null)
        {
            //Создаем новый объект
            targetTemp = new GameObject("targetLookAt");
            //Устанавливаем его в положение персонажа
            targetTemp.transform.position = Vector3.zero;
        }
        //В скрипте камеры, который мы навесили на персонаж устанавливаем цель камеры
        myCamera._target = targetTemp.transform;
        myCamera._nearclip = ((Camera) tempCamera.GetComponent(typeof(Camera))).nearClipPlane;

        //Запоминаем ссылку на трансформ камеры - кешируем
        _tr = tempCamera.transform;
    }

    private void _NotControl(bool val)
    {
        //Устанавливаем контролируем ли персонаж или нет
        _isControlable = val;
    }
}

