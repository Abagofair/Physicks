namespace Physicks.MathHelpers;

public class MatMN
{
    public MatMN(int rows, int columns)
    {
        M = rows;
        N = columns;
        Rows = new VecN[rows];
        Zero();
    }

    public MatMN(MatMN mat) : this(mat.M, mat.M)
    {
        for (int i = 0; i < mat.M; i++)
        {
            Rows[i] = mat.Rows[i];
        }
    }

    public int M { get; init; }
    public int N { get; init; }
    public VecN[] Rows { get; init; }

    public void Zero()
    {
        for (int i = 0; i < M; i++)
        {
            Rows[i] = new VecN(N);
            Rows[i].Zero();
        }
    }

    public MatMN Transpose()
    {
        MatMN result = new(N, M);

        for (int i = 0; i < M; i++)
        {
            for (int j = 0; j < N; j++)
            {
                result.Rows[j][i] = Rows[i][j];
            }
        }
        return result;
    }

    public static VecN operator* (MatMN mat, VecN vec)
    {
        if (vec.Data.Length != mat.N)
            throw new ArgumentException(nameof(mat));

        VecN result = new(mat.M);
        for (int i = 0; i < mat.M; i++)
        {
            result[i] = vec.Dot(mat.Rows[i]);
        }

        return result;
    }

    public static MatMN operator* (MatMN a, MatMN b)
    {
        if (a.N != b.M && a.M != b.N) 
            throw new ArgumentException(nameof(a));

        MatMN transposed = b.Transpose();
        MatMN result = new(a.M, b.N);
        for (int i = 0; i < a.M; i++)
        {
            for (int j = 0; j < b.N; j++)
            {
                result.Rows[i][j] = a.Rows[i].Dot(transposed.Rows[j]);
            }
        }
        return result;
    }

    public static VecN SolveGaussSeidel(MatMN a, VecN b)
    {
        int N = b.Data.Length;
        VecN X = new VecN(N);
        X.Zero();

        for (int iterations = 0; iterations < N; iterations++)
        {
            for (int i = 0; i < N; i++)
            {
                if (a.Rows[i][i] != 0.0f)
                {
                    float dx = (b[i] / a.Rows[i][i]) - (a.Rows[i].Dot(X) / a.Rows[i][i]);
                    if (!float.IsNaN(dx))
                        X[i] += dx;
                }
            }
        }

        return X;
    }
}