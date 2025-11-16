using MathNet.Numerics.LinearAlgebra;

namespace ArHw3;

public static partial class HomographyEstimator
{
    // 1.1 – matched noktalar için ana fonksiyon
    public static Matrix<double> EstimateHomographyMatched(
        IList<Vec2> scenePts,
        IList<Vec2> imagePts,
        int lmIters = 50)
    {
        if (scenePts.Count != imagePts.Count)
            throw new ArgumentException("scenePts ve imagePts aynı sayıda olmalı.");
        if (scenePts.Count < 4)
            throw new ArgumentException("En az 4 nokta gerekli.");

        // 1) DLT ile başlangıç H
        var H0 = ComputeHomographyLinear(scenePts, imagePts);

        // 2) LM ile refine edeceğiz (birazdan yazacağız)
        var H = RefineHomographyLM(H0, scenePts, imagePts, lmIters);

        return H;
    }

    // --- DLT / lineer başlangıç ---
    private static Matrix<double> ComputeHomographyLinear(
        IList<Vec2> scenePts,
        IList<Vec2> imagePts)
    {
        int n = scenePts.Count;
        int rows = 2 * n;
        int cols = 8;

        var A = Matrix<double>.Build.Dense(rows, cols);
        var b = Vector<double>.Build.Dense(rows);

        for (int i = 0; i < n; i++)
        {
            double x = scenePts[i].X;
            double y = scenePts[i].Y;
            double u = imagePts[i].X;
            double v = imagePts[i].Y;

            int r1 = 2 * i;
            int r2 = 2 * i + 1;

            // u denklemi
            // [xi​,yi​,1,0,0,0,−ui​xi​,−ui​yi​]⋅p=ui​
            // [0,0,0,xi​,yi​,1,−vi​xi​,−vi​yi​]⋅p=vi​
            A[r1, 0] = x;
            A[r1, 1] = y;
            A[r1, 2] = 1.0;
            A[r1, 3] = 0.0;
            A[r1, 4] = 0.0;
            A[r1, 5] = 0.0;
            A[r1, 6] = -u * x;
            A[r1, 7] = -u * y;
            b[r1] = u;

            // v denklemi
            A[r2, 0] = 0.0;
            A[r2, 1] = 0.0;
            A[r2, 2] = 0.0;
            A[r2, 3] = x;
            A[r2, 4] = y;
            A[r2, 5] = 1.0;
            A[r2, 6] = -v * x;
            A[r2, 7] = -v * y;
            b[r2] = v;
        }

        // Least squares çözümü: A p ≈ b
        var qr = A.QR();
        Vector<double> p = qr.Solve(b); // p(0)

        return HomographyUtils.ParamsToH(p);
    }
}

public static partial class HomographyEstimator
{
    // p: [a..h]
    private static Vector<double> ComputeResidualVector(
        Vector<double> p,
        IList<Vec2> scenePts,
        IList<Vec2> imagePts)
    {
        var H = HomographyUtils.ParamsToH(p);
        int n = scenePts.Count;
        var r = Vector<double>.Build.Dense(2 * n);

        for (int i = 0; i < n; i++)
        {
            double x = scenePts[i].X;
            double y = scenePts[i].Y;
            double u = imagePts[i].X;
            double v = imagePts[i].Y;

            var vScene = Vector<double>.Build.DenseOfArray(new[] { x, y, 1.0 });
            var proj = H * vScene;
            double uHat = proj[0] / proj[2];
            double vHat = proj[1] / proj[2];

            r[2 * i] = u - uHat;
            r[2 * i + 1] = v - vHat;
        }

        return r;
    }

    private static double ComputeTotalError(
        Vector<double> p,
        IList<Vec2> scenePts,
        IList<Vec2> imagePts)
    {
        var r = ComputeResidualVector(p, scenePts, imagePts);
        return r.DotProduct(r); // ∑ r_i^2
    }

    private static Matrix<double> ComputeJacobianFiniteDiff(
        Vector<double> p,
        IList<Vec2> scenePts,
        IList<Vec2> imagePts,
        double eps = 1e-6)
    {
        int n = scenePts.Count;
        int m = p.Count;       // 8
        int dim = 2 * n;

        var r0 = ComputeResidualVector(p, scenePts, imagePts);
        var J = Matrix<double>.Build.Dense(dim, m);

        for (int k = 0; k < m; k++)
        {
            var pPert = p.Clone();
            pPert[k] += eps;

            var rPert = ComputeResidualVector(pPert, scenePts, imagePts);

            for (int i = 0; i < dim; i++)
            {
                J[i, k] = (rPert[i] - r0[i]) / eps;
            }
        }

        return J;
    }
}
public static partial class HomographyEstimator
{
    private static Matrix<double> RefineHomographyLM(
        Matrix<double> H0,
        IList<Vec2> scenePts,
        IList<Vec2> imagePts,
        int maxIters = 50)
    {
        var p = HomographyUtils.HToParams(H0); // başlangıç p(0)

        double lambda = 1e-3;
        double prevError = ComputeTotalError(p, scenePts, imagePts);

        for (int iter = 0; iter < maxIters; iter++)
        {
            var r = ComputeResidualVector(p, scenePts, imagePts);
            var J = ComputeJacobianFiniteDiff(p, scenePts, imagePts);

            var JTJ = J.TransposeThisAndMultiply(J);   // 8x8
            var JTr = J.TransposeThisAndMultiply(r);   // 8

            // (JTJ + λ diag(JTJ)) Δp = -JTr
            var A = JTJ.Clone();
            for (int i = 0; i < A.RowCount; i++)
                A[i, i] += lambda * JTJ[i, i];

            var rhs = -JTr;

            Vector<double> dp;
            try
            {
                dp = A.Solve(rhs);
            }
            catch
            {
                // Sistem çözülemez hale gelirse çık
                break;
            }

            var pNew = p + dp;
            double newError = ComputeTotalError(pNew, scenePts, imagePts);

            if (newError < prevError)
            {
                // İyileşme: güncellemeyi kabul et, lambda'yı küçült
                if (Math.Abs(prevError - newError) < 1e-10)
                {
                    p = pNew;
                    break;
                }

                p = pNew;
                prevError = newError;
                lambda *= 0.1;
            }
            else
            {
                // Kötüleşme: adımı küçült, lambda'yı büyüt
                lambda *= 10.0;
            }
        }

        return HomographyUtils.ParamsToH(p);
    }
}