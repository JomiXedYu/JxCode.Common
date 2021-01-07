using System;
using System.Collections.Generic;

namespace JxCode.Common
{
    public struct JColor
    {
        private float _r;
        private float _g;
        private float _b;
        private float _a;

        #region prop
        public float r { get => _r; set => _r = Range1(value); }
        public float g { get => _g; set => _g = Range1(value); }
        public float b { get => _b; set => _b = Range1(value); }
        public float a { get => _a; set => _a = Range1(value); }

        public int R { get => Format1to255(_r); set => _r = Format255to1(value); }
        public int G { get => Format1to255(_g); set => _g = Format255to1(value); }
        public int B { get => Format1to255(_b); set => _b = Format255to1(value); }
        public int A { get => Format1to255(_a); set => _a = Format255to1(value); }

        public string Hex
        {
            get => ToRGBAHex();
            set
            {
                int len = value.Length;
                if (len < 6 || len > 9 || ((len == 9 || len == 7) && value.Substring(1) != "#"))
                    throw new ArgumentOutOfRangeException("十六进制颜色错误");
                //裁减掉#号
                if (len == 7 || len == 9) value = value.Substring(1);
                if (len == 6)
                    _a = 1f;
                else
                    A = Convert.ToInt32(value.Substring(6, 2), 16);
                R = Convert.ToInt32(value.Substring(0, 2), 16);
                G = Convert.ToInt32(value.Substring(2, 2), 16);
                B = Convert.ToInt32(value.Substring(4, 2), 16);
            }
        }

        private float h
        {
            get
            {
                float cmax = v;
                float c = rgbMax - rgbMin;
                float rst = 0;
                if (c == 0)
                    rst = 0;
                else if (cmax == this._r)
                    rst = 60 * (_g - _b) / c;
                else if (cmax == this._g)
                    rst = 60 * (_b - _r) / c + 2;
                else if (cmax == this._b)
                    rst = 60 * (_r - _g) / c + 4;

                if (rst < 0) rst += 360;
                return rst;
            }
            set => hsvToRgb(value, s, v);
        }
        private float s
        {
            get
            {
                if (this.v == 0) return 0;
                float c = rgbMax - rgbMin;
                return c / rgbMax;
            }
            set => hsvToRgb(h, value, v);
        }
        private float v
        {
            get => Math.Max(Math.Max(this._r, this._g), this._b);
            set => hsvToRgb(h, s, value);
        }
        public int H { get => (int)Math.Round(h); set => h = Range(value, 0, 360); }
        public int S { get => (int)Math.Round(s * 100); set => s = Range(value, 0, 100) / 100f; }
        public int V { get => (int)Math.Round(v * 100); set => v = Range(value, 0, 100) / 100f;  }

        private float rgbMin => Math.Min(Math.Min(_r, _g), _b);
        private float rgbMax => Math.Max(Math.Max(_r, _g), _b);
        private void hsvToRgb(float _h, float _s, float _v)
        {
            float _r = this._r;
            float _g = this._g;
            float _b = this._b;

            float c = _v * _s;
            float x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            float m = _v - c;
            if (0 <= _h && _h < 60) { _r = c; _g = x; _b = 0; }
            else if (60 <= _h && _h < 120) { _r = x; _g = c; _b = 0; }
            else if (120 <= _h && _h < 180) { _r = 0; _g = c; _b = x; }
            else if (180 <= _h && _h < 240) { _r = 0; _g = x; _b = c; }
            else if (240 <= _h && _h < 300) { _r = x; _g = 0; _b = c; }
            else if (300 <= _h && _h < 360) { _r = c; _g = 0; _b = x; }
            else { _r = 1; _g = 1; _b = 1; }

            this._r = _r + m;
            this._g = _g + m;
            this._b = _b + m;
        }
        #endregion
        #region ctor
        public JColor(float r, float g, float b)
        {
            this._r = r;
            this._g = g;
            this._b = b;
            this._a = 1;
        }
        public JColor(float r, float g, float b, float a)
        {
            this._r = r;
            this._g = g;
            this._b = b;
            this._a = a;
        }
        public JColor(int r, int g, int b)
        {
            _r = 1; _g = 1; _b = 1; _a = 1;
            this.R = r;
            this.G = g;
            this.B = b;
        }
        public JColor(int r, int g, int b, int a)
        {
            _r = 1; _g = 1; _b = 1; _a = 1;
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }
        public JColor(string hex)
        {
            _r = 1; _g = 1; _b = 1; _a = 1;
            this.Hex = hex;
        }
        #endregion
        #region range
        private float Range1(float num) => Range(num, 0, 1);
        private int Range100(int num) => Range(num, 0, 100);
        private int Range255(int num) => Range(num, 0, 255);
        private float Range(float num, float min, float max)
        {
            if (num < min)
                return min;
            else if (num > max)
                return max;
            else
                return num;
        }
        private int Range(int num, int min, int max)
        {
            if (num < min) return min;
            else if (num > max) return max;
            else return num;
        }
        #endregion
        #region format
        private float Format255to1(int i) => i / 255f;
        private int Format1to100(float value) => (int)Math.Round(value * 100f);
        private int Format1to255(float f) => (int)Math.Round(f * 255f);
        private float Format100to1(int i) => i / 100f;
        #endregion
        #region toString
        public override string ToString()
        {
            return ToRGBAHex();
        }
        public string ToRGBHex()
        {
            return R.ToString("X2") + G.ToString("X2") + B.ToString("X2");
        }
        public string ToRGBAHex()
        {
            return R.ToString("X2") + G.ToString("X2") + B.ToString("X2") + A.ToString("X2");
        }
        public string ToRGBString()
        {
            return R.ToString() + "," + G.ToString() + "," + B.ToString();
        }
        public string ToRGBAString()
        {
            return R.ToString() + "," + G.ToString() + "," + B.ToString() + "," + A.ToString();
        }
        public string TorgbString()
        {
            return r.ToString() + "," + g.ToString() + "," + b.ToString();
        }
        public string TorgbaString()
        {
            return TorgbString() + "," + a.ToString();
        }
        public int ToColorLong()
        {
            return Convert.ToInt32(ToRGBHex());
        }
        #endregion

        #region operator
        /// <summary>
        /// 增加亮度
        /// </summary>
        /// <param name="jcolor"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static JColor operator +(JColor jcolor, int value)
        {
            int v = jcolor.V;
            jcolor.V = v + value;
            return jcolor;
        }
        /// <summary>
        /// 减少亮度
        /// </summary>
        /// <param name="jcolor"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static JColor operator -(JColor jcolor, int value)
        {
            jcolor.V -= value;
            return jcolor;
        }
        #endregion

        public static JColor GetAverageColor(List<JColor> stream)
        {
            int r = 0;
            int g = 0;
            int b = 0;
            for (int i = 0; i < stream.Count; i++)
            {
                r += stream[i].R;
                g += stream[i].G;
                b += stream[i].B;
            }
            r /= stream.Count;
            g /= stream.Count;
            b /= stream.Count;
            return new JColor(r, g, b);
        }
    }
}
