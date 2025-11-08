using System.Drawing;

namespace QLVT.Utils
{
    /// <summary>
    /// Centralized font definitions for consistent typography across the application
    /// Using traditional Arial font for better readability
    /// </summary>
    public static class UIFonts
    {
        /// <summary>
        /// Primary font family used throughout the application (Traditional)
        /// </summary>
        public const string FontFamily = "Arial";
        
        
        // ===== FONT SIZES (Increased by 1pt for better readability) =====
        
        /// <summary>
        /// Extra Large - For main titles and headers (17pt, Bold)
        /// </summary>
        public static readonly Font TitleLarge = new Font(FontFamily, 17F, FontStyle.Bold);
        
        /// <summary>
        /// Large - For section headers (13pt, Bold)
        /// </summary>
        public static readonly Font TitleMedium = new Font(FontFamily, 13F, FontStyle.Bold);
        
        /// <summary>
        /// Standard - For section headers in GroupBox (11pt, Bold)
        /// </summary>
        public static readonly Font HeaderStandard = new Font(FontFamily, 11F, FontStyle.Bold);
        
        /// <summary>
        /// Standard - For buttons (10.5pt, Bold)
        /// </summary>
        public static readonly Font ButtonStandard = new Font(FontFamily, 10.5F, FontStyle.Bold);
        
        /// <summary>
        /// Standard - For labels, textboxes, regular text (10.5pt, Regular)
        /// </summary>
        public static readonly Font TextStandard = new Font(FontFamily, 10.5F, FontStyle.Regular);
        
        /// <summary>
        /// Standard - For DataGridView data (10.5pt, Regular) - KHÔNG in đậm
        /// </summary>
        public static readonly Font GridData = new Font(FontFamily, 10.5F, FontStyle.Regular);
        
        /// <summary>
        /// Small - For status messages, hints (10pt, Regular)
        /// </summary>
        public static readonly Font TextSmall = new Font(FontFamily, 10F, FontStyle.Regular);
        
        
        // ===== HELPER METHODS =====
        
        /// <summary>
        /// Create a custom font with specified size and style
        /// </summary>
        public static Font CreateFont(float size, FontStyle style = FontStyle.Regular)
        {
            return new Font(FontFamily, size, style);
        }
        
        /// <summary>
        /// Create a bold variant of a font
        /// </summary>
        public static Font CreateBoldFont(float size)
        {
            return new Font(FontFamily, size, FontStyle.Bold);
        }
        
        /// <summary>
        /// Create an italic variant of a font
        /// </summary>
        public static Font CreateItalicFont(float size)
        {
            return new Font(FontFamily, size, FontStyle.Italic);
        }
    }
}
