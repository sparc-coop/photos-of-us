﻿using System;
using System.Text;

using Microsoft.AspNetCore.Mvc;

using Rotativa.NetCore.Options;

namespace Rotativa.NetCore
{
    public abstract class AsPdfResultBase : AsResultBase
    {
        protected AsPdfResultBase()
        {
            this.PageMargins = new Margins();
        }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        [OptionFlag("-s")]
        public Size? PageSize { get; set; }

        /// <summary>
        /// Gets or sets the page width in mm.
        /// </summary>
        /// <remarks>Has priority over <see cref="PageSize"/> but <see cref="PageHeight"/> has to be also specified.</remarks>
        [OptionFlag("--page-width")]
        public double? PageWidth { get; set; }

        /// <summary>
        /// Gets or sets the page height in mm.
        /// </summary>
        /// <remarks>Has priority over <see cref="PageSize"/> but <see cref="PageWidth"/> has to be also specified.</remarks>
        [OptionFlag("--page-height")]
        public double? PageHeight { get; set; }

        /// <summary>
        /// Gets or sets the page orientation.
        /// </summary>
        [OptionFlag("-O")]
        public Orientation? PageOrientation { get; set; }

        /// <summary>
        /// Gets or sets the page margins.
        /// </summary>
        public Margins PageMargins { get; set; }

        /// <summary>
        /// Gets or sets Path to wkhtmltopdf binary.
        /// </summary>
        [Obsolete("Use WkhtmlPath instead of CookieName.", false)]
        public string WkhtmltopdfPath
        {
            get => this.WkhtmlPath;

            set => this.WkhtmlPath = value;
        }

        /// <summary>
        /// Indicates whether the PDF should be generated in lower quality.
        /// </summary>
        [OptionFlag("-l")]
        public bool IsLowQuality { get; set; }

        /// <summary>
        /// Number of copies to print into the PDF file.
        /// </summary>
        [OptionFlag("--copies")]
        public int? Copies { get; set; }

        /// <summary>
        /// Indicates whether the PDF should be generated in grayscale.
        /// </summary>
        [OptionFlag("-g")]
        public bool IsGrayScale { get; set; }

        protected override byte[] WkhtmlConvert(string switches)
        {
            return WkhtmltopdfDriver.Convert(this.WkhtmlPath, switches);
        }

        protected override string GetContentType()
        {
            return "application/pdf";
        }

        protected override string GetConvertOptions()
        {
            var result = new StringBuilder();

            if (this.PageMargins != null)
            {
                result.Append(this.PageMargins);
            }

            result.Append(" ");
            result.Append(base.GetConvertOptions());

            return result.ToString().Trim();
        }

        [Obsolete(@"Use BuildFile(this.ControllerContext) method instead.")]
        public byte[] BuildPdf(ControllerContext context)
        {
            return this.BuildFile(context);
        }
    }
}