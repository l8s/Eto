using System;
using System.ComponentModel;
using Eto.Drawing;
using System.Globalization;

namespace Eto.Drawing
{
	/// <summary>
	/// Converts instances of other types to and from a <see cref="Font"/>.
	/// </summary>
	/// <remarks>
    /// This only supports converting from a string supported by the <see cref="Font.TryParse"/> method.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class FontConverter : TypeConverter
	{
		/// <summary>
		/// Determines if this can convert a <see cref="Font"/> to the <paramref name="destinationType"/>
		/// </summary>
		/// <param name="context">Context of the conversion</param>
		/// <param name="destinationType">Type to convert to</param>
		/// <returns>True if this converter supports the <paramref name="destinationType"/>, false otherwise</returns>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}

		/// <summary>
		/// Determines if this can convert a value with the type of <paramref name="sourceType"/> to a <see cref="Font"/>
		/// </summary>
		/// <param name="context">Context of the conversion</param>
		/// <param name="sourceType">Type to convert from</param>
		/// <returns>True if this can convert to the <paramref name="sourceType"/>, false otherwise</returns>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
            if (sourceType == typeof(string))
                return true;
			return base.CanConvertFrom(context, sourceType);
		}

		/// <summary>
		/// Converts the <paramref name="value"/> to an instance of a <see cref="Font"/>
		/// </summary>
        /// <remarks>
        /// The string can be any of these formats:
        ///		- family
        ///		- family,size
        ///		- family,size,style
        ///		- family,size,style,decoration
        /// </remarks>
		/// <param name="context">Context of the conversion</param>
		/// <param name="culture">Culture to use for the conversion</param>
		/// <param name="value">Value to convert</param>
		/// <returns>A <see cref="Color"/> instance with the converted value</returns>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			var str = value as string;
			if (str != null)
			{
                string[] strs = str.Split(',');                
                FontFamily family = new FontFamily(strs[0]);
                float size = strs.Length > 1 ? Single.Parse(strs[1]) : 12.0f;

                FontStyle style = FontStyle.None;
                if (strs.Length > 2)
                {
                    string[] styles = strs[2].Split('+');
                    foreach (string stylestr in styles)
                    {
                        style |= (FontStyle)Enum.Parse(typeof(FontStyle), stylestr);
                    }
                }

                FontDecoration decoration = FontDecoration.None;
                if (strs.Length > 3)
                {
                    string[] decorations = strs[3].Split('+');
                    foreach (string decorationstr in decorations)
                    {
                        decoration |= (FontDecoration)Enum.Parse(typeof(FontDecoration), decorationstr);
                    }
                }

                return Fonts.Cached(family, size, style, decoration);
			}
			return base.ConvertFrom(context, culture, value);
		}

		/// <summary>
		/// Converts a <see cref="Font"/> instance to the specified <paramref name="destinationType"/>
		/// </summary>
		/// <param name="context">Context of the conversion</param>
		/// <param name="culture">Culture to use for the conversion</param>
        /// <param name="value"><see cref="Font"/> value to convert</param>
		/// <param name="destinationType">Type to convert the <paramref name="value"/> to</param>
		/// <returns>An object of type <paramref name="destinationType"/> converted from <paramref name="value"/></returns>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
            throw new NotImplementedException();
		}
	}
}

