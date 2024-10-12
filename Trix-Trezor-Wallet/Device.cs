using System;
using System.Linq;

namespace TrezorLib
{
    internal class Device
    {
        public static HidLibrary.HidDevice EndPoint;                
        public event EventHandler Attached;
        public event EventHandler Detached;
        System.Timers.Timer _timer;

        public bool IsConnected
        {
            get { return EndPoint != null; }
        }

        public void Connect()
        {
            if (_timer == null)
            {
                _timer = new System.Timers.Timer(500);
                _timer.Elapsed += _timer_Elapsed;
            }

            _timer.Start();
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            EndPoint = HidLibrary.HidDevices.Enumerate(21324).FirstOrDefault();

            if (EndPoint == null) return;

            EndPoint.Removed += _device_Removed;
            EndPoint.Inserted += _device_Inserted;
            EndPoint.MonitorDeviceEvents = true;
        }

        private void _device_Inserted()
        {
            _timer.Stop();

            if (EndPoint == null)
                EndPoint = HidLibrary.HidDevices.Enumerate(21324).FirstOrDefault();

            Attached?.Invoke(this, null);
        }

        private void _device_Removed()
        {
            EndPoint = null;
            Detached?.Invoke(this, null);
        }        
    }
}
