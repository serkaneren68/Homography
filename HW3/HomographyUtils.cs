using MathNet.Numerics.LinearAlgebra;

namespace ArHw3;

public static class HomographyUtils
{
    // H (3x3) -> p = [a,b,c,d,e,f,g,h]^T
    public static Vector<double> HToParams(Matrix<double> H)
    {
        var p = Vector<double>.Build.Dense(8);
        p[0] = H[0, 0]; // a
        p[1] = H[0, 1]; // b
        p[2] = H[0, 2]; // c
        p[3] = H[1, 0]; // d
        p[4] = H[1, 1]; // e
        p[5] = H[1, 2]; // f
        p[6] = H[2, 0]; // g
        p[7] = H[2, 1]; // h
        return p;
    }

    // p -> H (3x3), son eleman sabit 1
    public static Matrix<double> ParamsToH(Vector<double> p)
    {
        var H = Matrix<double>.Build.Dense(3, 3);
        H[0, 0] = p[0];
        H[0, 1] = p[1];
        H[0, 2] = p[2];
        H[1, 0] = p[3];
        H[1, 1] = p[4];
        H[1, 2] = p[5];
        H[2, 0] = p[6];
        H[2, 1] = p[7];
        H[2, 2] = 1.0;
        return H;
    }

    // Scene noktası (x, y) → Image (u, v)
    public static Vec2 Project(Matrix<double> H, Vec2 s)
    {
        var v = Vector<double>.Build.DenseOfArray(new[] { s.X, s.Y, 1.0 });
        var r = H * v;
        double u = r[0] / r[2];
        double v2 = r[1] / r[2];
        return new Vec2(u, v2);
    }

    public static Vec2 ProjectImagePointToScene(
    Vec2 imagePoint,
    Matrix<double> H)
    {
        // Homografinin tersi
        var Hinv = H.Inverse();

        double u = imagePoint.X;
        double v = imagePoint.Y;

        var img = Vector<double>.Build.DenseOfArray(new[] { u, v, 1.0 });
        var proj = Hinv * img;

        double x = proj[0] / proj[2];
        double y = proj[1] / proj[2];

        return new Vec2(x, y);
    }
}
