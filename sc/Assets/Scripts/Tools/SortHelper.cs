class SortHelper
{
    //条件委托  给a,b两个对象, 返回a基于自定义条件是否大于b   
    public delegate bool Condition<T>(T a, T b);

    //冒泡排序  第二个参数使用上面的委托将自定义比较条件推迟给调用者定义
    public static void Sort<T>(T[] array, Condition<T> condition)
    {
        for (int j = 0; j < array.Length - 1; j++)
        {
            for (int i = 0; i < array.Length - 1 - j; i++)
            {
                if (condition(array[i], array[i + 1]))
                {
                    T temp = array[i];
                    array[i] = array[i + 1];
                    array[i + 1] = temp;
                }
            }
        }
    }
}
