namespace Physicks.Math;

public class VecN
{
    public VecN(int n)
    {
        Data = new float[n];
        Zero();
    }

    public VecN(VecN vec) : this(vec.Data.Length)
    {
        for (int i = 0; i < vec.Data.Length; i++)
        {
            Data[i] = vec.Data[i];
        }
    }

    public float[] Data { get; init; }

    public void Zero()
    {
        for (int i = 0; i < Data.Length; i++)
        {
            Data[i] = 0.0f;
        }
    }

    public float Dot(VecN v)
    {
        Assert(this, v);

        float dot = 0.0f;
        for (int i = 0; i < v.Data.Length; i++)
        {
            var a = Data[i];
            var b = v.Data[i];

            dot += a * b;
        }

        return dot;
    }

    public static VecN operator +(VecN a, VecN b)
    {
        Assert(a, b);

        for (int i = 0; i < a.Data.Length; i++)
        {
            a.Data[i] = a.Data[i] + b.Data[i];
        }

        return a;
    }

    public static VecN operator -(VecN a, VecN b)
    {
        Assert(a, b);

        for (int i = 0; i < a.Data.Length; i++)
        {
            a.Data[i] = a.Data[i] - b.Data[i];
        }

        return a;
    }

    public static VecN operator *(VecN a, VecN b)
    {
        for (int i = 0; i < a.Data.Length; i++)
        {
            a.Data[i] = a.Data[i] * b.Data[i];
        }

        return a;
    }

    public static VecN operator *(VecN a, float scalar)
    {
        for (int i = 0; i < a.Data.Length; i++)
        {
            a.Data[i] = a.Data[i] * scalar;
        }

        return a;
    }

    public float this[int i]
    {
        get => Data[i];
        set => Data[i] = value;
    }

    private void Assert(VecN v)
    {
        if (v == null) throw new ArgumentNullException(nameof(v));
        if (Data.Length != v.Data.Length) throw new ArgumentException("Expected similar length");
    }

    private static void Assert(VecN a, VecN b)
    {
        if (a == null) throw new ArgumentNullException(nameof(a));
        if (b == null) throw new ArgumentNullException(nameof(b));
        if (a.Data.Length != b.Data.Length) throw new ArgumentException("Expected similar length");
    }
}