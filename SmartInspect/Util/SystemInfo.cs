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
using System.Configuration;
using System.Reflection;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;

namespace SmartInspect.Util
{
	/// <summary>
	/// Utility class for system specific information.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Utility class of static methods for system specific information.
	/// </para>
	/// </remarks>
	/// <author>Nicko Cadell</author>
	/// <author>Gert Driesen</author>
	/// <author>Alexey Solofnenko</author>
	public sealed class SystemInfo
	{
		#region Private Constants

		private const string DEFAULT_NULL_TEXT = "(null)";
		private const string DEFAULT_NOT_AVAILABLE_TEXT = "NOT AVAILABLE";

		#endregion

		#region Private Instance Constructors

		/// <summary>
		/// Private constructor to prevent instances.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Only static methods are exposed from this type.
		/// </para>
		/// </remarks>
		private SystemInfo() 
		{
		}

		#endregion Private Instance Constructors

		#region Public Static Constructor

		/// <summary>
		/// Initialize default values for private static fields.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Only static methods are exposed from this type.
		/// </para>
		/// </remarks>
		static SystemInfo()
		{
			string nullText = DEFAULT_NULL_TEXT;
			string notAvailableText = DEFAULT_NOT_AVAILABLE_TEXT;

#if !NETCF
			// Look for SmartInspect.NullText in AppSettings
			string nullTextAppSettingsKey = SystemInfo.GetAppSetting("SmartInspect.NullText");
			if (nullTextAppSettingsKey != null && nullTextAppSettingsKey.Length > 0)
			{
				LogLog.Debug(declaringType, "Initializing NullText value to [" + nullTextAppSettingsKey + "].");
				nullText = nullTextAppSettingsKey;
			}

			// Look for SmartInspect.NotAvailableText in AppSettings
			string notAvailableTextAppSettingsKey = SystemInfo.GetAppSetting("SmartInspect.NotAvailableText");
			if (notAvailableTextAppSettingsKey != null && notAvailableTextAppSettingsKey.Length > 0)
			{
				LogLog.Debug(declaringType, "Initializing NotAvailableText value to [" + notAvailableTextAppSettingsKey + "].");
				notAvailableText = notAvailableTextAppSettingsKey;
			}
#endif
			s_notAvailableText = notAvailableText;
			s_nullText = nullText;
		}

		#endregion

		#region Public Static Properties

		/// <summary>
		/// Gets the system dependent line terminator.
		/// </summary>
		/// <value>
		/// The system dependent line terminator.
		/// </value>
		/// <remarks>
		/// <para>
		/// Gets the system dependent line terminator.
		/// </para>
		/// </remarks>
		public static string NewLine
		{
			get
			{
#if NETCF
				return "\r\n";
#else
				return System.Environment.NewLine;
#endif
			}
		}

		/// <summary>
		/// Gets the base directory for this <see cref="AppDomain"/>.
		/// </summary>
		/// <value>The base directory path for the current <see cref="AppDomain"/>.</value>
		/// <remarks>
		/// <para>
		/// Gets the base directory for this <see cref="AppDomain"/>.
		/// </para>
		/// <para>
		/// The value returned may be either a local file path or a URI.
		/// </para>
		/// </remarks>
		public static string ApplicationBaseDirectory
		{
			get 
			{
#if NETCF
				return System.IO.Path.GetDirectoryName(SystemInfo.EntryAssemblyLocation) + System.IO.Path.DirectorySeparatorChar;
#else
				return AppDomain.CurrentDomain.BaseDirectory;
#endif
			}
		}

		/// <summary>
		/// Gets the path to the configuration file for the current <see cref="AppDomain"/>.
		/// </summary>
		/// <value>The path to the configuration file for the current <see cref="AppDomain"/>.</value>
		/// <remarks>
		/// <para>
		/// The .NET Compact Framework 1.0 does not have a concept of a configuration
		/// file. For this runtime, we use the entry assembly location as the root for
		/// the configuration file name.
		/// </para>
		/// <para>
		/// The value returned may be either a local file path or a URI.
		/// </para>
		/// </remarks>
		public static string ConfigurationFileLocation
		{
			get 
			{
#if NETCF
				return SystemInfo.EntryAssemblyLocation+".config";
#else
				return System.AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
#endif
			}
		}

