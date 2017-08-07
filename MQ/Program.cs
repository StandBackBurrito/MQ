using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace MQ
{
    class IrEvent
    {
        public IrEvent(long ticks, bool high)
        {
            Timestamp = ticks;
            State = high;
        }

        public long Timestamp { get; set; }
        public bool State { get; set; }
    }


    class IrReader
    {
        private InterruptPort _receiver ;
        private IrEvent[] _events;
        private int _index;
        private Timer _timer;

        public IrReader(Cpu.Pin irIn)
        {
            _receiver = new InterruptPort(irIn, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);

            _receiver.OnInterrupt += _receiver_OnInterrupt;

            _events = new IrEvent[256];

            _index = 0;

            _timer = new Timer(new TimerCallback(RCtimeout), null, Timeout.Infinite, Timeout.Infinite);
        }

        void RCtimeout(object o)
        {
            var bits = 0;
            for (int i = 0; i < _events.Length - 1; i++)
            {
                if (_events[i + 1] != null && _events[i + 1].Timestamp != 0)
                {
                    bits++;
                    var length = _events[i + 1].Timestamp - _events[i].Timestamp;

                    Debug.Print("Event " + i.ToString() + ": was " + _events[i].State.ToString() + " for " + length.ToString() + "us");
                }
            }

            Debug.Print("Received " + bits.ToString() + "bits");

            _events = new IrEvent[256];
            _index = 0;

            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }


        void _receiver_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            _events[_index++] = new IrEvent(time.Ticks / (System.TimeSpan.TicksPerMillisecond / 1000), data2 == 1);

            if (_index >= _events.Length)
            {
                _index = 0;
            }

            _timer.Change(1, Timeout.Infinite); 
        }

    }
    public class Program
    {
        public static OutputPort Led { get; set; }
        public static long ticks { get; set; }
        public static void Main()
        {

            var reader = new IrReader(Pins.GPIO_PIN_D1);
            Thread.Sleep(Timeout.Infinite);
        }

    }
}
