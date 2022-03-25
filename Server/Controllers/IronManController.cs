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
using BlazorIron.Server.Helpers;

namespace BlazorIron.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IronManController : ControllerBase
    {







        [DllImport("libc.so.6", SetLastError = true)]
        public static extern Int32 reboot(Int32 cmd, IntPtr arg);

        public const Int32 LINUX_REBOOT_CMD_RESTART = 0x01234567;
        public const Int32 LINUX_REBOOT_CMD_POWER_OFF = 0x4321FEDC;



        public IronSettings ironSettings = new IronSettings();

        private readonly ILogger<IronManController> _logger;
        private readonly IConfiguration Configuration;
        private ServoHelper servoHelper;

        public IronManController(ILogger<IronManController> logger, IConfiguration configuration)
        {
            _logger = logger;

            Configuration = configuration;
            Configuration.GetSection(IronSettings.Position).Bind(ironSettings);
            servoHelper=ServoHelper.Current(ironSettings);
            
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
        public IronSettings Config()
        {

            return ironSettings;

        }

        [HttpGet("[action]")]
        public void PowerOff()
        {

            reboot(LINUX_REBOOT_CMD_POWER_OFF, IntPtr.Zero);

        }

        [HttpGet("[action]")]
        public void OpenFace()
        {
            servoHelper.OpenFace();

        }

        [HttpGet("[action]")]
        public void CloseFace()
        {
            servoHelper.CloseFace();
        }

        [HttpGet("[action]")]
        public void LedOn()
        {
            Color c = Color.FromName(ironSettings.HelmetLedColor);
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
            image.SetPixel(ironSettings.HelmetLed1Pos, 0,c);

            image.SetPixel(ironSettings.HelmetLed2Pos, 0, c);
            
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

            servoHelper.MoveTo(input);
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

        
    }
}