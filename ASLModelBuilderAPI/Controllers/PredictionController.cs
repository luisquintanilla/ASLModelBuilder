using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ASLModelBuilderML.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;

namespace ASLModelBuilderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PredictionController : ControllerBase
    {
        private readonly PredictionEngine<ModelInput, ModelOutput> _predictionEngine;
        private readonly object _predictionEngineLock = new object();

        public PredictionController(PredictionEngine<ModelInput, ModelOutput> predictionEngine)
        {
            _predictionEngine = predictionEngine;
        }

        [HttpPost]
        public async Task<ModelOutput> ClassifyImage([FromBody] Dictionary<string, string> input)
        {
            ModelOutput prediction;
            
            // Get raw image bytes
            var imageBytes = Convert.FromBase64String(input["data"]);

            string imagePath = Path.Join(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), $"{imageBytes.GetHashCode()}.jpeg");

            using (var ms = new MemoryStream(imageBytes))
            {
                // Save the image to a file
                using (var img = await Task.Run(() => Image.FromStream(ms)))
                    await Task.Run(() => img.Save(imagePath));
            }

            lock (_predictionEngineLock)
            {
                // Use Prediction to classify image
                prediction = _predictionEngine.Predict(new ModelInput { ImageSource = imagePath });
            }

            return prediction;
        }
    }
}