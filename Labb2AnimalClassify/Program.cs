using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using System;

namespace Labb2AnimalClassify;

internal class Program
{
    private static string predictionKey = "1d1a3b757b0f42918d98dfc6c811c2b8";
    private static string endpoint = "https://customvisionanimal-prediction.cognitiveservices.azure.com/";
    private static Guid projectId = new Guid("b6817f19-36d8-4251-853d-114559202bce");
    private static string modelName = "ClassifyAnimal";

    static async Task Main(string[] args)
    {
        var client = new CustomVisionPredictionClient(new ApiKeyServiceClientCredentials(predictionKey))
        {
            Endpoint = endpoint
        };

        while (true)
        {
            Console.WriteLine("Ange URL eller filsökväg till bilden (eller skriv 'avsluta' för att avsluta):");
            string input = Console.ReadLine().Trim('"');

            if (input.Equals("avsluta", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            ImagePrediction result;

            if (Uri.IsWellFormedUriString(input, UriKind.Absolute))
            {
                // URL hantering
                result = await client.ClassifyImageUrlAsync(projectId, modelName, new ImageUrl(input));
            }
            else if (File.Exists(input))
            {
                // Filsökväg hantering
                try
                {
                    using (var imageStream = new FileStream(input, FileMode.Open, FileAccess.Read))
                    {
                        result = await client.ClassifyImageAsync(projectId, modelName, imageStream);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ett fel uppstod vid filhantering: {ex.Message}");
                    continue;
                }
            }
            else
            {
                Console.WriteLine("Ogiltig URL eller filsökväg.");
                continue;
            }

            // Visa resultatet
            foreach (var prediction in result.Predictions)
            {
                Console.WriteLine($"Tagg: {prediction.TagName}, Sannolikhet: {prediction.Probability:P1}");
            }

            Console.WriteLine("Vill du köra en ny klassificering? (ja/nej)");
            string response = Console.ReadLine().Trim().ToLower();

            if (response != "ja")
            {
                break;
            }
        }

        Console.WriteLine("Programmet avslutas!");
    }
}