		/// <summary>
		/// Gets the path to the file that first executed in the current <see cref="AppDomain"/>.
		/// </summary>
		/// <value>The path to the entry assembly.</value>
		/// <remarks>
		/// <para>
		/// Gets the path to the file that first executed in the current <see cref="AppDomain"/>.
		/// </para>
		/// </remarks>
		public static string EntryAssemblyLocation
		{
			get 
			{
#if NETCF
				return SystemInfo.NativeEntryAssemblyLocation;
#else
				return System.Reflection.Assembly.GetEntryAssembly().Location;
#endif
			}
		}

		/// <summary>
		/// Gets the ID of the current thread.
		/// </summary>
		/// <value>The ID of the current thread.</value>
		/// <remarks>
		/// <para>
		/// On the .NET framework, the <c>AppDomain.GetCurrentThreadId</c> method
		/// is used to obtain the thread ID for the current thread. This is the 
		/// operating system ID for the thread.
		/// </para>
		/// <para>
		/// On the .NET Compact Framework 1.0 it is not possible to get the 
		/// operating system thread ID for the current thread. The native method 
		/// <c>GetCurrentThreadId</c> is implemented inline in a header file
		/// and cannot be called.
		/// </para>
		/// <para>
		/// On the .NET Framework 2.0 the <c>Thread.ManagedThreadId</c> is used as this
		/// gives a stable id unrelated to the operating system thread ID which may 
		/// change if the runtime is using fibers.
		/// </para>
		/// </remarks>
		public static int CurrentThreadId
		{
			get 
			{
				return System.Threading.Thread.CurrentThread.ManagedThreadId;
			}
		}

		/// <summary>
		/// Get the host name or machine name for the current machine
		/// </summary>
		/// <value>
		/// The hostname or machine name
		/// </value>
		/// <remarks>
		/// <para>
		/// Get the host name or machine name for the current machine
		/// </para>
		/// <para>
		/// The host name (<see cref="System.Net.Dns.GetHostName"/>) or
		/// the machine name (<c>Environment.MachineName</c>) for
		/// the current machine, or if neither of these are available
		/// then <c>NOT AVAILABLE</c> is returned.
		/// </para>
		/// </remarks>
		public static string HostName
		{
			get
			{
				if (s_hostName == null)
				{

					// Get the DNS host name of the current machine
					try
					{
						// Lookup the host name
						s_hostName = System.Net.Dns.GetHostName();
					}
					catch (System.Net.Sockets.SocketException)
					{
						LogLog.Debug(declaringType, "Socket exception occurred while getting the dns hostname. Error Ignored.");
					}
					catch (System.Security.SecurityException)
					{
						// We may get a security exception looking up the hostname
						// You must have Unrestricted DnsPermission to access resource
						LogLog.Debug(declaringType, "Security exception occurred while getting the dns hostname. Error Ignored.");
					}
					catch (Exception ex)
					{
						LogLog.Debug(declaringType, "Some other exception occurred while getting the dns hostname. Error Ignored.", ex);
					}

					// Get the NETBIOS machine name of the current machine
					if (s_hostName == null || s_hostName.Length == 0)
					{
						try
						{
#if !NETCF
							s_hostName = Environment.MachineName;
#endif
						}
						catch(InvalidOperationException)
						{
						}
						catch(System.Security.SecurityException)
						{
							// We may get a security exception looking up the machine name
							// You must have Unrestricted EnvironmentPermission to access resource
						}
					}

					// Couldn't find a value
					if (s_hostName == null || s_hostName.Length == 0)
					{
						s_hostName = s_notAvailableText;
						LogLog.Debug(declaringType, "Could not determine the hostname. Error Ignored. Empty host name will be used");
					}
				}
				return s_hostName;
			}
		}

