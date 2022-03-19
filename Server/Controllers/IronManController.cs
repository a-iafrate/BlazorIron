using BlazorIron.Shared;
using Microsoft.AspNetCore.Mvc;
using Iot.Device.ServoMotor;
using System.Device.Pwm;
using System.Device.Spi;
using Iot.Device.Ws28xx;
using Iot.Device.Graphics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Device.Gpio;
using System.Diagnostics;

namespace BlazorIron.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IronManController : ControllerBase
    {



        private int CurrentAngleMotor1 = 0;
        private int CurrentAngleMotor2 = 0;





        [DllImport("libc.so.6", SetLastError = true)]
        public static extern Int32 reboot(Int32 cmd, IntPtr arg);

        public const Int32 LINUX_REBOOT_CMD_RESTART = 0x01234567;
        public const Int32 LINUX_REBOOT_CMD_POWER_OFF = 0x4321FEDC;



        public IronSettings ironSettings = new IronSettings();

        private readonly ILogger<IronManController> _logger;
        private readonly IConfiguration Configuration;

        public IronManController(ILogger<IronManController> logger, IConfiguration configuration)
        {
            _logger = logger;

            Configuration = configuration;
            Configuration.GetSection(IronSettings.Position).Bind(ironSettings);

            CurrentAngleMotor1 = ironSettings.AngleCloseMotor1;
            CurrentAngleMotor2 = ironSettings.AngleOpenMotor2;
        }

        [HttpGet]
        public void Get()
        {
        }

        [HttpGet("[action]")]
        public void StartVoice()
        {

            ExecuteCommand("/home/pi/Public/ironserver/IronServer");

        }

        [HttpGet("[action]")]
        public void StopVoice()
        {

            ExecuteCommand("systemctl --user stop ironserver");

        }

        [HttpGet("[action]")]
        public void Reboot()
        {

            reboot(LINUX_REBOOT_CMD_RESTART, IntPtr.Zero);

        }



        [HttpGet("[action]")]
        public void EyesOn()
        {


            GpioController controller = new GpioController();
            // Sets the pin to output mode so we can switch something on
            controller.OpenPin(ironSettings.EyeSwitchLed, PinMode.Output);
            controller.Write(ironSettings.EyeSwitchLed, PinValue.High);

        }

        [HttpGet("[action]")]
        public void EyesOff()
        {


            GpioController controller = new GpioController();
            // Sets the pin to output mode so we can switch something on
            controller.OpenPin(ironSettings.EyeSwitchLed, PinMode.Output);
            controller.Write(ironSettings.EyeSwitchLed, PinValue.Low);

        }

        [HttpGet("[action]")]
        public void PowerOff()
        {

            reboot(LINUX_REBOOT_CMD_POWER_OFF, IntPtr.Zero);

        }

        [HttpGet("[action]")]
        public void OpenFace()
        {

            ServoMotor servoMotor = new ServoMotor(PwmChannel.Create(0, 1, 50),
                180,
                ironSettings.MinimumPulse,
                ironSettings.MaximumPulse);
            ServoMotor servoMotor2 = new ServoMotor(PwmChannel.Create(0, 0, 50), 180,
                ironSettings.MinimumPulse,
                ironSettings.MaximumPulse);

            servoMotor.Start();  // Enable control signal.

            servoMotor2.Start();  // Enable control signal.


            OpenFace(servoMotor, servoMotor2);


        }

        [HttpGet("[action]")]
        public void CloseFace()
        {

            ServoMotor servoMotor = new ServoMotor(PwmChannel.Create(0, 1, 50),
                180,
                ironSettings.MinimumPulse,
                ironSettings.MaximumPulse);
            ServoMotor servoMotor2 = new ServoMotor(PwmChannel.Create(0, 0, 50), 180,
                ironSettings.MinimumPulse,
                ironSettings.MaximumPulse);

            servoMotor.Start();  // Enable control signal.

            servoMotor2.Start();  // Enable control signal.

            CloseFace(servoMotor, servoMotor2);

        }

        [HttpGet("[action]")]
        public void LedOn()
        {

            var count = 14; // number of LEDs
            var settings = new SpiConnectionSettings(0, 0)
            {
                ClockFrequency = 2_400_000,
                Mode = SpiMode.Mode0,
                DataBitLength = 8
            };

            using SpiDevice spi = SpiDevice.Create(settings);

            var device = new Ws2812b(spi, count);

            BitmapImage image = device.Image;
            image.Clear();
            image.SetPixel(0, 0, Color.Orange);

            image.SetPixel(1, 0, Color.White);
            image.SetPixel(2, 0, Color.Blue);
            device.Update();

        }

        [HttpGet("[action]")]
        public void LedOff()
        {

            var count = 14; // number of LEDs
            var settings = new SpiConnectionSettings(0, 0)
            {
                ClockFrequency = 2_400_000,
                Mode = SpiMode.Mode0,
                DataBitLength = 8
            };

            using SpiDevice spi = SpiDevice.Create(settings);

            var device = new Ws2812b(spi, count);

            BitmapImage image = device.Image;
            image.Clear();
            device.Update();

        }

        [HttpPost("[action]")]
        public void MoveServo(ServoInfo input)
        {

            ServoMotor servoMotor = new ServoMotor(PwmChannel.Create(0, 1, 50),
                180,
                input.MinPulse,
                input.MaxPulse);
            ServoMotor servoMotor2 = new ServoMotor(PwmChannel.Create(0, 0, 50), 180,
                input.MinPulse,
                input.MaxPulse);

            servoMotor.Start();  // Enable control signal.

            servoMotor2.Start();  // Enable control signal.


            MoveTo(servoMotor, input.Angle1, servoMotor2, input.Angle2);


            servoMotor.Stop();
            servoMotor2.Stop();
        }

        private void ExecuteCommand(string command)
        {

            string result = "";

            //Process.Start(command);

            using (System.Diagnostics.Process proc = new System.Diagnostics.Process())
            {
                proc.StartInfo.FileName = "/bin/bash";
                proc.StartInfo.Arguments = "-c \" " + command + " \"";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();

                result += proc.StandardOutput.ReadToEnd();
                result += proc.StandardError.ReadToEnd();

                proc.WaitForExit();
            }

        }

        private void CloseFace(ServoMotor servoMotor, ServoMotor servoMotor2)
        {


            MoveTo(servoMotor, ironSettings.AngleCloseMotor1, CurrentAngleMotor1, servoMotor2, ironSettings.AngleCloseMotor2, CurrentAngleMotor2);

        }

        private void OpenFace(ServoMotor servoMotor, ServoMotor servoMotor2)
        {

            MoveTo(servoMotor, ironSettings.AngleOpenMotor1, CurrentAngleMotor1, servoMotor2, ironSettings.AngleOpenMotor2, CurrentAngleMotor2);

        }

        private void MoveTo(ServoMotor servoMotor, int angle, ServoMotor servoMotor2, int angle2)
        {

            MoveTo(servoMotor2, angle2, CurrentAngleMotor2, servoMotor, angle, CurrentAngleMotor1);

            CurrentAngleMotor1 = angle;
            CurrentAngleMotor2 = angle2;
        }

        private void MoveTo(ServoMotor s, int angle, int currentAngle, ServoMotor s1, int angle1, int currentAngle1)
        {
            int pos = currentAngle;
            int pos1 = currentAngle;
            while (currentAngle != angle || currentAngle1 != angle1)
            {
                if (currentAngle < angle)
                {
                    currentAngle += ironSettings.MotorSpeed;
                }
                if (currentAngle > angle)
                {
                    currentAngle -= ironSettings.MotorSpeed;
                }

                s.WriteAngle(currentAngle);


                if (currentAngle1 < angle1)
                {
                    currentAngle1 += ironSettings.MotorSpeed;
                }
                if (currentAngle1 > angle1)
                {
                    currentAngle1 -= ironSettings.MotorSpeed;
                }

                s.WriteAngle(currentAngle);
                s1.WriteAngle(currentAngle1);
                Thread.Sleep(10);
                Console.WriteLine("Current: " + currentAngle + " - Angle: " + angle);
                Console.WriteLine("Current1: " + currentAngle1 + " - Angle1: " + angle1);

            }

            CurrentAngleMotor1 = angle;
            CurrentAngleMotor2 = angle1;
        }
    }
}