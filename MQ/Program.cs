using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace MQ
{
    class IrEvent
    {
        public IrEvent(long ticks, bool high)
        {
            Ticks = ticks;
            High = high;
        }

        public long Ticks { get; set; }
        public bool High { get; set; }
    }


    class IrReader
    {
        private InterruptPort _receiver ;
        private IrEvent[] _events;
        private int _index;
        private Timer _timer;
        private long _lastTicks;

        public IrReader(Cpu.Pin irIn)
        {
            _receiver = new InterruptPort(irIn, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);

            var timer = new Timer()

            _receiver.OnInterrupt += _receiver_OnInterrupt;

            _events = new IrEvent[256];

            _index = 0;

            _lastTicks = DateTime.Now.Ticks;
            _timer = new Timer();
        }

        void _receiver_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            _events[_index++] = new IrEvent(_lastTicks - time.Ticks, data2 == 1);
            _lastTicks = time.Ticks;

            if (_index >= 255)
            {
                _index = 0;
                //Fire event
            }
        }

    }
    public class Program
    {
        public static OutputPort Led { get; set; }
        public static long ticks { get; set; }
        public static void Main()
        {
            Thread.Sleep(Timeout.Infinite);
        }

    }
}
