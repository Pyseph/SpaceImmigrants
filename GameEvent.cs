using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceImmigrants
{
    public class Event
    {
        // Define a general event to inherit from
        // Each event needs to be manually defined in order to support type safety

        public class InvokedEvent<T> : Event
        {
            public delegate void InvokedDelegate(T Data);
            public event InvokedDelegate Invoked;
            public InvokedEvent()
            {
                Invoked = delegate { };
            }

            public void Invoke(T Data)
            {
                Invoked?.Invoke(Data);
            }
        }

        // Define specific events
        public static InvokedEvent<double> GamePreStepped = new();
        public static InvokedEvent<double> GamePostStepped = new();
        public static InvokedEvent<Keys> InputBegan = new();
        public static InvokedEvent<Keys> InputEnded = new();
        public static InvokedEvent<bool> GameEnded = new();

        public static InvokedEvent<Point> MouseButton1Down = new();
    }
}