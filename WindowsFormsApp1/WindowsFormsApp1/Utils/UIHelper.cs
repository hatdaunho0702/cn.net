using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WindowsFormsApp1.Utils
{
    /// <summary>
    /// Helper class cho các thao tác UI
    /// </summary>
    public static class UIHelper
    {
        /// <summary>
        /// T?o GraphicsPath cho h?nh ch? nh?t bo góc
        /// </summary>
        public static GraphicsPath GetRoundedRectangle(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();
            
            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // Top left arc
            path.AddArc(arc, 180, 90);
            
            // Top right arc
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            
            // Bottom right arc
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            
            // Bottom left arc
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// T?o màu hover (sáng hõn)
        /// </summary>
        public static Color GetHoverColor(Color baseColor)
        {
            return ControlPaint.Light(baseColor);
        }

        /// <summary>
        /// T?o màu pressed (t?i hõn)
        /// </summary>
        public static Color GetPressedColor(Color baseColor)
        {
            return ControlPaint.Dark(baseColor);
        }
    }
}
