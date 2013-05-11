using System.Windows.Media;

namespace PlanEditor
{
    public class Colours
    {
        private static SolidColorBrush black = new SolidColorBrush();
        private static SolidColorBrush gray = new SolidColorBrush();        
        private static SolidColorBrush yellow = new SolidColorBrush();
        private static SolidColorBrush violet = new SolidColorBrush();
        private static SolidColorBrush emerland = new SolidColorBrush();
        private static SolidColorBrush indigo = new SolidColorBrush();
        private static SolidColorBrush cyan = new SolidColorBrush();
        private static SolidColorBrush pink = new SolidColorBrush();
        private static SolidColorBrush white = new SolidColorBrush();
        private static SolidColorBrush red = new SolidColorBrush();
        private static SolidColorBrush green = new SolidColorBrush();
        private static SolidColorBrush lightGray = new SolidColorBrush();

        public static SolidColorBrush Black { get { black.Color = Colors.Black; return black; } }
        public static SolidColorBrush Gray { get { gray.Color = Colors.Gray; return gray; } }
        public static SolidColorBrush Yellow { get { yellow.Color = Colors.Yellow; return yellow; } }
        public static SolidColorBrush Violet { get { violet.Color = Color.FromArgb(255, 170, 0, 255); return violet; } }
        public static SolidColorBrush Emerland { get { emerland.Color = Color.FromArgb(255, 0, 138, 0); return emerland; } }
        public static SolidColorBrush Indigo { get { indigo.Color = Color.FromArgb(255, 109, 0, 255); return indigo; } }
        public static SolidColorBrush Cyan { get { cyan.Color = Color.FromArgb(255, 27, 161, 226); return cyan; } }
        public static SolidColorBrush Pink { get { pink.Color = Color.FromArgb(255, 244, 141, 208); return pink; } }
        public static SolidColorBrush White { get { white.Color = Colors.White; return white; } }
        public static SolidColorBrush Red { get { red.Color = Colors.Red; return red; } }
        public static SolidColorBrush Green { get { green.Color = Colors.Green; return green; } }
        public static SolidColorBrush LightGray { get { lightGray.Color = Colors.LightGray; return lightGray; } }
    }
}
