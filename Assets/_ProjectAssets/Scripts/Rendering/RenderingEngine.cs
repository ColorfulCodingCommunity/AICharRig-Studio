using System.Diagnostics;
using System.IO;
using UnityEngine;

public class RenderingEngine : MonoBehaviour
{
    public void RunFFMpeg(string arguments)
    {
        var ffmpegPath = Path.Join(Application.streamingAssetsPath, "ffmpeg-7.1-essentials_build", "bin");
        ffmpegPath = Path.Combine(ffmpegPath, "ffmpeg.exe");

        ProcessStartInfo processStartInfo = new ProcessStartInfo
        {
            FileName = ffmpegPath,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = new Process())
        {
            process.StartInfo = processStartInfo;
            process.OutputDataReceived += (sender, args) => UnityEngine.Debug.Log(args.Data);
            process.ErrorDataReceived += (sender, args) => UnityEngine.Debug.LogError(args.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            UnityEngine.Debug.Log("Done!");
        }
    }

    public void ImageSequenceToVideo(string mainFolder, string imageSequencePath, string auxOutPath)
    {

        //Generate mp4 from image sequence
        RunFFMpeg($"-framerate 30 -i {imageSequencePath}\\frame_%d.png -c:v libx264 -r 30 {auxOutPath}\\output.mp4");

        //Add audio to the video
        AudioClip drivingAudio = AssetManager.Instance.drivingAudio;
        string audioPath = Path.Combine(auxOutPath, "out.wav");

        SavWav.Save(audioPath, drivingAudio);
        RunFFMpeg($"-i {auxOutPath}\\output.mp4 -i {audioPath} -c copy -map 0:v:0 -map 1:a:0 {mainFolder}\\out_final.mp4");
    }
}
