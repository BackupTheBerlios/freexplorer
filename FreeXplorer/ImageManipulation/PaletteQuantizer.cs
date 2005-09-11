/* 
  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF 
  ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
  THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
  PARTICULAR PURPOSE. 
  
    Original code modified by Olivier Marcoux for Freebox specific palette handling
*/ 

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace ImageManipulation
{
	/// <summary>
	/// Summary description for PaletteQuantizer.
	/// </summary>
	public unsafe class PaletteQuantizer : Quantizer
	{
		/// <summary>
		/// Construct the palette quantizer
		/// </summary>
		/// <param name="palette">The color palette to quantize to</param>
		/// <remarks>
		/// Palette quantization only requires a single quantization step
		/// </remarks>
		public PaletteQuantizer ( ArrayList palette ) : base ( true )
		{
			_colorMap = new Hashtable ( ) ;

			_colors = new Color[palette.Count] ;
			palette.CopyTo ( _colors ) ;
		}

		/// <summary>
		/// Override this to process the pixel in the second pass of the algorithm
		/// </summary>
		/// <param name="pixel">The pixel to quantize</param>
		/// <returns>The quantized value</returns>
		protected override byte QuantizePixel ( Color32* pixel )
		{
			byte	colorIndex = 0 ;
			int		colorHash = pixel->ARGB ;	

			// Check if the color is in the lookup table
			if ( _colorMap.ContainsKey ( colorHash ) )
				colorIndex = (byte)_colorMap[colorHash] ;
			else
			{
				// Not found - loop through the palette and find the nearest match.
				// Firstly check the alpha value - if < 128, set the transparent color
				if ( pixel->Alpha < 128 )
                    colorIndex = 0; // color 0 is transparent for Freebox
				else
				{
					// Not transparent...
					int	leastDistance = int.MaxValue ;
					int red = pixel->Red ;
					int green = pixel->Green;
					int blue = pixel->Blue;

					// Loop through the freebox palette visible colors, looking for the closest color match
					for ( int index = 1 ; index < 192 ; index++ )
					{
						Color	paletteColor = _colors[index];
						
						int	redDistance = paletteColor.R - red ;
						int	greenDistance = paletteColor.G - green ;
						int	blueDistance = paletteColor.B - blue ;

						int		distance = ( redDistance * redDistance ) + 
										   ( greenDistance * greenDistance ) + 
										   ( blueDistance * blueDistance ) ;

						if ( distance < leastDistance )
						{
							colorIndex = (byte)index ;
							leastDistance = distance ;

							// And if it's an exact match, exit the loop
							if ( 0 == distance )
								break ;
						}
					}
				}

				// Now I have the color, pop it into the hashtable for next time
				_colorMap.Add ( colorHash , colorIndex ) ;
			}

			return colorIndex ;
		}

		/// <summary>
		/// Retrieve the palette for the quantized image
		/// </summary>
		/// <param name="palette">Any old palette, this is overrwritten</param>
		/// <returns>The new color palette</returns>
		protected override ColorPalette GetPalette ( ColorPalette palette )
		{
			for ( int index = 0 ; index < _colors.Length ; index++ )
				palette.Entries[index] = _colors[index] ;

			return palette ;
		}

		/// <summary>
		/// Lookup table for colors
		/// </summary>
		private Hashtable	_colorMap ;

		/// <summary>
		/// List of all colors in the palette
		/// </summary>
		private Color[]		_colors ;
	}
}