		/// <summary>
		/// Get this application's friendly name
		/// </summary>
		/// <value>
		/// The friendly name of this application as a string
		/// </value>
		/// <remarks>
		/// <para>
		/// If available the name of the application is retrieved from
		/// the <c>AppDomain</c> using <c>AppDomain.CurrentDomain.FriendlyName</c>.
		/// </para>
		/// <para>
		/// Otherwise the file name of the entry assembly is used.
		/// </para>
		/// </remarks>
		public static string ApplicationFriendlyName
		{
			get
			{
				if (s_appFriendlyName == null)
				{
					try
					{
#if !NETCF
						s_appFriendlyName = AppDomain.CurrentDomain.FriendlyName;
#endif
					}
					catch(System.Security.SecurityException)
					{
						// This security exception will occur if the caller does not have 
						// some undefined set of SecurityPermission flags.
						LogLog.Debug(declaringType, "Security exception while trying to get current domain friendly name. Error Ignored.");
					}

					if (s_appFriendlyName == null || s_appFriendlyName.Length == 0)
					{
						try
						{
							string assemblyLocation = SystemInfo.EntryAssemblyLocation;
							s_appFriendlyName = System.IO.Path.GetFileName(assemblyLocation);
						}
						catch(System.Security.SecurityException)
						{
							// Caller needs path discovery permission
						}
					}

					if (s_appFriendlyName == null || s_appFriendlyName.Length == 0)
					{
						s_appFriendlyName = s_notAvailableText;
					}
				}
				return s_appFriendlyName;
			}
		}

		/// <summary>
		/// Get the start time for the current process.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is the time at which the SmartInspect library was loaded into the
		/// AppDomain. Due to reports of a hang in the call to <c>System.Diagnostics.Process.StartTime</c>
		/// this is not the start time for the current process.
		/// </para>
		/// <para>
		/// The SmartInspect library should be loaded by an application early during its
		/// startup, therefore this start time should be a good approximation for
		/// the actual start time.
		/// </para>
		/// <para>
		/// Note that AppDomains may be loaded and unloaded within the
		/// same process without the process terminating, however this start time
		/// will be set per AppDomain.
		/// </para>
		/// </remarks>
		public static DateTime ProcessStartTime
		{
			get { return s_processStartTime; }
		}

		/// <summary>
		/// Text to output when a <c>null</c> is encountered.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Use this value to indicate a <c>null</c> has been encountered while
		/// outputting a string representation of an item.
		/// </para>
		/// <para>
		/// The default value is <c>(null)</c>. This value can be overridden by specifying
		/// a value for the <c>SmartInspect.NullText</c> appSetting in the application's
		/// .config file.
		/// </para>
		/// </remarks>
		public static string NullText
		{
			get { return s_nullText; }
			set { s_nullText = value; }
		}

		/// <summary>
		/// Text to output when an unsupported feature is requested.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Use this value when an unsupported feature is requested.
		/// </para>
		/// <para>
		/// The default value is <c>NOT AVAILABLE</c>. This value can be overridden by specifying
		/// a value for the <c>SmartInspect.NotAvailableText</c> appSetting in the application's
		/// .config file.
		/// </para>
		/// </remarks>
		public static string NotAvailableText
		{
			get { return s_notAvailableText; }
			set { s_notAvailableText = value; }
		}

		#endregion Public Static Properties

		#region Public Static Methods

