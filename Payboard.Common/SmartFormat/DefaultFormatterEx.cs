using System;
using System.Net;
using SmartFormat.Core.Extensions;
using SmartFormat.Core.Output;
using SmartFormat.Core.Parsing;

namespace Payboard.Common.SmartFormat
{
    /// <summary>
    /// ks 3/13/14 - This is more-or-less a copy-paste of the DefaultFormatter from the SmartFormat.NET project.
    /// However, it has two main differences:
    /// (1) It can optionally format as HTML; and
    /// (2) It will throw an exception if the object to be formatted is null
    /// </summary>
    public class DefaultFormatterEx : IFormatter
    {
        public bool FormatAsHtml { get; set; }

        public DefaultFormatterEx(bool formatAsHtml)
        {
            FormatAsHtml = formatAsHtml;
        }

        /// <summary>
        ///     Duplicate the existing formatting code, but HTMLEncode the output before writing it.
        /// </summary>
        /// <remarks>
        ///     This code was basically borrowed from the SmartFormat.Extensions.DefaultFormatter, with a few modifications.
        /// </remarks>
        public void EvaluateFormat(object current, Format format, ref bool handled, IOutput output, FormatDetails formatDetails)
        {
            // If the format has nested placeholders, we process those first 
            // instead of formatting the item:
            if (format != null && format.HasNested)
            {
                formatDetails.Formatter.Format(output, format, current, formatDetails);
                return;
            }

            // ks 3/13/14 - In order to flag to our users when data is missing,
            // if we've got this far in our processing and we still don't have anything we
            // could recognize as data, we need to throw an error.
            if (current == null)
            {
                throw new ArgumentNullException("current");
            }

            // This function always handles the method:
            handled = true;

            //  (The following code was adapted from the built-in String.Format code)

            //  We will try using IFormatProvider, IFormattable, and if all else fails, ToString.
            var formatter = formatDetails.Formatter;
            string result = null;
            ICustomFormatter cFormatter;
            IFormattable formattable;
            // Use the provider to see if a CustomFormatter is available:
            if (formatDetails.Provider != null && (cFormatter = formatDetails.Provider.GetFormat(typeof(ICustomFormatter)) as ICustomFormatter) != null)
            {
                var formatText = format == null ? null : format.GetText();
                result = cFormatter.Format(formatText, current, formatDetails.Provider);
            }
            // IFormattable:
            else if ((formattable = current as IFormattable) != null)
            {
                var formatText = format == null ? null : format.ToString();
                result = formattable.ToString(formatText, formatDetails.Provider);
            }
            // ToString:
            else
            {
                result = current.ToString();
            }

            // ks 10/16/13 - Optionally format the result as HTML
            if (FormatAsHtml)
            {
                result = WebUtility.HtmlEncode(result);
            }

            // Now that we have the result, let's output it (and consider alignment):

            // See if there's a pre-alignment to consider:
            if (formatDetails.Placeholder.Alignment > 0)
            {
                var spaces = formatDetails.Placeholder.Alignment - result.Length;
                if (spaces > 0)
                {
                    output.Write(new String(' ', spaces), formatDetails);
                }
            }

            // Output the result:
            output.Write(result, formatDetails);

            // See if there's a post-alignment to consider:
            if (formatDetails.Placeholder.Alignment < 0)
            {
                var spaces = -formatDetails.Placeholder.Alignment - result.Length;
                if (spaces > 0)
                {
                    output.Write(new String(' ', spaces), formatDetails);
                }
            }
        }
    }
}