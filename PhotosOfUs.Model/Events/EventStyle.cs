using Kuvio.Kernel.Core.Common;
using PhotosOfUs.Core.Photos;
using PhotosOfUs.Core.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace PhotosOfUs.Core.Events
{
    public class EventStyle : ValueObject<EventStyle>
    {
        public string PersonalLogoUrl { get; protected set; }
        public string FeaturedImageUrl { get; protected set; }
        public string OverlayColorCode { get; protected set; }
        private decimal overlayOpacity;

        public EventStyle(string personalLogoUrl, string featuredImageUrl, string overlayColorCode, decimal overlayOpacity, 
            string accentColorCode, string backgroundColorCode, string headerColorCode, string bodyColorCode, SeparatorStyle separatorStyle, 
            int separatorThickness, int separatorWidth, HomepageTemplate homepageTemplate)// ThemeColor themeColor , HomepageTemplate homepageTemplate
        {
            PersonalLogoUrl = personalLogoUrl ?? throw new ArgumentNullException(nameof(personalLogoUrl));
            FeaturedImageUrl = featuredImageUrl ?? throw new ArgumentNullException(nameof(featuredImageUrl));
            OverlayColorCode = overlayColorCode ?? throw new ArgumentNullException(nameof(overlayColorCode));
            OverlayOpacity = overlayOpacity;
            AccentColorCode = accentColorCode ?? throw new ArgumentNullException(nameof(accentColorCode));
            BackgroundColorCode = backgroundColorCode ?? throw new ArgumentNullException(nameof(backgroundColorCode));
            HeaderColorCode = headerColorCode ?? throw new ArgumentNullException(nameof(headerColorCode));
            BodyColorCode = bodyColorCode ?? throw new ArgumentNullException(nameof(bodyColorCode));
            SeparatorStyle = separatorStyle ?? throw new ArgumentNullException(nameof(separatorStyle));
            SeparatorThickness = separatorThickness;
            SeparatorWidth = separatorWidth;
            //ThemeColor = themeColor ?? throw new ArgumentNullException(nameof(themeColor));
            HomepageTemplate = homepageTemplate ?? throw new ArgumentNullException(nameof(homepageTemplate));
        }

        public decimal OverlayOpacity
        {
            get => overlayOpacity; protected set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException();
                }

                overlayOpacity = value;
            }
        }

        //TODO: Add validation for color codes
        public string AccentColorCode { get; protected set; }
        public string BackgroundColorCode { get; protected set; }
        public string HeaderColorCode { get; protected set; }
        public string BodyColorCode { get; protected set; }
        public SeparatorStyle SeparatorStyle { get; protected set; }
        public int SeparatorThickness { get; protected set; }
        public int SeparatorWidth { get; protected set; }
        //public ThemeColor ThemeColor { get; protected set; }
        public HomepageTemplate HomepageTemplate { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return AccentColorCode;
            yield return BackgroundColorCode;
            yield return HeaderColorCode;
            yield return BodyColorCode;
            yield return SeparatorStyle;
            yield return SeparatorThickness;
            yield return SeparatorWidth;
            //yield return ThemeColor;
        }

    }

    public class HomepageTemplate : ValueObject<HomepageTemplate>
    {
        public HomepageTemplate(string value, string text) { Value = value; Text = text; }

        public string Value { get; private set; }
        public string Text { get; private set; }

        public static HomepageTemplate TwoOneSplit { get { return new HomepageTemplate("TwoOneSplit", "Two One Split"); } }
        public static HomepageTemplate OneTwoSplit { get { return new HomepageTemplate("OneTwoSplit", "One Two Split"); } }
        public static HomepageTemplate FullBackground { get { return new HomepageTemplate("FullBackground", "Full Background"); } }


        public static List<HomepageTemplate> GetAll()
        {
            var list = new List<HomepageTemplate>();
            list.Add(TwoOneSplit);
            list.Add(OneTwoSplit);
            list.Add(FullBackground);
            return list;
        }

        public static HomepageTemplate Get(string value)
        {
            var list = GetAll();
            return list.First(y => y.Value == value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

    }

    public class SeparatorStyle : ValueObject<SeparatorStyle>
    {
        private SeparatorStyle(string value, string text) { Value = value; Text = text; }

        public string Value { get; private set; }
        public string Text { get; private set; }

        public static SeparatorStyle StraightLine { get { return new SeparatorStyle("StraightLine", "Straight Line"); } }
        public static SeparatorStyle DottedLine { get { return new SeparatorStyle("DottedLine", "Dotted Line"); } }


        public static List<SeparatorStyle> GetAll()
        {
            var list = new List<SeparatorStyle>();
            list.Add(StraightLine);
            list.Add(DottedLine);
            return list;
        }

        public static SeparatorStyle Get(string value)
        {
            var list = GetAll();
            return list.First(y => y.Value == value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

    }

    public class ThemeColor : ValueObject<ThemeColor>
    {
        private ThemeColor(string value, string text) { Value = value; Text = text; }

        public string Value { get; private set; }
        public string Text { get; private set; }

        public static ThemeColor Light { get { return new ThemeColor("Light", "Light"); } }
        public static ThemeColor Dark { get { return new ThemeColor("Dark", "Dark"); } }


        public static List<ThemeColor> GetAll()
        {
            var list = new List<ThemeColor>();
            list.Add(Light);
            list.Add(Dark);
            return list;
        }

        public static ThemeColor Get(string value)
        {
            var list = GetAll();
            return list.First(y => y.Value == value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

    }
}
