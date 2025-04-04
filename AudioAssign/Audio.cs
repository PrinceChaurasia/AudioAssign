using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using System.Timers;
using System.IO;

namespace Assignment {
    public class Audio { 
        private WaveInEvent wavein; 
        private WaveFileWriter wavefile;
        private string outputDirectory; 
        private MemoryStream audioBuffer;
        private System.Timers.Timer silenceTimer;
        private DateTime lastvoiceDetected;
        private bool isRecording;
        public Audio(string outputDir) {
            outputDirectory = outputDir; Directory.CreateDirectory(outputDir); 
            wavein = new WaveInEvent { WaveFormat = new WaveFormat(44100, 1) };
            wavein.DataAvailable += OnDataAvailable;
            wavein.RecordingStopped += OnRecordingStopped;
            silenceTimer = new System.Timers.Timer(1000); 
            silenceTimer.Elapsed += CheckForSilence;
            silenceTimer.Start();
        } 
        public void Start() 
        { 
            wavein.StartRecording();
            Console.WriteLine("Started monitoring microphone...");
        }
        private void OnDataAvailable(object sender, WaveInEventArgs e) 
        { 
            float maxVolume = GetMaxVolume(e.Buffer, e.BytesRecorded);
            if (maxVolume > 0.02f)
            {
                lastvoiceDetected = DateTime.Now;
                if (!isRecording)
                { StartRecording();
                }
            }
            if (isRecording)
            { wavefile.Write(e.Buffer, 0, e.BytesRecorded);
                wavefile.Flush();
            } 
        }
        private void StartRecording() 
        { 
            string fileName = Path.Combine(outputDirectory, $"Recording_{DateTime.Now:yyyyMMdd_HHmmss}.wav"); 
            wavefile = new WaveFileWriter(fileName, wavein.WaveFormat); 
            isRecording = true; 
            Console.WriteLine("Recording started...");
        } 
        private void StopRecording() 
        {
            if (isRecording) 
            { 
                isRecording = false; wavefile?.Dispose(); wavefile = null;
                Console.WriteLine("Recording stopped.");
            } }
        private void OnRecordingStopped(object sender, StoppedEventArgs e)
        { StopRecording(); } 
        private void CheckForSilence(object sender, ElapsedEventArgs e) 
        { 
            if (isRecording && (DateTime.Now - lastvoiceDetected).TotalSeconds > 3)
            { StopRecording(); } }
        private float GetMaxVolume(byte[] buffer, int bytesRecorded)
        { 
            float max = 0; 
            for (int index = 0; index < bytesRecorded; index += 2) 
            { 
                short sample = (short)((buffer[index + 1] << 8) | buffer[index]); float sample32 = sample / 32768f;
                if (sample32 < 0) sample32 = -sample32;
                if (sample32 > max) max = sample32; } 
            return max;
        }
        public void Stop()
        {
            wavein.StopRecording(); silenceTimer.Stop(); }
    }
}