		/// <summary>
		/// Gets the assembly location path for the specified assembly.
		/// </summary>
		/// <param name="myAssembly">The assembly to get the location for.</param>
		/// <returns>The location of the assembly.</returns>
		/// <remarks>
		/// <para>
		/// This method does not guarantee to return the correct path
		/// to the assembly. If only tries to give an indication as to
		/// where the assembly was loaded from.
		/// </para>
		/// </remarks>
		public static string AssemblyLocationInfo(Assembly myAssembly)
		{
#if NETCF
			return "Not supported on Microsoft .NET Compact Framework";
#else
			if (myAssembly.GlobalAssemblyCache)
			{
				return "Global Assembly Cache";
			}
			else
			{
				try
				{
#if FRAMEWORK_4_0_OR_ABOVE
					if (myAssembly.IsDynamic)
					{
						return "Dynamic Assembly";
					}
#else
					if (myAssembly is System.Reflection.Emit.AssemblyBuilder)
					{
						return "Dynamic Assembly";
					}
					else if(myAssembly.GetType().FullName == "System.Reflection.Emit.InternalAssemblyBuilder")
					{
						return "Dynamic Assembly";
					}
#endif
					else
					{
						// This call requires FileIOPermission for access to the path
						// if we don't have permission then we just ignore it and
						// carry on.
						return myAssembly.Location;
					}
				}
				catch (NotSupportedException)
				{
					// The location information may be unavailable for dynamic assemblies and a NotSupportedException
					// is thrown in those cases. See: http://msdn.microsoft.com/de-de/library/system.reflection.assembly.location.aspx
					return "Dynamic Assembly";
				}
				catch (TargetInvocationException ex)
				{
					return "Location Detect Failed (" + ex.Message + ")";
				}
				catch (ArgumentException ex)
				{
					return "Location Detect Failed (" + ex.Message + ")";
				}
				catch (System.Security.SecurityException)
				{
					return "Location Permission Denied";
				}
			}
#endif
		}

		/// <summary>
		/// Gets the fully qualified name of the <see cref="Type" />, including 
		/// the name of the assembly from which the <see cref="Type" /> was 
		/// loaded.
		/// </summary>
		/// <param name="type">The <see cref="Type" /> to get the fully qualified name for.</param>
		/// <returns>The fully qualified name for the <see cref="Type" />.</returns>
		/// <remarks>
		/// <para>
		/// This is equivalent to the <c>Type.AssemblyQualifiedName</c> property,
		/// but this method works on the .NET Compact Framework 1.0 as well as
		/// the full .NET runtime.
		/// </para>
		/// </remarks>
		public static string AssemblyQualifiedName(Type type)
		{
			return type.FullName + ", " + type.Assembly.FullName;
		}

		/// <summary>
		/// Gets the short name of the <see cref="Assembly" />.
		/// </summary>
		/// <param name="myAssembly">The <see cref="Assembly" /> to get the name for.</param>
		/// <returns>The short name of the <see cref="Assembly" />.</returns>
		/// <remarks>
		/// <para>
		/// The short name of the assembly is the <see cref="Assembly.FullName" /> 
		/// without the version, culture, or public key. i.e. it is just the 
		/// assembly's file name without the extension.
		/// </para>
		/// <para>
		/// Use this rather than <c>Assembly.GetName().Name</c> because that
		/// is not available on the Compact Framework.
		/// </para>
		/// <para>
		/// Because of a FileIOPermission security demand we cannot do
		/// the obvious Assembly.GetName().Name. We are allowed to get
		/// the <see cref="Assembly.FullName" /> of the assembly so we 
		/// start from there and strip out just the assembly name.
		/// </para>
		/// </remarks>
		public static string AssemblyShortName(Assembly myAssembly)
		{
			string name = myAssembly.FullName;
			int offset = name.IndexOf(',');
			if (offset > 0)
			{
				name = name.Substring(0, offset);
			}
			return name.Trim();

			// TODO: Do we need to unescape the assembly name string? 
			// Doc says '\' is an escape char but has this already been 
			// done by the string loader?
		}

