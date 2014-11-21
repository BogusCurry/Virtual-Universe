/***************************************************************************
 *	                VIRTUAL REALITY PUBLIC SOURCE LICENSE
 * 
 * Date				: Sun January 1, 2006
 * Copyright		: (c) 2006-2014 by Virtual Reality Development Team. 
 *                    All Rights Reserved.
 * Website			: http://www.syndarveruleiki.is
 *
 * Product Name		: Virtual Reality
 * License Text     : packages/docs/VRLICENSE.txt
 * 
 * Planetary Info   : Information about the Planetary code
 * 
 * Copyright        : (c) 2014-2024 by Second Galaxy Development Team
 *                    All Rights Reserved.
 * 
 * Website          : http://www.secondgalaxy.com
 * 
 * Product Name     : Virtual Reality
 * License Text     : packages/docs/SGLICENSE.txt
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the WhiteCore-Sim Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
***************************************************************************/

using System;
using CSJ2K.j2k.codestream.writer;
using CSJ2K.j2k.wavelet.analysis;
using CSJ2K.j2k.quantization;
using CSJ2K.j2k.wavelet;
using CSJ2K.j2k.encoder;
using CSJ2K.j2k.image;
using CSJ2K.j2k.util;
namespace CSJ2K.j2k.quantization.quantizer
{
	
	/// <summary> This abstract class provides the general interface for quantizers. The
	/// input of a quantizer is the output of a wavelet transform. The output of
	/// the quantizer is the set of quantized wavelet coefficients represented in
	/// sign-magnitude notation (see below).
	/// 
	/// <p>This class provides default implementation for most of the methods
	/// (wherever it makes sense), under the assumption that the image, component
	/// dimensions, and the tiles, are not modifed by the quantizer. If it is not
	/// the case for a particular implementation, then the methods should be
	/// overriden.</p>
	/// 
	/// <p>Sign magnitude representation is used (instead of two's complement) for
	/// the output data. The most significant bit is used for the sign (0 if
	/// positive, 1 if negative). Then the magnitude of the quantized coefficient
	/// is stored in the next M most significat bits. The rest of the bits (least
	/// significant bits) can contain a fractional value of the quantized
	/// coefficient. This fractional value is not to be coded by the entropy
	/// coder. However, it can be used to compute rate-distortion measures with
	/// greater precision.</p>
	/// 
	/// <p>The value of M is determined for each subband as the sum of the number
	/// of guard bits G and the nominal range of quantized wavelet coefficients in
	/// the corresponding subband (Rq), minus 1:</p>
	/// 
	/// <p>M = G + Rq -1</p>
	/// 
	/// <p>The value of G should be the same for all subbands. The value of Rq
	/// depends on the quantization step size, the nominal range of the component
	/// before the wavelet transform and the analysis gain of the subband (see
	/// Subband).</p>
	/// 
	/// <p>The blocks of data that are requested should not cross subband
	/// boundaries.</p>
	/// 
	/// <p>NOTE: At the moment only quantizers that implement the
	/// 'CBlkQuantDataSrcEnc' interface are supported.</p>
	/// 
	/// </summary>
	/// <seealso cref="Subband">
	/// 
	/// </seealso>
	public abstract class Quantizer:ImgDataAdapter, CBlkQuantDataSrcEnc
	{
		/// <summary> Returns the horizontal offset of the code-block partition. Allowable
		/// values are 0 and 1, nothing else.
		/// 
		/// </summary>
		virtual public int CbULX
		{
			get
			{
				return src.CbULX;
			}
			
		}
		/// <summary> Returns the vertical offset of the code-block partition. Allowable
		/// values are 0 and 1, nothing else.
		/// 
		/// </summary>
		virtual public int CbULY
		{
			get
			{
				return src.CbULY;
			}
			
		}
		/// <summary> Returns the parameters that are used in this class and implementing
		/// classes. It returns a 2D String array. Each of the 1D arrays is for a
		/// different option, and they have 3 elements. The first element is the
		/// option name, the second one is the synopsis, the third one is a long
		/// description of what the parameter is and the fourth is its default
		/// value. The synopsis or description may be 'null', in which case it is
		/// assumed that there is no synopsis or description of the option,
		/// respectively. Null may be returned if no options are supported.
		/// 
		/// </summary>
		/// <returns> the options name, their synopsis and their explanation, 
		/// or null if no options are supported.
		/// 
		/// </returns>
		public static System.String[][] ParameterInfo
		{
			get
			{
				return pinfo;
			}
			
		}
		
		/// <summary>The prefix for quantizer options: 'Q' </summary>
		public const char OPT_PREFIX = 'Q';
		
