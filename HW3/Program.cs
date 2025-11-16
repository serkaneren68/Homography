using ArHw3;
using MathNet.Numerics.LinearAlgebra;
using static System.Net.Mime.MediaTypeNames;

namespace ArHw3;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== PROBLEM 1.1 — Homography Estimation (Matched Points) ===\n");

        // Örnek scene (marker üzerindeki gerçek dünya) noktaları
        var scenePoints = new List<Vec2>
        {
            new Vec2(0, 0),
            new Vec2(7, 0),
            new Vec2(7, 7),
            new Vec2(0, 7)
        };

        // Örnek image (kameradaki piksel koordinatları)
        var imagePoints = new List<Vec2>
        {
            new Vec2(450, 300),
            new Vec2(900, 320),
            new Vec2(880, 900),
            new Vec2(430, 880)
        };

        // 1.1 — Homografiyi hesapla (DLT + LM refine)
        var H = HomographyEstimator.EstimateHomographyMatched(scenePoints, imagePoints);

        Console.WriteLine("Estimated Homography Matrix H:");
        Console.WriteLine(H.ToMatrixString());
        Console.WriteLine();


        Console.WriteLine("=== PROBLEM 1.3 — Project a Scene Point Using H ===\n");

        // Projeksiyon için bir örnek scene noktası
        var testScenePoint = new Vec2(3.5, 3.5);

        // 1.3 — Noktanın projeksiyonu
        var projected = HomographyUtils.Project(H, testScenePoint);

        Console.WriteLine($"Scene Point: ({testScenePoint.X:F3}, {testScenePoint.Y:F3})");
        Console.WriteLine($"Projected Image Point: ({projected.X:F3}, {projected.Y:F3})");


        Console.WriteLine("\n=== PROBLEM 1.4 — Project an Image Point Back to the Scene ===\n");

        // Örnek image noktası
        var testImagePoint = new Vec2(700, 600);

        // 1.4 – Image → Scene projeksiyonu
        var backProjected = HomographyUtils.ProjectImagePointToScene(testImagePoint, H);

        Console.WriteLine($"Image Point: ({testImagePoint.X:F3}, {testImagePoint.Y:F3})");
        Console.WriteLine($"Back-Projected Scene Point: ({backProjected.X:F3}, {backProjected.Y:F3})");

        // ------------------------------------------------------------------
        // PROBLEM 1.5 – Üç görüntü için değerlendirme
        // ------------------------------------------------------------------
        Console.WriteLine("\n\n=== PROBLEM 1.5 — Three Images Evaluation ===");

        //        Img 0,0 300,200 800,400 500,500 700,300
        //Image 1 472,514 928,1222    1377,2377   1614,1688   1152,2146
        //Image 2 283,506 728,1203    1182,2404   1434,1685   949, 2155
        //Image 3 492,595 974,1375    1360,2441   1609,1827   1161,2242

        //        Img 0,0 100,100 300,300 500,500 700,700
        //Image 1 472,514 703,753 1162,1224   1614,1688   2066,2152
        //Image 2 283,506 507,735 961, 1201   1434,1685   2935,2187
        //Image 3 492,595 744,875 1206,1381   1609,1827   1920,2231




        // TODO: BURAYA GERÇEK NOKTALARINI KOYACAKSIN
        // Image 1 için:
        var sceneTrainImg = new List<Vec2>
        {
            new Vec2(0, 0),
            new Vec2(300, 200),
            new Vec2(800, 400),
            new Vec2(700, 300)
        };

        var imageTrainImg1 = new List<Vec2>
        {
            new Vec2(472, 514),
            new Vec2(928,1222),
            new Vec2(1377,2377),
            new Vec2(1152,2146)
        };

        var sceneTestImg = new List<Vec2>
        {
            new Vec2(500, 500)
        };

        var imageTestImg1 = new List<Vec2>
        {
            new Vec2(1614, 1688)
        };

        if (sceneTrainImg.Count > 0)
        {
            var H1 = HomographyEvaluation.EvaluateForImage(
                "Image 1",
                sceneTrainImg, imageTrainImg1,
                sceneTestImg, imageTestImg1);
        }
        else
        {
            Console.WriteLine("Image 1 için henüz noktalar doldurulmadı.");
        }

        // Image 2 için aynı yapıyı kopyalayıp doldur:
        //Image 2 283,506 728,1203    1182,2404   1434,1685   949, 2155

        var imageTrainImg2 = new List<Vec2>
        {
            new Vec2(283, 506),
            new Vec2(728,1203),
            new Vec2(1182,2404),
            new Vec2(949, 2155)

        };
        var imageTestImg2 = new List<Vec2>
        {
            new Vec2(1434, 1685)
        };

        if (sceneTrainImg.Count > 0)
        {
            var H2 = HomographyEvaluation.EvaluateForImage(
                "Image 2",
                sceneTrainImg, imageTrainImg2,
                sceneTestImg, imageTestImg2);
        }
        else
        {
            Console.WriteLine("Image 2 için henüz noktalar doldurulmadı.");
        }

        //Image 3 492,595 974,1375    1360,2441   1609,1827   1161,2242
        var imageTrainImg3 = new List<Vec2>
        {
            new Vec2(492, 595),
            new Vec2(974,1375),
            new Vec2(1360,2441),
            new Vec2(1161,2242) 

        };
        var imageTestImg3 = new List<Vec2>
        {
            new Vec2(1609, 1827)
        };

        if (sceneTrainImg.Count > 0)
        {
            var H3 = HomographyEvaluation.EvaluateForImage(
                "Image 3",
                sceneTrainImg, imageTrainImg3,
                sceneTestImg, imageTestImg3);
        }
        else
        {
            Console.WriteLine("Image 3 için henüz noktalar doldurulmadı.");
        }

        Console.WriteLine("\n--- All done (for now). ---");
        Console.ReadLine();
        Console.WriteLine("\n--- Demo Completed ---");
    }
}
