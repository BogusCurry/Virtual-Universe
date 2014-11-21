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
using CSJ2K.j2k.util;
using CSJ2K.j2k.io;
using CSJ2K.Color;
using CSJ2K.Icc;
namespace CSJ2K.Color.Boxes
{
	
	/// <summary> This class models the Color Specification Box in a JP2 image.
	/// 
	/// </summary>
	/// <version> 	1.0
	/// </version>
	/// <author> 	Bruce A. Kern
	/// </author>
	public sealed class ColorSpecificationBox:JP2Box
	{
        
		public ColorSpace.MethodEnum Method
		{
			// Return an enumeration for the colorspace method. 
			
			get
			{
				return method;
			}
			
		}
		public ColorSpace.CSEnum ColorSpace
		{
			// Return an enumeration for the colorspace. 
			
			get
			{
				return colorSpace;
			}
			
		}
        
		public System.String ColorSpaceString
		{
			// Return a String representation of the colorspace. 
			
			get
			{
				return colorSpace.ToString();
			}
			
		}
		public System.String MethodString
		{
			// Return a String representation of the colorspace method. 
			
			get
			{
				return method.ToString();
			}
			
		}
        
		public byte[] ICCProfile
		{
			/* Retrieve the ICC Profile from the image as a byte []. */
			
			get
			{
				return iccProfile;
			}
			
		}
		
		private ColorSpace.MethodEnum method;
		private ColorSpace.CSEnum colorSpace;
		private byte[] iccProfile = null;
		
		/// <summary> Construct a ColorSpecificationBox from an input image.</summary>
		/// <param name="in">RandomAccessIO jp2 image
		/// </param>
		/// <param name="boxStart">offset to the start of the box in the image
		/// </param>
		/// <exception cref="IOException,">ColorSpaceException 
		/// 
		/// </exception>
		public ColorSpecificationBox(RandomAccessIO in_Renamed, int boxStart):base(in_Renamed, boxStart)
		{
			readBox();
		}
		
