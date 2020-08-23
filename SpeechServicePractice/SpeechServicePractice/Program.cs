using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.Media;
using System.Threading.Tasks;


namespace SpeechServicePractice
{
    class Program
    {
        private const string serviceKey1 = "6700cf7c7e2c457fa3078a096cf10f8e";
        private const string serviceKey2 = "f90abad758ad435a8649f09d937927a9";
        private readonly static string[] files = { "DoYouHaveSomeTime.wav", "WhatAreYouDoing.wav", "MiMiMi.wav", "SoTheySay.wav" };
        private readonly static string[][] replies = { new string[]{"Sure! I alway have time for you.","Oh! sorry, I'm going to take a bath, talk later." },
                                                       new string[]{"Thinking of you.","Not your business, get out of my way." },
                                                       new string[]{"Mi mi mi mi mi mi mi Mi mi mi sexy mi","What?" },
                                                       new string[]{ "Dance for me, dance for me, dance for me, oh-oh-oh", "Say what? Shut up?" }
                                                      };
        static async Task Main()
        {
            var config = SpeechConfig.FromSubscription(serviceKey1, "eastasia");
            //await RecognizeSpeechAsync();
            PrintTitle();
            while (true)
            {
                string filename = "";
                int selectedFileIndex = -1;
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        filename = files[0];
                        selectedFileIndex = 0;
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        filename = files[1];
                        selectedFileIndex = 1;
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        filename = files[2];
                        selectedFileIndex = 2;
                        break;
                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        filename = files[3];
                        selectedFileIndex = 3;
                        break;
                    case ConsoleKey.Q:
                        filename = "Q";
                        break;
                    default:
                        Console.WriteLine("");
                        PrintTitle();
                        break;
                }
                Console.WriteLine("");
                if (filename.Equals("Q"))
                {
                    break;
                }
                else if (filename.Equals(""))
                {
                    continue;
                }
                else
                {
                    filename = "..\\..\\..\\inputFiles\\" + filename;
                    PlayAudioFile(filename);
                    try
                    {
                        await RecognizeSpeechAsync(config, filename);
                        string respond = replies[selectedFileIndex][(new Random().Next()) % 2];
                        Console.WriteLine("System said : " + respond);
                        await SynthesizeAudioAsync(config, respond);
                        PrintTitle();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Try again?");
                    }
                }
            }
            Console.WriteLine("Bye");
            //await SynthesizeAudioAsync(config, "So, they say");
        }
        private static void PrintTitle()
        {
            Console.WriteLine("1 : Do you have some time?");
            Console.WriteLine("2 : What are you doing?");
            Console.WriteLine("3 : Mi Mi Mi~");
            Console.WriteLine("4 : So they say");
            Console.WriteLine("Q : Quit");
        }

        static async Task RecognizeSpeechAsync(SpeechConfig config, string wavFileName)
        {
            using (var audioInput = AudioConfig.FromWavFileInput(wavFileName))
            {
                using (SpeechRecognizer recognizer = new SpeechRecognizer(config, audioInput))
                {
                    Console.WriteLine("Waiting for result...");
                    var result = await recognizer.RecognizeOnceAsync();
                    switch (result.Reason)
                    {
                        case ResultReason.RecognizedSpeech:
                            Console.WriteLine($"We recognized: {result.Text}");
                            break;
                        case ResultReason.NoMatch:
                            Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                            throw new Exception();
                        case ResultReason.Canceled:
                            var cancellation = CancellationDetails.FromResult(result);
                            Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                            if (cancellation.Reason == CancellationReason.Error)
                            {
                                Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                                //Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                                //Console.WriteLine($"CANCELED: Did you update the subscription info?");
                            }
                            throw new Exception();
                    }
                }
            }
        }
        static void PlayAudioFile(string fileName)
        {
            SoundPlayer player = new SoundPlayer(fileName);
            player.LoadAsync();
            player.PlaySync();
        }
        static async Task SynthesizeAudioAsync(SpeechConfig config, string toSpeechText)
        {
            // using var audioConfig = AudioConfig.FromWavFileOutput("D:/GitHub/C#/Speech practice/SpeechServicePractice/inputFiles/SoTheySay.wav");
            using var synthesizer = new SpeechSynthesizer(config/*, audioConfig*/);
            await synthesizer.SpeakTextAsync(toSpeechText);
        }


    }
}