		/// <summary>
		/// Gets the file name portion of the <see cref="Assembly" />, including the extension.
		/// </summary>
		/// <param name="myAssembly">The <see cref="Assembly" /> to get the file name for.</param>
		/// <returns>The file name of the assembly.</returns>
		/// <remarks>
		/// <para>
		/// Gets the file name portion of the <see cref="Assembly" />, including the extension.
		/// </para>
		/// </remarks>
		public static string AssemblyFileName(Assembly myAssembly)
		{
#if NETCF
			// This is not very good because it assumes that only
			// the entry assembly can be an EXE. In fact multiple
			// EXEs can be loaded in to a process.

			string assemblyShortName = SystemInfo.AssemblyShortName(myAssembly);
			string entryAssemblyShortName = System.IO.Path.GetFileNameWithoutExtension(SystemInfo.EntryAssemblyLocation);

			if (string.Compare(assemblyShortName, entryAssemblyShortName, true) == 0)
			{
				// assembly is entry assembly
				return assemblyShortName + ".exe";
			}
			else
			{
				// assembly is not entry assembly
				return assemblyShortName + ".dll";
			}
#else
			return System.IO.Path.GetFileName(myAssembly.Location);
#endif
		}

		/// <summary>
		/// Loads the type specified in the type string.
		/// </summary>
		/// <param name="relativeType">A sibling type to use to load the type.</param>
		/// <param name="typeName">The name of the type to load.</param>
		/// <param name="throwOnError">Flag set to <c>true</c> to throw an exception if the type cannot be loaded.</param>
		/// <param name="ignoreCase"><c>true</c> to ignore the case of the type name; otherwise, <c>false</c></param>
		/// <returns>The type loaded or <c>null</c> if it could not be loaded.</returns>
		/// <remarks>
		/// <para>
		/// If the type name is fully qualified, i.e. if contains an assembly name in 
		/// the type name, the type will be loaded from the system using 
		/// <see cref="M:Type.GetType(string,bool)"/>.
		/// </para>
		/// <para>
		/// If the type name is not fully qualified, it will be loaded from the assembly
		/// containing the specified relative type. If the type is not found in the assembly 
		/// then all the loaded assemblies will be searched for the type.
		/// </para>
		/// </remarks>
		public static Type GetTypeFromString(Type relativeType, string typeName, bool throwOnError, bool ignoreCase)
		{
			return GetTypeFromString(relativeType.Assembly, typeName, throwOnError, ignoreCase);
		}

		/// <summary>
		/// Loads the type specified in the type string.
		/// </summary>
		/// <param name="typeName">The name of the type to load.</param>
		/// <param name="throwOnError">Flag set to <c>true</c> to throw an exception if the type cannot be loaded.</param>
		/// <param name="ignoreCase"><c>true</c> to ignore the case of the type name; otherwise, <c>false</c></param>
		/// <returns>The type loaded or <c>null</c> if it could not be loaded.</returns>		
		/// <remarks>
		/// <para>
		/// If the type name is fully qualified, i.e. if contains an assembly name in 
		/// the type name, the type will be loaded from the system using 
		/// <see cref="M:Type.GetType(string,bool)"/>.
		/// </para>
		/// <para>
		/// If the type name is not fully qualified it will be loaded from the
		/// assembly that is directly calling this method. If the type is not found 
		/// in the assembly then all the loaded assemblies will be searched for the type.
		/// </para>
		/// </remarks>
		public static Type GetTypeFromString(string typeName, bool throwOnError, bool ignoreCase)
		{
			return GetTypeFromString(Assembly.GetCallingAssembly(), typeName, throwOnError, ignoreCase);
		}