		/// <summary>Analyze the box content. </summary>
		private void  readBox()
		{
			byte[] boxHeader = new byte[256];
			in_Renamed.seek(dataStart);
			in_Renamed.readFully(boxHeader, 0, 11);
			switch (boxHeader[0])
			{
				
				case 1: 
					method = CSJ2K.Color.ColorSpace.MethodEnum.ENUMERATED;
                    int cs = CSJ2K.Icc.ICCProfile.getInt(boxHeader, 3);
					switch (cs)
					{
						case 16: 
							colorSpace = CSJ2K.Color.ColorSpace.CSEnum.sRGB;
							break; // from switch (cs)...
						
						case 17: 
							colorSpace = CSJ2K.Color.ColorSpace.CSEnum.GreyScale;
							break; // from switch (cs)...
						
						case 18: 
							colorSpace = CSJ2K.Color.ColorSpace.CSEnum.sYCC;
							break; // from switch (cs)...
                        case 20:
                            colorSpace = CSJ2K.Color.ColorSpace.CSEnum.esRGB;
                            break;

                        #region Known but unsupported colorspaces
                        case 3:
                            FacilityManager.getMsgLogger().printmsg(CSJ2K.j2k.util.MsgLogger_Fields.WARNING, "Unsupported enumerated colorspace YCbCr(2) in color specification box");
                            colorSpace = CSJ2K.Color.ColorSpace.CSEnum.Unknown;
                            break;
                        case 4:
                            FacilityManager.getMsgLogger().printmsg(CSJ2K.j2k.util.MsgLogger_Fields.WARNING, "Unsupported enumerated colorspace YCbCr(3) in color specification box");
                            colorSpace = CSJ2K.Color.ColorSpace.CSEnum.Unknown;
                            break;
                        case 9:
                            FacilityManager.getMsgLogger().printmsg(CSJ2K.j2k.util.MsgLogger_Fields.WARNING, "Unsupported enumerated colorspace PhotoYCC in color specification box");
                            colorSpace = CSJ2K.Color.ColorSpace.CSEnum.Unknown;
                            break;
                        case 11:
                            FacilityManager.getMsgLogger().printmsg(CSJ2K.j2k.util.MsgLogger_Fields.WARNING, "Unsupported enumerated colorspace CMY in color specification box");
                            colorSpace = CSJ2K.Color.ColorSpace.CSEnum.Unknown;
                            break;
                        case 12:
                            FacilityManager.getMsgLogger().printmsg(CSJ2K.j2k.util.MsgLogger_Fields.WARNING, "Unsupported enumerated colorspace CMYK in color specification box");
                            colorSpace = CSJ2K.Color.ColorSpace.CSEnum.Unknown;
                            break;
                        case 13:
                            FacilityManager.getMsgLogger().printmsg(CSJ2K.j2k.util.MsgLogger_Fields.WARNING, "Unsupported enumerated colorspace YCCK in color specification box");
                            colorSpace = CSJ2K.Color.ColorSpace.CSEnum.Unknown;
                            break;
                        case 14:
                            FacilityManager.getMsgLogger().printmsg(CSJ2K.j2k.util.MsgLogger_Fields.WARNING, "Unsupported enumerated colorspace CIELab in color specification box");
                            colorSpace = CSJ2K.Color.ColorSpace.CSEnum.Unknown;
                            break;
                        case 15:
                            FacilityManager.getMsgLogger().printmsg(CSJ2K.j2k.util.MsgLogger_Fields.WARNING, "Unsupported enumerated colorspace Bi-Level(2) in color specification box");
                            colorSpace = CSJ2K.Color.ColorSpace.CSEnum.Unknown;
                            break;
                        case 19:
                            FacilityManager.getMsgLogger().printmsg(CSJ2K.j2k.util.MsgLogger_Fields.WARNING, "Unsupported enumerated colorspace CIEJab in color specification box");
                            colorSpace = CSJ2K.Color.ColorSpace.CSEnum.Unknown;
                            break;
                        case 21:
                            FacilityManager.getMsgLogger().printmsg(CSJ2K.j2k.util.MsgLogger_Fields.WARNING, "Unsupported enumerated colorspace ROMM-RGB in color specification box");
                            colorSpace = CSJ2K.Color.ColorSpace.CSEnum.Unknown;
                            break;
                        case 22:
                            FacilityManager.getMsgLogger().printmsg(CSJ2K.j2k.util.MsgLogger_Fields.WARNING, "Unsupported enumerated colorspace YPbPr(1125/60) in color specification box");
                            colorSpace = CSJ2K.Color.ColorSpace.CSEnum.Unknown;
                            break;
                        case 23:
                            FacilityManager.getMsgLogger().printmsg(CSJ2K.j2k.util.MsgLogger_Fields.WARNING, "Unsupported enumerated colorspace YPbPr(1250/50) in color specification box");
                            colorSpace = CSJ2K.Color.ColorSpace.CSEnum.Unknown;
                            break;
                        case 24:
                            FacilityManager.getMsgLogger().printmsg(CSJ2K.j2k.util.MsgLogger_Fields.WARNING, "Unsupported enumerated colorspace e-sYCC in color specification box");
                            colorSpace = CSJ2K.Color.ColorSpace.CSEnum.Unknown;
                            break;
                        #endregion

                        default: 
							FacilityManager.getMsgLogger().printmsg(CSJ2K.j2k.util.MsgLogger_Fields.WARNING, "Unknown enumerated colorspace (" + cs + ") in color specification box");
							colorSpace = CSJ2K.Color.ColorSpace.CSEnum.Unknown;
							break;
						
					}
					break; // from switch (boxHeader[0])...
				
				case 2: 
					method = CSJ2K.Color.ColorSpace.MethodEnum.ICC_PROFILED;
                    int size = CSJ2K.Icc.ICCProfile.getInt(boxHeader, 3);
					iccProfile = new byte[size];
					in_Renamed.seek(dataStart + 3);
					in_Renamed.readFully(iccProfile, 0, size);
					break; // from switch (boxHeader[0])...
				
				default: 
					throw new ColorSpaceException("Bad specification method (" + boxHeader[0] + ") in " + this);
           			
			}
		}
		
		/// <summary>Return a suitable String representation of the class instance. </summary>
		public override System.String ToString()
		{
			System.Text.StringBuilder rep = new System.Text.StringBuilder("[ColorSpecificationBox ");
			rep.Append("method= ").Append(System.Convert.ToString(method)).Append(", ");
			rep.Append("colorspace= ").Append(System.Convert.ToString(colorSpace)).Append("]");
			return rep.ToString();
		}
		
		/* end class ColorSpecificationBox */
		static ColorSpecificationBox()
		{
			{
				type = 0x636f6c72;
			}
		}
	}
}