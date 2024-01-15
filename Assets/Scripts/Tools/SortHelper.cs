class SortHelper
{
    //����ί��  ��a,b��������, ����a�����Զ��������Ƿ����b   
    public delegate bool Condition<T>(T a, T b);

    //ð������  �ڶ�������ʹ�������ί�н��Զ���Ƚ������Ƴٸ������߶���
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
