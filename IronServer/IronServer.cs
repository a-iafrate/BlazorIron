using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using NetCoreAudio;

namespace IronServer
{
    internal class IronServer
    {
        private const string SpeechSubscriptionKey = "X";
        private const string SpeechRegion = "westeurope";
        private const string Culture = "it-IT";
        SpeechRecognizer cognitiveRecognizer;

#if DEBUG
        private const string ServerApi = "http://localhost:5158/";
#else
        private const string ServerApi = "http://localhost:5000/";
#endif

        public void Init()
        {
            var config = SpeechConfig.FromSubscription(SpeechSubscriptionKey, SpeechRegion);
            cognitiveRecognizer = new SpeechRecognizer(config, Culture, AudioConfig.FromDefaultMicrophoneInput());
        }


        public async Task<bool> Start()
        {
            Init();
            var player = new Player();
            player.Play("mp3/init.mp3");

            
            bool exit = false;
            while (!exit)
            {
                Console.Write("Say something: ");

                // Starts recognition. It returns when the first utterance has been  recognized.
                var result = await cognitiveRecognizer.RecognizeOnceAsync();

                // Checks result.
                string line="";
                switch (result.Reason)
                {
                    case ResultReason.RecognizedSpeech:
                        line = result.Text;
                        break;
                    case ResultReason.Canceled:

                        //Try re-init for solve problems
                        Init();
                        break;
                    default:
                        line = result.Reason.ToString();
                        break;

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
            return true;
        }

        public void CallApi(string api)
        {

            using (var httpClient = new HttpClient())
            {
                using (var response = httpClient.GetAsync(ServerApi + api).Result)
                {

                }
            }

        }



        public string CleanString(string line)
        {
            return line.ToLower().Replace(".", "").Replace("?", "");
        }
    }
}
