namespace Clothes.Extension
{
    public static class BoolExtension
    {
        public static int ToInt(this bool v)
        {
            return v ? 1 : 0;
        }
    }
}