		/// <summary>
		/// Loads the type specified in the type string.
		/// </summary>
		/// <param name="relativeAssembly">An assembly to load the type from.</param>
		/// <param name="typeName">The name of the type to load.</param>
		/// <param name="throwOnError">Flag set to <c>true</c> to throw an exception if the type cannot be loaded.</param>
		/// <param name="ignoreCase"><c>true</c> to ignore the case of the type name; otherwise, <c>false</c></param>
		/// <returns>The type loaded or <c>null</c> if it could not be loaded.</returns>
		/// <remarks>
		/// <para>
		/// If the type name is fully qualified, i.e. if contains an assembly name in 
		/// the type name, the type will be loaded from the system using 
		/// <see cref="M:Type.GetType(string,bool)"/>.
		/// </para>
		/// <para>
		/// If the type name is not fully qualified it will be loaded from the specified
		/// assembly. If the type is not found in the assembly then all the loaded assemblies 
		/// will be searched for the type.
		/// </para>
		/// </remarks>
		public static Type GetTypeFromString(Assembly relativeAssembly, string typeName, bool throwOnError, bool ignoreCase)
		{
			// Check if the type name specifies the assembly name
			if(typeName.IndexOf(',') == -1)
			{
				//LogLog.Debug(declaringType, "SystemInfo: Loading type ["+typeName+"] from assembly ["+relativeAssembly.FullName+"]");
#if NETCF
				return relativeAssembly.GetType(typeName, throwOnError);
#else
				// Attempt to lookup the type from the relativeAssembly
				Type type = relativeAssembly.GetType(typeName, false, ignoreCase);
				if (type != null)
				{
					// Found type in relative assembly
					//LogLog.Debug(declaringType, "SystemInfo: Loaded type ["+typeName+"] from assembly ["+relativeAssembly.FullName+"]");
					return type;
				}

				Assembly[] loadedAssemblies = null;
				try
				{
					loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
				}
				catch(System.Security.SecurityException)
				{
					// Insufficient permissions to get the list of loaded assemblies
				}

				if (loadedAssemblies != null)
				{
					// Search the loaded assemblies for the type
					foreach (Assembly assembly in loadedAssemblies) 
					{
						type = assembly.GetType(typeName, false, ignoreCase);
						if (type != null)
						{
							// Found type in loaded assembly
							LogLog.Debug(declaringType, "Loaded type ["+typeName+"] from assembly ["+assembly.FullName+"] by searching loaded assemblies.");
							return type;
						}
					}
				}

				// Didn't find the type
				if (throwOnError)
				{
					throw new TypeLoadException("Could not load type ["+typeName+"]. Tried assembly ["+relativeAssembly.FullName+"] and all loaded assemblies");
				}
				return null;
#endif
			}
			else
			{
				// Includes explicit assembly name
				//LogLog.Debug(declaringType, "SystemInfo: Loading type ["+typeName+"] from global Type");

#if NETCF
				// In NETCF 2 and 3 arg versions seem to behave differently
				// https://issues.apache.org/jira/browse/SmartInspect-113
				return Type.GetType(typeName, throwOnError);
#else
				return Type.GetType(typeName, throwOnError, ignoreCase);
#endif
			}
		}


		/// <summary>
		/// Generate a new guid
		/// </summary>
		/// <returns>A new Guid</returns>
		/// <remarks>
		/// <para>
		/// Generate a new guid
		/// </para>
		/// </remarks>
		public static Guid NewGuid()
		{
			return Guid.NewGuid();
		}

		/// <summary>
		/// Create an <see cref="ArgumentOutOfRangeException"/>
		/// </summary>
		/// <param name="parameterName">The name of the parameter that caused the exception</param>
		/// <param name="actualValue">The value of the argument that causes this exception</param>
		/// <param name="message">The message that describes the error</param>
		/// <returns>the ArgumentOutOfRangeException object</returns>
		/// <remarks>
		/// <para>
		/// Create a new instance of the <see cref="ArgumentOutOfRangeException"/> class 
		/// with a specified error message, the parameter name, and the value 
		/// of the argument.
		/// </para>
		/// <para>
		/// The Compact Framework does not support the 3 parameter constructor for the
		/// <see cref="ArgumentOutOfRangeException"/> type. This method provides an
		/// implementation that works for all platforms.
		/// </para>
		/// </remarks>
		public static ArgumentOutOfRangeException CreateArgumentOutOfRangeException(string parameterName, object actualValue, string message)
		{
#if NETCF
			return new ArgumentOutOfRangeException(parameterName, message + " [value=" + actualValue + "]");
#else
			return new ArgumentOutOfRangeException(parameterName, actualValue, message);
#endif
		}


