using System.Drawing;

namespace QLVT.Utils
{
    /// <summary>
    /// Centralized color palette for consistent UI theming across the application
    /// Neutral, modern palette for dense business screens.
    /// </summary>
    public static class UIColorPalette
    {
        // ===== PRIMARY COLORS =====
        
        /// <summary>
        /// Primary Blue - Base color for primary actions, titles, highlights
        /// </summary>
        public static readonly Color PrimaryBlue = Color.FromArgb(30, 91, 168);  // #1E5BA8
        
        /// <summary>
        /// Primary Blue - Darker variant for hover states
        /// </summary>
        public static readonly Color PrimaryBlueDark = Color.FromArgb(24, 74, 138);  // #184A8A
        
        /// <summary>
        /// Success Green - Base color for positive actions, success messages
        /// </summary>
        public static readonly Color SuccessGreen = Color.FromArgb(22, 163, 74);  // #16A34A
        
        /// <summary>
        /// Success Green - Darker variant for hover states
        /// </summary>
        public static readonly Color SuccessGreenDark = Color.FromArgb(21, 128, 61);  // #15803D
        
        /// <summary>
        /// Warning Orange - Base color for important actions, warnings
        /// </summary>
        public static readonly Color WarningOrange = Color.FromArgb(217, 119, 6);  // #D97706
        
        /// <summary>
        /// Warning Orange - Darker variant for hover states
        /// </summary>
        public static readonly Color WarningOrangeDark = Color.FromArgb(180, 83, 9);  // #B45309
        
        /// <summary>
        /// Danger Red - Base color for destructive actions
        /// </summary>
        public static readonly Color DangerRed = Color.FromArgb(220, 38, 38);  // #DC2626
        
        /// <summary>
        /// Danger Red - Darker variant for hover states
        /// </summary>
        public static readonly Color DangerRedDark = Color.FromArgb(185, 28, 28);  // #B91C1C
        
        
        // ===== BACKGROUND COLORS =====
        
        /// <summary>
        /// Light Gray - Main background color for forms and panels
        /// </summary>
        public static readonly Color BackgroundLight = Color.FromArgb(232, 236, 242);  // #E8ECF2
        
        /// <summary>
        /// White - Background for cards, panels, and content areas
        /// </summary>
        public static readonly Color BackgroundWhite = Color.FromArgb(246, 248, 251);  // #F6F8FB
        public static readonly Color SurfaceMuted = Color.FromArgb(235, 239, 245);  // #EBEFF5
        public static readonly Color Border = Color.FromArgb(204, 213, 225);  // #CCD5E1
        public static readonly Color BorderStrong = Color.FromArgb(170, 184, 202);  // #AAB8CA
        public static readonly Color InputBorder = Color.FromArgb(120, 157, 195);
        public static readonly Color InputFocusBorder = Color.FromArgb(230, 126, 34);
        public static readonly Color InputDisabledBorder = Color.FromArgb(185, 198, 212);
        public static readonly Color InputDisabledBack = Color.FromArgb(226, 232, 240);
        
        
        // ===== TEXT COLORS =====
        
        /// <summary>
        /// Dark Gray - Primary text color for headers and titles
        /// </summary>
        public static readonly Color TextDark = Color.FromArgb(17, 24, 39);  // #111827
        public static readonly Color TextMuted = Color.FromArgb(75, 85, 99);  // #4B5563
        
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
        public static readonly Color StatusError = DangerRed;
        
        /// <summary>
        /// Status - Processing (Blue)
        /// </summary>
        public static readonly Color StatusProcessing = PrimaryBlue;
        
        /// <summary>
        /// Status - Warning (Dark Red for text)
        /// </summary>
        public static readonly Color StatusWarningText = WarningOrangeDark;
        
        /// <summary>
        /// Status - Warning (Light Pink for background)
        /// </summary>
        public static readonly Color StatusWarningBackground = Color.FromArgb(255, 251, 235);
        
        
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
        
        /// <summary>
        /// Button style: Info (Light Yellow) - For view details, information actions
        /// </summary>
        public static class ButtonInfo
        {
            public static readonly Color Base = Color.FromArgb(255, 250, 205);  // Light yellow - #FFFACD
            public static readonly Color Hover = Color.FromArgb(255, 235, 156);  // Darker yellow
            public static readonly Color Text = TextBlack;
        }
        
        /// <summary>
        /// Button style: Action (Light Cyan) - For action buttons like "Hoàn ứng"
        /// </summary>
        public static class ButtonAction
        {
            public static readonly Color Base = Color.FromArgb(224, 255, 255);  // Light cyan - #E0FFFF
            public static readonly Color Hover = Color.FromArgb(175, 238, 238);  // Pale turquoise
            public static readonly Color Text = TextBlack;
        }
    }
}
