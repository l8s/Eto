using System;
using System.IO;
using System.Reflection;
#if !PCL
using System.Windows.Markup;
using System.Xaml;
#endif

namespace Eto.Serialization.Xaml
{
	/// <summary>
	/// Methods to help load/save Eto objects to/from xaml
	/// </summary>
	public static class XamlReader
	{
		static Stream GetStream(Type type)
		{
			return
				GetStream(type, type.FullName + ".xeto")
				?? GetStream(type, type.FullName + ".xaml")
				?? GetStream(type, type.Name + ".xeto"); // for f# projects
		}

		static Stream GetStream(Type type, string resourceName)
		{
#if PCL
			return type.GetTypeInfo().Assembly.GetManifestResourceStream(resourceName);
#else
			return type.Assembly.GetManifestResourceStream(resourceName);
#endif
		}

		/// <summary>
		/// Loads the specified type from a xaml of the same name
		/// </summary>
		/// <remarks>
		/// If your class name is MyNamespace.MyType, then this will attempt to load MyNamespace.MyType.xaml
		/// for the xaml definition in the same assembly.
		/// 
		/// If you want to specify a different xaml, use <see cref="Load{T}(Stream)"/>
		/// </remarks>
		/// <typeparam name="T">Type of object to load from xaml</typeparam>
		/// <returns>A new instance of the specified type with the contents loaded from xaml</returns>
		public static T Load<T>()
			where T : Widget, new()
		{
			using (var stream = GetStream(typeof(T)))
			{
				return Load<T>(stream, null);
			}
		}

		/// <summary>
		/// Loads the specified type from the specified xaml stream
		/// </summary>
		/// <remarks>
		/// If your class name is MyNamespace.MyType, then this will attempt to load MyNamespace.MyType.xaml
		/// for the xaml definition in the same assembly.
		/// </remarks>
		/// <typeparam name="T">Type of object to load from the specified xaml</typeparam>
		/// <param name="stream">Xaml content to load (e.g. from resources)</param>
		/// <returns>A new instance of the specified type with the contents loaded from the xaml stream</returns>
		public static T Load<T>(Stream stream)
			where T : Widget, new()
		{
			return Load<T>(stream, null);
		}

		/// <summary>
		/// Loads the specified instance with xaml of the same name
		/// </summary>
		/// <remarks>
		/// If your class name is MyNamespace.MyType, then this will attempt to load MyNamespace.MyType.xaml
		/// for the xaml definition in the same assembly.
		/// 
		/// If you want to specify a different xaml, use <see cref="Load{T}(Stream, T)"/>
		/// </remarks>
		/// <typeparam name="T">Type of object to load from the specified xaml</typeparam>
		/// <param name="instance">Instance to use as the starting object</param>
		/// <returns>A new or existing instance of the specified type with the contents loaded from the xaml stream</returns>
		public static void Load<T>(T instance)
			where T : Widget
		{
			using (var stream = GetStream(typeof(T)))
			{
				Load<T>(stream, instance);
			}
		}

		/// <summary>
		/// Loads the specified instance with a specified fully qualified xaml embedded resource
		/// </summary>
		/// <remarks>
		/// This will load the embedded resource from the same assembly as <paramref name="instance"/> with the 
		/// specified <paramref name="resourceName"/> embedded resource.
		/// 
		/// If you want to specify a different xaml, use <see cref="Load{T}(Stream, T)"/>
		/// </remarks>
		/// <typeparam name="T">Type of object to load from the specified xaml</typeparam>
		/// <param name="instance">Instance to use as the starting object</param>
		/// <param name="resourceName">Fully qualified name of the embedded resource to load.</param>
		/// <returns>An existing instance of the specified type with the contents loaded from the xaml stream</returns>
		public static void Load<T>(T instance, string resourceName)
			where T : Widget
		{
			using (var stream = GetStream(typeof(T), resourceName))
			{
				Load<T>(stream, instance);
			}
		}

#if !PCL
		static readonly EtoXamlSchemaContext context = new EtoXamlSchemaContext(new [] { typeof(XamlReader).Assembly });
#endif

		/// <summary>
		/// Gets or sets a value indicating that the reader is used in design mode
		/// </summary>
		/// <remarks>
		/// In Design mode, events are not wired up and will not cause exceptions due to missing methods to wire up to.
		/// </remarks>
		public static bool DesignMode
		{
#if !PCL
			get { return context.DesignMode; }
			set { context.DesignMode = value; }
#else
			get; set;
#endif
		}

		/// <summary>
		/// Loads the specified type from the specified xaml stream
		/// </summary>
		/// <typeparam name="T">Type of object to load from the specified xaml</typeparam>
		/// <param name="stream">Xaml content to load (e.g. from resources)</param>
		/// <param name="instance">Instance to use as the starting object, or null to create a new instance</param>
		/// <returns>A new or existing instance of the specified type with the contents loaded from the xaml stream</returns>
		public static T Load<T>(Stream stream, T instance)
			where T : Widget
		{
#if PCL
			throw new NotImplementedException("You must reference the Eto.Serlialization.Xaml package from your net45 project that references your PCL library");
#else
			var reader = new XamlXmlReader(stream, context);
			var writerSettings = new XamlObjectWriterSettings();
			writerSettings.AfterPropertiesHandler += delegate(object sender, XamlObjectEventArgs e)
			{
				if (writerSettings.RootObjectInstance != null)
				{
					var instanceType = writerSettings.RootObjectInstance.GetType();
					var obj = e.Instance as Widget;
					if (obj != null && !string.IsNullOrEmpty(obj.ID))
					{
                        var property = instanceType.GetProperty(obj.ID, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                        if (property != null)
							property.SetValue(writerSettings.RootObjectInstance, obj, null);
						else
						{
							var field = instanceType.GetField(obj.ID, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
							if (field != null)
								field.SetValue(writerSettings.RootObjectInstance, obj);
						}
					}
				}
			};
			writerSettings.RootObjectInstance = instance;
			var writer = new XamlObjectWriter(context, writerSettings);
			
			XamlServices.Transform(reader, writer);
			return writer.Result as T;
#endif
		}
	}
}