using BlazorIron.Shared;
using Iot.Device.ServoMotor;
using System.Device.Pwm;
using System.Diagnostics;

namespace BlazorIron.Server.Helpers
{
    public class ServoHelper
    {
#if DEBUG
        private const bool SIMULATE = true;
#else
    private const bool SIMULATE = false;
#endif
        
        private static int CurrentAngleMotor1 = 0;
        private static int CurrentAngleMotor2 = 0;
        public IronSettings _settings;

        public static ServoHelper _current;
        public bool _isWorking=false;

        public static ServoHelper Current(IronSettings ironSettings)
        {
            if (_current==null)
            {
                _current = new ServoHelper();
                CurrentAngleMotor1 = ironSettings.AngleCloseMotor1;
                CurrentAngleMotor2 = ironSettings.AngleCloseMotor2;
            }
            _current._settings = ironSettings;
            return _current;
        }

        private ServoHelper() { }


        public void OpenFace()
        {
            if (_isWorking)
            {
                return;
            }
            if (!SIMULATE)
            {
                _isWorking = true;
                ServoMotor servoMotor = new ServoMotor(PwmChannel.Create(0, 1, 50),
                    180,
                    _settings.MinimumPulse,
                    _settings.MaximumPulse);
                ServoMotor servoMotor2 = new ServoMotor(PwmChannel.Create(0, 0, 50), 180,
                    _settings.MinimumPulse,
                    _settings.MaximumPulse);

                servoMotor.Start();  // Enable control signal.

                servoMotor2.Start();  // Enable control signal.


                OpenFace(servoMotor, servoMotor2);

                servoMotor.Stop();
                servoMotor2.Stop();
                _isWorking=false;
            }
            else
            {
                OpenFace(null, null);
            }
        }

        public void CloseFace()
        {
            if (_isWorking)
            {
                return;
            }
            if (!SIMULATE)
            {
                _isWorking = true;
                ServoMotor servoMotor = new ServoMotor(PwmChannel.Create(0, 1, 50),
                180,
                _settings.MinimumPulse,
                _settings.MaximumPulse);
                ServoMotor servoMotor2 = new ServoMotor(PwmChannel.Create(0, 0, 50), 180,
                    _settings.MinimumPulse,
                    _settings.MaximumPulse);

                servoMotor.Start();  // Enable control signal.

                servoMotor2.Start();  // Enable control signal.

                CloseFace(servoMotor, servoMotor2);

                servoMotor.Stop();
                servoMotor2.Stop();
                _isWorking = false;
            }
            else
            {
                CloseFace(null, null);
            }
        }


        private void CloseFace(ServoMotor servoMotor, ServoMotor servoMotor2)
        {


            MoveTo(servoMotor, _settings.AngleCloseMotor1, CurrentAngleMotor1, servoMotor2, _settings.AngleCloseMotor2, CurrentAngleMotor2);

        }

        private void OpenFace(ServoMotor servoMotor, ServoMotor servoMotor2)
        {

            MoveTo(servoMotor, _settings.AngleOpenMotor1, CurrentAngleMotor1, servoMotor2, _settings.AngleOpenMotor2, CurrentAngleMotor2);

        }

        private void MoveTo(ServoMotor servoMotor, int angle, ServoMotor servoMotor2, int angle2)
        {

            MoveTo(servoMotor2, angle2, CurrentAngleMotor2, servoMotor, angle, CurrentAngleMotor1);

            //CurrentAngleMotor1 = angle;
            //CurrentAngleMotor2 = angle2;
        }

        private void MoveTo(ServoMotor s, int angle, int currentAngle, ServoMotor s1, int angle1, int currentAngle1)
        {

            while (currentAngle != angle || currentAngle1 != angle1)
            {
                if (currentAngle < angle)
                {
                    currentAngle += _settings.MotorSpeed;
                }
                if (currentAngle > angle)
                {
                    currentAngle -= _settings.MotorSpeed;
                }


                if (currentAngle1 < angle1)
                {
                    currentAngle1 += _settings.MotorSpeed;
                }
                if (currentAngle1 > angle1)
                {
                    currentAngle1 -= _settings.MotorSpeed;
                }

                if (!SIMULATE)
                {
                    s.WriteAngle(currentAngle);
                    s1.WriteAngle(currentAngle1);
                }
                Thread.Sleep(25);
                Debug.WriteLine("Current: " + currentAngle + " - Current1: " + currentAngle1);

            }

            CurrentAngleMotor1 = angle;
            CurrentAngleMotor2 = angle1;
        }

        public void MoveTo(ServoInfo input)
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
    }
}
