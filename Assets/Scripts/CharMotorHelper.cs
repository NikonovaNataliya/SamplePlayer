using UnityEngine;

public struct ClipPlane
{
    //Точки, определяющие положение ближней плоскостиотсечения камеры
    public Vector3 upperLeft;
    public Vector3 upperRight;
    public Vector3 lowerLeft;
    public Vector3 lowerRight;

    //Конструктор. На входе - позиция камеры
    public ClipPlane(Vector3 pos)
    {
        //Если главной камеры нет
        if(Camera.main == null)
        {
            //Все точки просто устанавливаем равными входному значению
            upperRight = upperLeft = lowerRight = lowerLeft = pos;
            return;
        }

        Transform tr = Camera.main.transform;
        float distance = Camera.main.nearClipPlane;
        //Высота ближней плоскости отсечения
        float height = distance * Mathf.Tan((Camera.main.fieldOfView / 2) * Mathf.Deg2Rad);
        //ее ширина
        float width = height * Camera.main.aspect;

        //рассчитываем положение точек
        lowerRight = pos + tr.right * width - tr.up * height + tr.forward * distance;
        lowerLeft = pos - tr.right * width - tr.up * height + tr.forward * distance;
        upperRight = pos + tr.right * width + tr.up * height + tr.forward * distance;
        upperLeft = pos - tr.right * width + tr.up * height + tr.forward * distance;
    }
}

//Новый вспомогательный статический класс
internal static class Helper
{
    //метод для ограничения угла поворота
    public static float ClampAngle(float angle, float min, float max)
    {
        //Делаем
        do
        {
            //Если угол меньше -360 прибавляем к нему 360
            if (angle < -360) angle += 360;
            //Если больше 360 - вычитем
            if (angle > 360) angle -= 360;
            //Пока он не окажется в диапазоне [-360;360]
        } while (angle < -360 || angle > 360);
        //Возвращаем ограничив сверху и снизу
        return Mathf.Clamp(angle, min, max);
    }
}