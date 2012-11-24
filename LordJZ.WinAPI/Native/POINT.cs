using System;
using System.Runtime.InteropServices;

namespace LordJZ.WinAPI.Native
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        int _x;
        int _y;

        public POINT(int x, int y)
        {
            this._x = x;
            this._y = y;
        }

        public int X
        {
            get { return this._x; }
            set { this._x = value; }
        }

        public int Y
        {
            get { return this._y; }
            set { this._y = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj is POINT)
            {
                var point = (POINT)obj;

                return point._x == this._x && point._y == this._y;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this._x.GetHashCode() ^ this._y.GetHashCode();
        }

        public static bool operator ==(POINT a, POINT b)
        {
            return a._x == b._x && a._y == b._y;
        }

        public static bool operator !=(POINT a, POINT b)
        {
            return !(a == b);
        }
    }
}