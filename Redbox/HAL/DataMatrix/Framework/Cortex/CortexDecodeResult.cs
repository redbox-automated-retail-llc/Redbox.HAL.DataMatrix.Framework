using System.Drawing;

namespace Redbox.HAL.DataMatrix.Framework.Cortex
{
    public sealed class CortexDecodeResult
    {
        public override string ToString()
        {
            return string.Format("{0},{1};{2},{3};{4},{5};{6},{7} Matrix = {8}", (object)this.corner0.X, (object)this.corner0.Y, (object)this.corner1.X, (object)this.corner1.Y, (object)this.corner2.X, (object)this.corner2.Y, (object)this.corner3.X, (object)this.corner3.Y, (object)this.DecodeData);
        }

        internal Point[] PolyPoints
        {
            get
            {
                return new Point[4]
                {
          this.corner0,
          this.corner1,
          this.corner2,
          this.corner3
                };
            }
        }

        internal string DecodeData { get; set; }

        internal int dataLength { get; set; }

        internal Point corner0 { get; set; }

        internal Point corner1 { get; set; }

        internal Point corner2 { get; set; }

        internal Point corner3 { get; set; }

        internal Point center { get; set; }

        internal int symbolType { get; set; }

        internal int symbolModifier { get; set; }
    }
}
