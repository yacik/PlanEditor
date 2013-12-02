using System.Windows.Media;

namespace PlanEditor.Helpers
{
    public class Colours
    {
        private static readonly SolidColorBrush black = new SolidColorBrush();
        private static readonly SolidColorBrush gray = new SolidColorBrush();        
        private static readonly SolidColorBrush yellow = new SolidColorBrush();
        private static readonly SolidColorBrush violet = new SolidColorBrush();
        private static readonly SolidColorBrush emerland = new SolidColorBrush();
        private static readonly SolidColorBrush indigo = new SolidColorBrush();
        private static readonly SolidColorBrush cyan = new SolidColorBrush();
        private static readonly SolidColorBrush pink = new SolidColorBrush();
        private static readonly SolidColorBrush white = new SolidColorBrush();
        private static readonly SolidColorBrush red = new SolidColorBrush();
        private static readonly SolidColorBrush green = new SolidColorBrush();
        private static readonly SolidColorBrush lightGray = new SolidColorBrush();

        public static SolidColorBrush Black { get { black.Color = Colors.Black; return black; } }
        public static SolidColorBrush Gray { get { gray.Color = Colors.Gray; return gray; } }
        public static SolidColorBrush Yellow { get { yellow.Color = Colors.Yellow; return yellow; } }
        public static SolidColorBrush Violet { get { violet.Color = Color.FromArgb(255, 156, 58, 255); return violet; } }
        public static SolidColorBrush Emerland { get { emerland.Color = Color.FromArgb(255, 0, 138, 0); return emerland; } }
        public static SolidColorBrush Indigo { get { indigo.Color = Color.FromArgb(255, 109, 93, 255); return indigo; } }
        public static SolidColorBrush Cyan { get { cyan.Color = Color.FromArgb(255, 27, 161, 226); return cyan; } }
        public static SolidColorBrush Pink { get { pink.Color = Color.FromArgb(255, 244, 141, 208); return pink; } }
        public static SolidColorBrush White { get { white.Color = Colors.White; return white; } }
        public static SolidColorBrush Red { get { red.Color = Colors.Red; return red; } }
        public static SolidColorBrush Green { get { green.Color = Color.FromArgb(255, 63, 96, 63); return green; } }
        public static SolidColorBrush LightGray { get { lightGray.Color = Colors.LightGray; return lightGray; } }


        private static readonly SolidColorBrush room = new SolidColorBrush();
        public static SolidColorBrush Room { get { room.Color = Color.FromArgb(255, 160, 160, 160); return room; } }

        private static readonly SolidColorBrush halfway = new SolidColorBrush();
        public static SolidColorBrush Halfway { get { halfway.Color = Color.FromArgb(255, 192, 192, 192); return halfway; } }

        private static readonly SolidColorBrush stairway = new SolidColorBrush();
        public static SolidColorBrush Stairway { get { stairway.Color = Color.FromArgb(255, 128, 128, 128); return stairway; } }
    }
}
