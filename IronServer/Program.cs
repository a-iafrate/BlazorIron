using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using NetCoreAudio;



namespace IronServer // Note: actual namespace depends on the project name.
{
    public class Program
    {
        private const string SpeechSubscriptionKey = "XXXXXXX";
        private const string SpeechRegion = "westeurope";
        private const string Culture = "it-IT";

#if DEBUG
        private const string ServerApi = "http://localhost:5158/";
#else
        private const string ServerApi = "http://localhost:5000/";
#endif



        private static async Task Main(string[] args)
        {
            var player = new Player();
            player.Play("mp3/init.mp3");





            var config = SpeechConfig.FromSubscription(SpeechSubscriptionKey, SpeechRegion);
            using var cognitiveRecognizer = new SpeechRecognizer(config, Culture, AudioConfig.FromDefaultMicrophoneInput());
            bool exit = false;
            while (!exit)
            {
                Console.Write("Say something: ");

                // Starts recognition. It returns when the first utterance has been  recognized.
                var result = await cognitiveRecognizer.RecognizeOnceAsync();

                // Checks result.
                string line;
                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    line = result.Text;
                }
                else
                {
                    line = result.Reason.ToString();
                }

                line = CleanString(line);
                Console.WriteLine(line);
               
                switch (line)
                {
                    case "accendi led":
                        player.Play("mp3/ok.mp3");
                        CallApi("ironman/ledon");
                        break;
                    case "spegni led":
                        player.Play("mp3/ok.mp3");
                        CallApi("ironman/ledoff");
                        break;
                    case "accendi occhi":
                        player.Play("mp3/ok.mp3");
                        CallApi("ironman/eyeson");
                        break;
                    case "spegni occhi":
                        player.Play("mp3/ok.mp3");
                        CallApi("ironman/eyesoff");
                        break;
                    case "apri":
                        player.Play("mp3/ok.mp3");
                        CallApi("ironman/openface");
                        break;
                    case "chiudi":
                        player.Play("mp3/ok.mp3");
                        CallApi("ironman/closeface");
                        break;
                    case "esci":
                        player.Play("mp3/close.mp3");
                        exit = true;
                        break;
                }
            }
        }

        public static void CallApi(string api)
        {
            
            using (var httpClient = new HttpClient())
            {
                using (var response = httpClient.GetAsync(ServerApi + api).Result)
                {
                    
                }
            }
           
        }



        public static string CleanString(string line)
        {
            return line.ToLower().Replace(".", "").Replace("?", "");
        }
    }
}