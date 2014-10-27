using System.Collections.Generic;
using SmartFormat;
using SmartFormat.Core.Extensions;
using SmartFormat.Core.Parsing;

namespace Payboard.Common.SmartFormat
{
    /// <summary>
    ///     Modeled after the DictionarySource class from SmartFormat, but allows for expando objects and/or generic
    ///     IDictionary objects.
    /// </summary>
    public class GenericDictionarySource : ISource
    {
        public void EvaluateSelector(object current, Selector selector, ref bool handled, ref object result,
            FormatDetails formatDetails)
        {
            // Only makes sense to check if current is an IDictionary<string, object>
            var genericDictionary = current as IDictionary<string, object>;
            if (genericDictionary != null)
            {
                // First, try to see if we can find it using the normal casing.
                if (!genericDictionary.TryGetValue(selector.Text, out result))
                {
                    // If we can't find it with normal casing, try to find it in all lower case
                    var selectorTextLower = selector.Text.ToLowerInvariant();
                    if (!genericDictionary.TryGetValue(selectorTextLower, out result))
                    {
                        // result = new MissingDictionarySourceResult();
                    }
                }
                handled = true;
            }
        }
    }

    /// <summary>
    /// Represents a null dictionary source result, so that we can handle these things in the ConditionalFormatterEx class rather than here.
    /// </summary>
    public class MissingDictionarySourceResult
    {
        public override string ToString()
        {
            return "";
        }
    }
}