		/// <summary>
		/// Parse a string into an <see cref="Int32"/> value
		/// </summary>
		/// <param name="s">the string to parse</param>
		/// <param name="val">out param where the parsed value is placed</param>
		/// <returns><c>true</c> if the string was able to be parsed into an integer</returns>
		/// <remarks>
		/// <para>
		/// Attempts to parse the string into an integer. If the string cannot
		/// be parsed then this method returns <c>false</c>. The method does not throw an exception.
		/// </para>
		/// </remarks>
		public static bool TryParse(string s, out int val)
		{
#if NETCF
			val = 0;
			try
			{
				val = int.Parse(s, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
				return true;
			}
			catch
			{
			}

			return false;
#else
			// Initialise out param
			val = 0;

			try
			{
				double doubleVal;
				if (Double.TryParse(s, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out doubleVal))
				{
					val = Convert.ToInt32(doubleVal);
					return true;
				}
			}
			catch
			{
				// Ignore exception, just return false
			}

			return false;
#endif
		}

		/// <summary>
		/// Parse a string into an <see cref="Int64"/> value
		/// </summary>
		/// <param name="s">the string to parse</param>
		/// <param name="val">out param where the parsed value is placed</param>
		/// <returns><c>true</c> if the string was able to be parsed into an integer</returns>
		/// <remarks>
		/// <para>
		/// Attempts to parse the string into an integer. If the string cannot
		/// be parsed then this method returns <c>false</c>. The method does not throw an exception.
		/// </para>
		/// </remarks>
		public static bool TryParse(string s, out long val)
		{
#if NETCF
			val = 0;
			try
			{
				val = long.Parse(s, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
				return true;
			}
			catch
			{
			}

			return false;
#else
			// Initialise out param
			val = 0;

			try
			{
				double doubleVal;
				if (Double.TryParse(s, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out doubleVal))
				{
					val = Convert.ToInt64(doubleVal);
					return true;
				}
			}
			catch
			{
				// Ignore exception, just return false
			}

			return false;
#endif
		}

        /// <summary>
        /// Parse a string into an <see cref="Int16"/> value
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <param name="val">out param where the parsed value is placed</param>
        /// <returns><c>true</c> if the string was able to be parsed into an integer</returns>
        /// <remarks>
        /// <para>
        /// Attempts to parse the string into an integer. If the string cannot
        /// be parsed then this method returns <c>false</c>. The method does not throw an exception.
        /// </para>
        /// </remarks>
        public static bool TryParse(string s, out short val)
        {
#if NETCF
			val = 0;
			try
			{
				val = short.Parse(s, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
				return true;
			}
			catch
			{
			}

			return false;
#else
            // Initialise out param
            val = 0;

            try 
            {
                double doubleVal;
                if (Double.TryParse(s, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out doubleVal))
                {
                    val = Convert.ToInt16(doubleVal);
                    return true;
                }
            }
            catch
            {
                // Ignore exception, just return false
            }

            return false;
#endif
        }

        /// <summary>
		/// Lookup an application setting
		/// </summary>
		/// <param name="key">the application settings key to lookup</param>
		/// <returns>the value for the key, or <c>null</c></returns>
		/// <remarks>
		/// <para>
		/// Configuration APIs are not supported under the Compact Framework
		/// </para>
		/// </remarks>
		public static string GetAppSetting(string key)
		{
#if NETCF
				// Configuration APIs are not suported under the Compact Framework
#else
			try
			{
				return ConfigurationManager.AppSettings[key];
			}
			catch(Exception ex)
			{
				// If an exception is thrown here then it looks like the config file does not parse correctly.
				LogLog.Error(declaringType, "Exception while reading ConfigurationSettings. Check your .config file is well formed XML.", ex);
			}
#endif
			return null;
		}

		/// <summary>
		/// Convert a path into a fully qualified local file path.
		/// </summary>
		/// <param name="path">The path to convert.</param>
		/// <returns>The fully qualified path.</returns>
		/// <remarks>
		/// <para>
		/// Converts the path specified to a fully
		/// qualified path. If the path is relative it is
		/// taken as relative from the application base 
		/// directory.
		/// </para>
		/// <para>
		/// The path specified must be a local file path, a URI is not supported.
		/// </para>
		/// </remarks>
		public static string ConvertToFullPath(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}

			string baseDirectory = "";
			try
			{
				string applicationBaseDirectory = SystemInfo.ApplicationBaseDirectory;
				if (applicationBaseDirectory != null)
				{
					// applicationBaseDirectory may be a URI not a local file path
					Uri applicationBaseDirectoryUri = new Uri(applicationBaseDirectory);
					if (applicationBaseDirectoryUri.IsFile)
					{
						baseDirectory = applicationBaseDirectoryUri.LocalPath;
					}
				}
			}
			catch
			{
				// Ignore URI exceptions & SecurityExceptions from SystemInfo.ApplicationBaseDirectory
			}

			if (baseDirectory != null && baseDirectory.Length > 0)
			{
				// Note that Path.Combine will return the second path if it is rooted
				return Path.GetFullPath(Path.Combine(baseDirectory, path));
			}
			return Path.GetFullPath(path);
		}

		/// <summary>
		/// Creates a new case-insensitive instance of the <see cref="Hashtable"/> class with the default initial capacity. 
		/// </summary>
		/// <returns>A new case-insensitive instance of the <see cref="Hashtable"/> class with the default initial capacity</returns>
		/// <remarks>
		/// <para>
		/// The new Hashtable instance uses the default load factor, the CaseInsensitiveHashCodeProvider, and the CaseInsensitiveComparer.
		/// </para>
		/// </remarks>
		public static Hashtable CreateCaseInsensitiveHashtable()
		{
			return new Hashtable(StringComparer.OrdinalIgnoreCase);
		}

		#endregion Public Static Methods

		#region Private Static Methods

#if NETCF
		private static string NativeEntryAssemblyLocation 
		{
			get 
			{
				StringBuilder moduleName = null;

				IntPtr moduleHandle = GetModuleHandle(IntPtr.Zero);

				if (moduleHandle != IntPtr.Zero) 
				{
					moduleName = new StringBuilder(255);
					if (GetModuleFileName(moduleHandle, moduleName,	moduleName.Capacity) == 0) 
					{
						throw new NotSupportedException(NativeError.GetLastError().ToString());
					}
				} 
				else 
				{
					throw new NotSupportedException(NativeError.GetLastError().ToString());
				}

				return moduleName.ToString();
			}
		}

		[DllImport("CoreDll.dll", SetLastError=true, CharSet=CharSet.Unicode)]
		private static extern IntPtr GetModuleHandle(IntPtr ModuleName);

		[DllImport("CoreDll.dll", SetLastError=true, CharSet=CharSet.Unicode)]
		private static extern Int32 GetModuleFileName(
			IntPtr hModule,
			StringBuilder ModuleName,
			Int32 cch);

#endif

		#endregion Private Static Methods

		#region Public Static Fields

		/// <summary>
		/// Gets an empty array of types.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The <c>Type.EmptyTypes</c> field is not available on
		/// the .NET Compact Framework 1.0.
		/// </para>
		/// </remarks>
		public static readonly Type[] EmptyTypes = new Type[0];

		#endregion Public Static Fields

		#region Private Static Fields

	    /// <summary>
	    /// The fully qualified type of the SystemInfo class.
	    /// </summary>
	    /// <remarks>
	    /// Used by the internal logger to record the Type of the
	    /// log message.
	    /// </remarks>
	    private readonly static Type declaringType = typeof(SystemInfo);

		/// <summary>
		/// Cache the host name for the current machine
		/// </summary>
		private static string s_hostName;

		/// <summary>
		/// Cache the application friendly name
		/// </summary>
		private static string s_appFriendlyName;

		/// <summary>
		/// Text to output when a <c>null</c> is encountered.
		/// </summary>
		private static string s_nullText;

		/// <summary>
		/// Text to output when an unsupported feature is requested.
		/// </summary>
		private static string s_notAvailableText;

		/// <summary>
		/// Start time for the current process.
		/// </summary>
		private static DateTime s_processStartTime = DateTime.Now;

		#endregion
	}
}