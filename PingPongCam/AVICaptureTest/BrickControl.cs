using System;
using System.Collections.Generic;
using System.Text;

using NKH.MindSqualls;
using NKH.MindSqualls.MotorControl;

namespace AVICaptureTest
{
    class BrickControl
    {
        public static void RunForward()
        {
            NxtBrick brick = new NxtBrick(NxtCommLinkType.USB, 0);

            // Attach a motor to port B.
            brick.MotorB = new NxtMotor();

            // Connect to the NXT.
            brick.Connect();
            Console.WriteLine("NXT name: " + brick.Name);
            System.Diagnostics.Debug.WriteLine("NXT name: " + brick.Name);

            // Write out the firmware version of the NXT-brick.
            var conn = new NxtUsbConnection();
            NxtGetFirmwareVersionReply? reply = conn.GetFirmwareVersion();
            if (reply.HasValue)
                System.Diagnostics.Debug.WriteLine("NXT firmware version: " + reply.Value.firmwareVersion);

            // see: http://www.mindsqualls.net/MotorControl.aspx
            MotorControlProxy.StartMotorControl(brick.CommLink);
            System.Threading.Thread.Sleep(500);

            MotorControlProxy.CONTROLLED_MOTORCMD(brick.CommLink, MotorControlMotorPort.PortB,"111", "000360", '5');
            System.Diagnostics.Debug.WriteLine("Done");
            System.Threading.Thread.Sleep(5000);
            MotorControlProxy.StopMotorControl(brick.CommLink);
            brick.Disconnect();
        }
    }
}
