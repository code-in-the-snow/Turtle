using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turtle
{
    class Program
    {
        
        public struct Point
        {
            public double X { get; set; }
            public double Y { get; set; }
            public Point(double x, double y)
                :this()
            {
                X = x;
                Y = y;
            }
            
        }
        
        class Turtle
        {
            public double PlatformWidth { get; set; }
            public double PlatformHeight { get; set; }
            public double MotorSpeed { get; set; }
            public Point CurrentPosition { get; private set;}
            public double CurrentOrientation { get; private set;}
             
            //current state of the motor
            public enum MotorState
            {
                Stopped,
                Running,
                Reversed
            }
            public MotorState LeftMotorState { get; set; }
            public MotorState RightMotorState { get; set; }

            
            // Run the turtle for specified duration
            public void RunFor(double duration)
            {
                if (duration <= double.Epsilon)
                {
                    throw new ArgumentException(
                        "Must provide a duration greater than 0.", 
                        "duration");
                }
                try
                {
                    if (LeftMotorState == MotorState.Stopped &&
                    RightMotorState == MotorState.Stopped)
                    {
                        // Nothing happends at full stop
                        return;
                    }
                    else if ((LeftMotorState == MotorState.Running &&
                             RightMotorState == MotorState.Running) ||
                             (LeftMotorState == MotorState.Reversed &&
                             RightMotorState == MotorState.Reversed))
                    {
                        // Motors are running in the same direction; just drive.
                        Drive(duration);
                        return;
                    }
                    else if ((LeftMotorState == MotorState.Running &&
                             RightMotorState == MotorState.Reversed) ||
                             (LeftMotorState == MotorState.Reversed &&
                             RightMotorState == MotorState.Running))
                    {
                        // Motors are running in opposite directions, just
                        //    rotate about center of rig.
                        Rotate(duration);
                        return;
                    }
                }
                catch (InvalidOperationException e)
                {
                    throw new Exception("Some problem with the turtle has occurred.", e);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Log message: " + e.Message);
                    throw;
                }
                finally
                {
                    Console.WriteLine("In the Turtle finally block.");
                }
            }

            private void Rotate(double duration)
            {
                if (PlatformWidth <= 0)
                {
                    throw new InvalidOperationException(
                        "The PlatformWidth must be initialized to a value > 0.0.");
                }
                //Total circumference of turning circle
                double circum = Math.PI * PlatformWidth;
                //Total distance traveled
                double dist = duration * MotorSpeed;
                if (LeftMotorState == MotorState.Reversed)
                {
                    //Moving backwards if motors are reversed
                    dist *= -1.0;
                }
                double proportionOfWholeCircle = dist / circum;
                //One complete rotation is 360 degrees of 2i radians
                CurrentOrientation = CurrentOrientation + (Math.PI * 2.0 * proportionOfWholeCircle);
            }

            private void Drive(double duration)
            {
                //Total distance traveled
                double dist = duration * MotorSpeed;
                if ((LeftMotorState == MotorState.Reversed) ||
                    (RightMotorState == MotorState.Reversed))
                {
                    // Moving backwards if motors are reversed
                    dist *= -1.0;
                }
                //Calculate change in X, Y coordinates
                double deltaX = dist * Math.Sin(CurrentOrientation);
                double deltaY = dist * Math.Cos(CurrentOrientation);

                //Update position
                CurrentPosition = 
                    new Point(CurrentPosition.X + deltaX, CurrentPosition.Y + deltaY);
            }

            static void Main(string[] args)
            {
                Turtle arthurTurtle =
                    new Turtle { PlatformWidth = 0.0, PlatformHeight = 10.0, MotorSpeed = 5.0 };

                ShowPosition(arthurTurtle);

                try
                {
                    arthurTurtle.RunFor(0.0);

                    // Test forward.
                    arthurTurtle.LeftMotorState = MotorState.Running;
                    arthurTurtle.RightMotorState = MotorState.Running;
                    arthurTurtle.RunFor(2.0);

                    ShowPosition(arthurTurtle);

                    // Test rotate.
                    arthurTurtle.RightMotorState = MotorState.Reversed;
                    arthurTurtle.RunFor(Math.PI / 2.0);

                    ShowPosition(arthurTurtle);

                    // Test reverse.
                    arthurTurtle.LeftMotorState = MotorState.Reversed;
                    arthurTurtle.RightMotorState = MotorState.Reversed;
                    arthurTurtle.RunFor(5.0);

                    ShowPosition(arthurTurtle);

                    // Rotate back the other way.
                    arthurTurtle.RightMotorState = MotorState.Running;
                    arthurTurtle.RunFor(Math.PI / 4.0);

                    ShowPosition(arthurTurtle);

                    //Drive backwards.
                    arthurTurtle.RightMotorState = MotorState.Reversed;
                    arthurTurtle.LeftMotorState = MotorState.Reversed;
                    arthurTurtle.RunFor(Math.PI / 4);

                    ShowPosition(arthurTurtle);
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine("Error running turtle: ");
                    Console.WriteLine(e.Message);
                }
                catch (Exception e)
                {
                    // Loop through the inner exceptions, printing their messages
                    Exception current = e;
                    while (current != null)
                    {
                        Console.WriteLine(current.Message);
                        current = current.InnerException;
                    }
                }

                finally
                {
                    Console.WriteLine();
                    Console.WriteLine("Waiting in the finally block...");
                    Console.ReadKey();
                }
                
            }

            private static void ShowPosition(Turtle aTurtle)
            {
                Console.WriteLine(
                    "Arthur is at ({0}, {1}) and is pointing at angle {2} radians.",
                    aTurtle.CurrentPosition.X.ToString("0.00"), aTurtle.CurrentPosition.Y.ToString("0.00"), aTurtle.CurrentOrientation.ToString("0.00"));
            }
        }
    }
}
