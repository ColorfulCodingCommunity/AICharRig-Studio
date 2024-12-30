using System.Diagnostics;
using System.IO;
using UnityEngine;

public class RenderingEngine : MonoBehaviour
{
    public void RunFFMpeg(string arguments)
    {
        var ffmpegPath = Path.Join(Application.streamingAssetsPath, "ffmpeg-7.1-essentials_build", "bin");
        ffmpegPath = Path.Combine(ffmpegPath, "ffmpeg.exe");

        UnityEngine.Debug.Log($"Running command: {ffmpegPath} {arguments}");
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
        }
    }

    public void ImageSequenceToVideoAndAudio(string mainFolder, string imageSequencePath, string auxOutPath)
    {
        ImageSequenceToVideo(imageSequencePath, auxOutPath);

        string outputPath = Path.Combine(mainFolder, "out.mp4");
        if (File.Exists(outputPath))
        {
            File.Delete(outputPath);
        }

        //Add audio to the video
        AudioClip drivingAudio = AssetManager.Instance.drivingAudio;

        if(drivingAudio == null)
        {
            File.Move(Path.Combine(auxOutPath, "out.mp4"), outputPath);
            return;
        }

        //Else add render video with audio
        string audioPath = Path.Combine(auxOutPath, "out.wav");
        SavWav.Save(audioPath, drivingAudio);

        RunFFMpeg($"-i {auxOutPath}\\out.mp4 -i {audioPath} -c copy -map 0:v:0 -map 1:a:0 {outputPath}");
    }

    public void ImageSequenceToVideo(string imageSequencePath, string outputFolderPath)
    {
        string outputPath = Path.Combine(outputFolderPath, "out.mp4");
        if (File.Exists(outputPath))
        {
            File.Delete(outputPath);
        }

        RunFFMpeg($"-framerate 30 -i {imageSequencePath}\\frame_%d.png -c:v libx264 -r 30 -pix_fmt yuv420p {outputPath}");
    }
}