		/// <summary>The list of parameters that is accepted for quantization. Options 
		/// for quantization start with 'Q'. 
		/// </summary>
		//UPGRADE_NOTE: Final was removed from the declaration of 'pinfo'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		private static readonly System.String[][] pinfo = new System.String[][]{new System.String[]{"Qtype", "[<tile-component idx>] <id> " + "[ [<tile-component idx>] <id> ...]", "Specifies which quantization type to use for specified " + "tile-component. The default type is either 'reversible' or " + "'expounded' depending on whether or not the '-lossless' option " + " is specified.\n" + "<tile-component idx> : see general note.\n" + "<id>: Supported quantization types specification are : " + "'reversible' " + "(no quantization), 'derived' (derived quantization step size) and " + "'expounded'.\n" + "Example: -Qtype reversible or -Qtype t2,4-8 c2 reversible t9 " + "derived.", null}, new System.String[]{"Qstep", "[<tile-component idx>] <bnss> " + "[ [<tile-component idx>] <bnss> ...]", "This option specifies the base normalized quantization step " + "size (bnss) for tile-components. It is normalized to a " + "dynamic range of 1 in the image domain. This parameter is " + "ignored in reversible coding. The default value is '1/128'" + " (i.e. 0.0078125).", "0.0078125"}, new System.String[]{"Qguard_bits", "[<tile-component idx>] <gb> " + "[ [<tile-component idx>] <gb> ...]", "The number of bits used for each tile-component in the quantizer" + " to avoid overflow (gb).", "2"}};
		
		/// <summary>The source of wavelet transform coefficients </summary>
		protected internal CBlkWTDataSrc src;
		
		/// <summary> Initializes the source of wavelet transform coefficients.
		/// 
		/// </summary>
		/// <param name="src">The source of wavelet transform coefficients.
		/// 
		/// </param>
		public Quantizer(CBlkWTDataSrc src):base(src)
		{
			this.src = src;
		}
		
		/// <summary> Returns the number of guard bits used by this quantizer in the
		/// given tile-component.
		/// 
		/// </summary>
		/// <param name="t">Tile index
		/// 
		/// </param>
		/// <param name="c">Component index
		/// 
		/// </param>
		/// <returns> The number of guard bits
		/// 
		/// </returns>
		public abstract int getNumGuardBits(int t, int c);
		
		/// <summary> Returns true if the quantizer of given tile-component uses derived
		/// quantization step sizes.
		/// 
		/// </summary>
		/// <param name="t">Tile index
		/// 
		/// </param>
		/// <param name="c">Component index
		/// 
		/// </param>
		/// <returns> True if derived quantization is used.
		/// 
		/// </returns>
		public abstract bool isDerived(int t, int c);
		
		/// <summary> Calculates the parameters of the SubbandAn objects that depend on the
		/// Quantizer. The 'stepWMSE' field is calculated for each subband which is
		/// a leaf in the tree rooted at 'sb', for the specified component. The
		/// subband tree 'sb' must be the one for the component 'n'.
		/// 
		/// </summary>
		/// <param name="sb">The root of the subband tree.
		/// 
		/// </param>
		/// <param name="n">The component index.
		/// 
		/// </param>
		/// <seealso cref="SubbandAn.stepWMSE">
		/// 
		/// </seealso>
		protected internal abstract void  calcSbParams(SubbandAn sb, int n);
		
		/// <summary> Returns a reference to the subband tree structure representing the
		/// subband decomposition for the specified tile-component.
		/// 
		/// <P>This method gets the subband tree from the source and then
		/// calculates the magnitude bits for each leaf using the method
		/// calcSbParams().
		/// 
		/// </summary>
		/// <param name="t">The index of the tile.
		/// 
		/// </param>
		/// <param name="c">The index of the component.
		/// 
		/// </param>
		/// <returns> The subband tree structure, see SubbandAn.
		/// 
		/// </returns>
		/// <seealso cref="SubbandAn">
		/// 
		/// </seealso>
		/// <seealso cref="Subband">
		/// 
		/// </seealso>
		/// <seealso cref="calcSbParams">
		/// 
		/// </seealso>
		public virtual SubbandAn getAnSubbandTree(int t, int c)
		{
			SubbandAn sbba;
			
			// Ask for the wavelet tree of the source
			sbba = src.getAnSubbandTree(t, c);
			// Calculate the stepWMSE
			calcSbParams(sbba, c);
			return sbba;
		}
		
		/// <summary> Creates a Quantizer object for the appropriate type of quantization
		/// specified in the options in the parameter list 'pl', and having 'src'
		/// as the source of data to be quantized. The 'rev' flag indicates if the
		/// quantization should be reversible.
		/// 
		/// NOTE: At the moment only sources of wavelet data that implement the
		/// 'CBlkWTDataSrc' interface are supported.
		/// 
		/// </summary>
		/// <param name="src">The source of data to be quantized
		/// 
		/// </param>
		/// <param name="encSpec">Encoder specifications
		/// 
		/// </param>
		/// <exception cref="IllegalArgumentException">If an error occurs while parsing
		/// the options in 'pl'
		/// 
		/// </exception>
		public static Quantizer createInstance(CBlkWTDataSrc src, EncoderSpecs encSpec)
		{
			// Instantiate quantizer
			return new StdQuantizer(src, encSpec);
		}
		
		/// <summary> Returns the maximum number of magnitude bits in any subband in the
		/// current tile.
		/// 
		/// </summary>
		/// <param name="c">the component number
		/// 
		/// </param>
		/// <returns> The maximum number of magnitude bits in all subbands of the
		/// current tile.
		/// 
		/// </returns>
		public abstract int getMaxMagBits(int c);
		public abstract CSJ2K.j2k.wavelet.analysis.CBlkWTData getNextInternCodeBlock(int param1, CSJ2K.j2k.wavelet.analysis.CBlkWTData param2);
		public abstract CSJ2K.j2k.wavelet.analysis.CBlkWTData getNextCodeBlock(int param1, CSJ2K.j2k.wavelet.analysis.CBlkWTData param2);
		public abstract bool isReversible(int param1, int param2);
	}
}