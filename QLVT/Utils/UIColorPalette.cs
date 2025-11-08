using System.Drawing;

namespace QLVT.Utils
{
    /// <summary>
    /// Centralized color palette for consistent UI theming across the application
    /// Based on Material Design principles
    /// </summary>
    public static class UIColorPalette
    {
        // ===== PRIMARY COLORS =====
        
        /// <summary>
        /// Primary Blue - Base color for primary actions, titles, highlights
        /// </summary>
        public static readonly Color PrimaryBlue = Color.FromArgb(52, 152, 219);  // #3498DB
        
        /// <summary>
        /// Primary Blue - Darker variant for hover states
        /// </summary>
        public static readonly Color PrimaryBlueDark = Color.FromArgb(41, 128, 185);  // #2980B9
        
        /// <summary>
        /// Success Green - Base color for positive actions, success messages
        /// </summary>
        public static readonly Color SuccessGreen = Color.FromArgb(46, 204, 113);  // #2ECC71
        
        /// <summary>
        /// Success Green - Darker variant for hover states
        /// </summary>
        public static readonly Color SuccessGreenDark = Color.FromArgb(39, 174, 96);  // #27AE60
        
        /// <summary>
        /// Warning Orange - Base color for important actions, warnings
        /// </summary>
        public static readonly Color WarningOrange = Color.FromArgb(230, 126, 34);  // #E67E22
        
        /// <summary>
        /// Warning Orange - Darker variant for hover states
        /// </summary>
        public static readonly Color WarningOrangeDark = Color.FromArgb(211, 84, 0);  // #D35400
        
        /// <summary>
        /// Danger Red - Base color for destructive actions
        /// </summary>
        public static readonly Color DangerRed = Color.FromArgb(231, 76, 60);  // #E74C3C
        
        /// <summary>
        /// Danger Red - Darker variant for hover states
        /// </summary>
        public static readonly Color DangerRedDark = Color.FromArgb(192, 57, 43);  // #C0392B
        
        
        // ===== BACKGROUND COLORS =====
        
        /// <summary>
        /// Light Gray - Main background color for forms and panels
        /// </summary>
        public static readonly Color BackgroundLight = Color.FromArgb(236, 240, 241);  // #ECF0F1
        
        /// <summary>
        /// White - Background for cards, panels, and content areas
        /// </summary>
        public static readonly Color BackgroundWhite = Color.White;  // #FFFFFF
        
        
        // ===== TEXT COLORS =====
        
        /// <summary>
        /// Dark Gray - Primary text color for headers and titles
        /// </summary>
        public static readonly Color TextDark = Color.FromArgb(52, 73, 94);  // #34495E
        
        /// <summary>
        /// Black - Standard text color
        /// </summary>
        public static readonly Color TextBlack = Color.Black;  // #000000
        
        /// <summary>
        /// White - Text on dark backgrounds
        /// </summary>
        public static readonly Color TextWhite = Color.White;  // #FFFFFF
        
        
        // ===== STATUS COLORS =====
        
        /// <summary>
        /// Status - Success (Green)
        /// </summary>
        public static readonly Color StatusSuccess = Color.FromArgb(46, 204, 113);  // #2ECC71
        
        /// <summary>
        /// Status - Error (Red)
        /// </summary>
        public static readonly Color StatusError = Color.Red;
        
        /// <summary>
        /// Status - Processing (Blue)
        /// </summary>
        public static readonly Color StatusProcessing = Color.Blue;
        
        /// <summary>
        /// Status - Warning (Dark Red for text)
        /// </summary>
        public static readonly Color StatusWarningText = Color.DarkRed;
        
        /// <summary>
        /// Status - Warning (Light Pink for background)
        /// </summary>
        public static readonly Color StatusWarningBackground = Color.LightPink;
        
        
        // ===== BUTTON COLORS =====
        
        /// <summary>
        /// Button style: Primary (Blue) - For search, query, load actions
        /// </summary>
        public static class ButtonPrimary
        {
            public static readonly Color Base = PrimaryBlue;
            public static readonly Color Hover = PrimaryBlueDark;
            public static readonly Color Text = TextWhite;
        }
        
        /// <summary>
        /// Button style: Success (Green) - For refresh, reload, save actions
        /// </summary>
        public static class ButtonSuccess
        {
            public static readonly Color Base = SuccessGreen;
            public static readonly Color Hover = SuccessGreenDark;
            public static readonly Color Text = TextWhite;
        }
        
        /// <summary>
        /// Button style: Warning (Orange) - For confirm, submit, execute actions
        /// </summary>
        public static class ButtonWarning
        {
            public static readonly Color Base = WarningOrange;
            public static readonly Color Hover = WarningOrangeDark;
            public static readonly Color Text = TextWhite;
        }
        
        /// <summary>
        /// Button style: Danger (Red) - For delete, cancel, remove actions
        /// </summary>
        public static class ButtonDanger
        {
            public static readonly Color Base = DangerRed;
            public static readonly Color Hover = DangerRedDark;
            public static readonly Color Text = TextWhite;
        }
    }
}
