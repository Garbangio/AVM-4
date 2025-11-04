using System;

class Program
{
    static void Main()
    {
        double[,] A = {
            { 5, 1, 1 },
            { 1, 4, 1 },
            { 1, 1, 6 }
        };
        double[] b = { 7, 5, 11 };

        Console.WriteLine("Решение системы уравнений:");
        Console.WriteLine("5x1 + x2 + x3 = 7");
        Console.WriteLine("x1 + 4x2 + x3 = 5");
        Console.WriteLine("x1 + x2 + 6x3 = 11");
        Console.WriteLine();

        // Метод Якоби
        double[] xJacobi = Jac(A, b);
        Console.WriteLine("Метод Якоби:");
        Console.WriteLine($"x1 = {xJacobi[0]:F6}");
        Console.WriteLine($"x2 = {xJacobi[1]:F6}");
        Console.WriteLine($"x3 = {xJacobi[2]:F6}");
        Console.WriteLine();

        // Метод Зейделя
        double[] xSeidel = Sei(A, b);
        Console.WriteLine("Метод Зейделя:");
        Console.WriteLine($"x1 = {xSeidel[0]:F6}");
        Console.WriteLine($"x2 = {xSeidel[1]:F6}");
        Console.WriteLine($"x3 = {xSeidel[2]:F6}");
    }

    static double[] Jac(double[,] A, double[] b)
    {
        int n = b.Length;
        double[] x = new double[n];
        double[] xNew = new double[n];

        for (int i = 0; i < n; i++)
            x[i] = 0;

        int iteration = 0;
        do
        {
            for (int i = 0; i < n; i++)
            {
                double sum = 0;
                for (int j = 0; j < n; j++)
                    if (j != i)
                        sum += A[i, j] * x[j];
                xNew[i] = (b[i] - sum) / A[i, i];
            }
            Array.Copy(xNew, x, n);
            iteration++;
        } while (!Checkx(A, b, x));

        Console.WriteLine($"Сошлось за {iteration} итераций");
        return x;
    }

    static double[] Sei(double[,] A, double[] b)
    {
        int n = b.Length;
        double[] x = new double[n];
        for (int i = 0; i < n; i++)
            x[i] = 0;

        int iteration = 0;
        do
        {
            for (int i = 0; i < n; i++)
            {
                double sum = 0;
                for (int j = 0; j < n; j++)
                    if (j != i)
                        sum += A[i, j] * x[j];
                x[i] = (b[i] - sum) / A[i, i];
            }
            iteration++;
        } while (!Checkx(A, b, x));

        Console.WriteLine($"Сошлось за {iteration} итераций");
        return x;
    }

    static bool Checkx(double[,] matrix, double[] vector, double[] x, double epsilon = 1e-10)
    {
        int n = vector.Length;
        for (int i = 0; i < n; i++)
        {
            double sum = 0;
            for (int j = 0; j < n; j++)
            {
                sum += matrix[i, j] * x[j];
            }
            if (Math.Abs(sum - vector[i]) > epsilon)
            {
                return false;
            }
        }
        return true;
    }
}