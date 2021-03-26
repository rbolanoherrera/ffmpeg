using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MarcaAgua
{
    class Program
    {
        //static Process process;
        static TaskCompletionSource<bool> eventHandled;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Inicio proceso de Maracado de Agua");

            string fileName = Environment.CurrentDirectory + "\\sio-sio_el-jedig.mp4";
            fileName = Environment.CurrentDirectory + "\\PruebaZoom-18-03-01.mp4";
            string workingDirectory = Environment.CurrentDirectory;// @"D:\dev\Net\libs\ffmpeg\bin";
            string outputFileName = "videoConTexto_" + getCurrentDate() + DateTime.Now.Millisecond.ToString() + ".mp4";
            string strCommand = "";
            string duracion = "9 min";

            //coimando con texto en un Archivo
            //string strCommand = "-y -i \"" + fileName + "\" -vf drawtext=\"textfile=tiempoNotaria.txt: fontcolor=white: fontsize=20: x=25: y=h-50: box=1: boxcolor=black@0.5:\" -codec:a copy \"" + outputFileName + "\"";
            //strCommand = $"-y -i {fileName} {outputFileName}";
            strCommand = $"-y -i {fileName} -vf \"[in]drawtext=fontcolor=white: fontsize=20: text='Fecha del tramite\\: {getCurrentDateText()}': x=25: y=h-56: box=1: boxcolor=black@0.5, drawtext=fontcolor=white: fontsize=20: text='Duración\\: {duracion}' :x=25: y=h-40: box=1: boxcolor=black@0.5\" -codec:a copy {outputFileName}";

            eventHandled = new TaskCompletionSource<bool>();

            var task = Task.Run(() =>
            {

                var startInfo = new ProcessStartInfo
                {
                    FileName = Environment.CurrentDirectory + "\\ffmpeg.exe",
                    Arguments = strCommand,
                    WorkingDirectory = workingDirectory,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                //using (process = new Process())
                using (var process = new Process { StartInfo = startInfo })
                {
                    //process.StartInfo = startInfo;
                    //process.EnableRaisingEvents = true;
                    //process.Exited += new EventHandler(process_Exited);
                    process.Start();
                    //processId = process.Id;

                    Console.WriteLine("startTime: " + process.StartTime);

                    process.WaitForExit();

                    while (!process.HasExited)
                    {
                        Console.WriteLine($"{getCurrentDate()}: Ejecutando..");
                    }

                    Console.WriteLine("exitTime: " + process.ExitTime);
                    Console.WriteLine($"tiempo de ejecución : {Math.Round((process.ExitTime - process.StartTime).TotalSeconds)} seg");
                }

            });

            await task;

            Console.WriteLine(getCurrentDate() + ": Proceso 1 Terminado.");

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine(getCurrentDate() + ": Proceso 2 Iniciado.");

            string outputFileName2 = $"videoSalida{getCurrentDate()}{DateTime.Now.Millisecond.ToString()}.mp4";
            string waterMark = Environment.CurrentDirectory + "\\logolargo-blanco2.png";

            var task1 = Task.Run(() =>
            {
                var startInfo2 = new ProcessStartInfo
                {
                    FileName = Environment.CurrentDirectory + "\\ffmpeg.exe",
                    Arguments = $"-y -i \"{outputFileName}\" -i \"{waterMark}\" -filter_complex \"overlay=main_w-overlay_w-5:main_h-overlay_h-5\" -codec:a copy {outputFileName2}",
                    WorkingDirectory = workingDirectory,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                using (var process2 = new Process { StartInfo = startInfo2 })
                {
                    //process2.EnableRaisingEvents = true;
                    //process2.Exited += new EventHandler(process2_Exited);
                    process2.Start();
                    process2.WaitForExit();

                    //while (!process2.HasExited)
                    //{
                    //    Console.WriteLine(getCurrentDate() + ": proceso2 Ejecutandose...");
                    //}

                    Console.WriteLine("exitTime2: " + process2.ExitTime);
                    Console.WriteLine($"tiempo de ejecución2 : {Math.Round((process2.ExitTime - process2.StartTime).TotalSeconds)} seg");
                }

            });

            await task1;

            Console.WriteLine(getCurrentDate() + ": Proceso 2 Terminado.");

            Console.WriteLine(getCurrentDate() + ": Ejecución Consola terminada...");
        }


        // Handle Exited event and display process information.
        private static void process_Exited(object sender, System.EventArgs e)
        {
            Console.WriteLine(getCurrentDate() + ": Proceso 1 Termina do.\n\n");

            //Console.WriteLine(
            //    $"Exit time    : {process.ExitTime}\n" +
            //    $"Exit code    : {process.ExitCode}\n" +
            //    $"Elapsed time : {Math.Round((process.ExitTime - process.StartTime).TotalMilliseconds)}");

            eventHandled.TrySetResult(true);

            Console.WriteLine("Termino el proceso de conversión");
        }

        // Handle Exited event and display process information.
        private static void process2_Exited(object sender, System.EventArgs e)
        {
            Console.WriteLine(getCurrentDate() + ": Proceso 2 Termina do.\n\n");

            //Console.WriteLine(
            //    $"Exit time    : {process.ExitTime}\n" +
            //    $"Exit code    : {process.ExitCode}\n" +
            //    $"Elapsed time : {Math.Round((process.ExitTime - process.StartTime).TotalMilliseconds)}");

            eventHandled.TrySetResult(true);

            Console.WriteLine("Termino el proceso de conversión 2");
        }

        /// <summary>
        /// Obtener la fecha actual en formaro: hora minutos segundos
        /// </summary>
        /// <returns></returns>
        static string getCurrentDate()
        {
            return DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00") +
                "_" + DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + DateTime.Now.Second.ToString("00");
        }
        
        static string getCurrentDateText()
        {
            return DateTime.Now.Day.ToString("00") + "/"+ DateTime.Now.Month.ToString("00") + "/" + DateTime.Now.Year.ToString("0000");
        }

        private static TimeSpan GetVideoDuration(string filePath)
        {
            using (var shell = ShellObject.FromParsingName(filePath))
            {
                IShellProperty prop = shell.Properties.System.Media.Duration;
                var t = (ulong)prop.ValueAsObject;
                return TimeSpan.FromTicks((long)t);
            }
        }

    }
}
