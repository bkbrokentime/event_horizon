public static class Generic
{
	public static void Swap<T>(ref T first, ref T second)
	{
		T temp = first;
		first = second;
		second = temp;
	}
    public static float[] ArrayMaxValue(float[] array, int num)
    {
        float[] result = new float[num];
        int count = array.Length;
        for (int i=0;i<num;i++)
        {
            ArrayAfterSwapMaxToSetPoint(array, i);
            result[i] = array[i];
        }
        return result;
    }
    public static void ArrayAfterSwapMaxToSetPoint(float[] array, int num)
    {
        int count = array.Length;
        if (num >= count - 1)
            return;
        for (int i = num + 1; i < count; i++)
        {
            if (array[num] < array[i])
                Swap(ref array[num], ref array[num + 1]);
        }
    }

    public static float DefaultForNaN(float value, float defaultvalue)
    {
        if (float.IsNaN(value))
            return defaultvalue;
        return value;
    }

}