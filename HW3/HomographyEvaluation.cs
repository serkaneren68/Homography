using MathNet.Numerics.LinearAlgebra;

namespace ArHw3;

public static class HomographyEvaluation
{
    /// <summary>
    /// Bir görüntü için:
    /// - Eğitim eşleşmeleriyle H hesaplar
    /// - Test eşleşmelerinde projeksiyon hatasını ölçer
    /// </summary>
    public static Matrix<double> EvaluateForImage(
        string imageName,
        IList<Vec2> sceneTrain,   // en az 5 nokta
        IList<Vec2> imageTrain,
        IList<Vec2> sceneTest,    // hata hesabı için 3 nokta
        IList<Vec2> imageTest)
    {
        Console.WriteLine($"\n=== Evaluation for {imageName} ===");

        if (sceneTrain.Count != imageTrain.Count)
            throw new ArgumentException("Train listeleri aynı uzunlukta olmalı.");
        if (sceneTrain.Count < 5)
            Console.WriteLine("Uyarı: En az 5 train noktası önerilir.");

        if (sceneTest.Count != imageTest.Count)
            throw new ArgumentException("Test listeleri aynı uzunlukta olmalı.");

        // 1) 1.1'deki fonksiyonla H'yi hesapla
        var H = HomographyEstimator.EstimateHomographyMatched(sceneTrain, imageTrain);
        Console.WriteLine("\nHomography H:");
        Console.WriteLine(H.ToMatrixString());

        // 2) Test noktalarındaki hatayı hesapla
        double totalError = 0.0;

        Console.WriteLine("\nTest point errors:");
        Console.WriteLine("Idx | Scene(x,y)        | Image(u,v)        | Projected(û,v̂)    | Error(px)");
        Console.WriteLine("----+-------------------+-------------------+---------------------+----------");

        for (int i = 0; i < sceneTest.Count; i++)
        {
            var s = sceneTest[i];
            var iTrue = imageTest[i];

            var iProj = HomographyUtils.Project(H, s);

            double du = iTrue.X - iProj.X;
            double dv = iTrue.Y - iProj.Y;
            double err = Math.Sqrt(du * du + dv * dv);
            totalError += err;

            Console.WriteLine(
                $"{i,3} | ({s.X,6:F2}, {s.Y,6:F2}) | ({iTrue.X,6:F1}, {iTrue.Y,6:F1}) | ({iProj.X,7:F2}, {iProj.Y,7:F2}) | {err,8:F3}");
        }

        double meanError = totalError / sceneTest.Count;
        Console.WriteLine($"\nAverage test error: {meanError:F3} pixels");

        return H;
    }
}
