using BlazorIron.Shared;
using Microsoft.AspNetCore.Mvc;
using Iot.Device.ServoMotor;
using System.Device.Pwm;


namespace BlazorIron.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IronManController : ControllerBase
    {
        
        private const int ANGLE_OPEN_MOTOR1 = 160;
        private const int ANGLE_OPEN_MOTOR2 = 20;
        private const int ANGLE_CLOSE_MOTOR1 = 20;
        private const int ANGLE_CLOSE_MOTOR2 = 160;

        private static int CurrentAngleMotor1 = 20;
        private static int CurrentAngleMotor2 = 160;
        private static int MOTOR_SPEED = 5;


        private readonly ILogger<IronManController> _logger;

        public IronManController(ILogger<IronManController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public void Get()
        {
        }

        [HttpGet("[action]")]
        public void Test()
        {
           


           

            ServoMotor servoMotor = new ServoMotor(PwmChannel.Create(0, 1, 50),
                180,
                1000,
                2000);
            ServoMotor servoMotor2 = new ServoMotor(PwmChannel.Create(0, 0, 50), 180,
                1000,
                2000);

            servoMotor.Start();  // Enable control signal.

            servoMotor2.Start();  // Enable control signal.
            

            OpenFace(servoMotor, servoMotor2);
            Thread.Sleep(5000);
            CloseFace(servoMotor, servoMotor2);

        }

        [HttpPost("[action]")]
        public void MoveServo(ServoInfo input)
        {

            ServoMotor servoMotor = new ServoMotor(PwmChannel.Create(0, 1, 50),
                180,
                1000,
                2000);
            ServoMotor servoMotor2 = new ServoMotor(PwmChannel.Create(0, 0, 50), 180,
                1000,
                2000);

            servoMotor.Start();  // Enable control signal.

            servoMotor2.Start();  // Enable control signal.


            MoveTo(servoMotor,input.Angle1,servoMotor2,input.Angle2);



        }

        private void CloseFace(ServoMotor servoMotor, ServoMotor servoMotor2)
        {


            MoveTo(servoMotor, ANGLE_CLOSE_MOTOR1, CurrentAngleMotor1, servoMotor2, ANGLE_CLOSE_MOTOR2, CurrentAngleMotor2);



            CurrentAngleMotor1 = ANGLE_CLOSE_MOTOR1;
            CurrentAngleMotor2 = ANGLE_CLOSE_MOTOR2;
        }

        private void OpenFace(ServoMotor servoMotor, ServoMotor servoMotor2)
        {

            MoveTo(servoMotor2, ANGLE_OPEN_MOTOR2, CurrentAngleMotor2, servoMotor, ANGLE_OPEN_MOTOR1, CurrentAngleMotor1);

            CurrentAngleMotor1 = ANGLE_OPEN_MOTOR1;
            CurrentAngleMotor2 = ANGLE_OPEN_MOTOR2;
        }

        private void MoveTo(ServoMotor servoMotor,int angle, ServoMotor servoMotor2, int angle2)
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
                    currentAngle += MOTOR_SPEED;
                }
                if (currentAngle > angle)
                {
                    currentAngle -= MOTOR_SPEED;
                }

                s.WriteAngle(currentAngle);


                if (currentAngle1 < angle1)
                {
                    currentAngle1 += MOTOR_SPEED;
                }
                if (currentAngle1 > angle1)
                {
                    currentAngle1 -= MOTOR_SPEED;
                }

                s.WriteAngle(currentAngle);
                s1.WriteAngle(currentAngle1);
                Thread.Sleep(10);
                Console.WriteLine("Current: " + currentAngle + " - Angle: " + angle);
                Console.WriteLine("Current1: " + currentAngle1 + " - Angle1: " + angle1);

            }
        }
    }
}