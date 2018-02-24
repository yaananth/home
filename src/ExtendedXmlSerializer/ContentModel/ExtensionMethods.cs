﻿// MIT License
//
// Copyright (c) 2016-2018 Wojciech Nagórski
//                    Michael DeMond
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel
{
	static class ExtensionMethods
	{
		public static ISerializer Adapt<T>(this ISerializer<T> @this) => new GenericSerializerAdapter<T>(@this);

		public static ISerializer<T> Adapt<T>(this ISerializer @this) =>
			@this as ISerializer<T> ?? new SerializerAdapter<T>(@this);

		public static IWriter Adapt<T>(this IWriter<T> @this) => new GenericWriterAdapter<T>(@this);
		public static IWriter<T> Adapt<T>(this IWriter @this) => new WriterAdapter<T>(@this);

		public static IReader Adapt<T>(this IReader<T> @this) => new GenericReaderAdapter<T>(@this);
		public static IReader<T> Adapt<T>(this IReader @this) => new ReaderAdapter<T>(@this);

		public static IReader<T> CreateContents<T>(this IInnerContentServices @this, IInnerContentHandler parameter)
			=> new ReaderAdapter<T>(@this.Create(Support<T>.Key, parameter));

		public static TypeInfo GetClassification(this IClassification @this, IFormatReader parameter,
		                                         TypeInfo defaultValue = null)
		{
			var result = @this.Get(parameter) ?? defaultValue;
			if (result == null)
			{
				var name = IdentityFormatter.Default.Get(parameter);
				throw new InvalidOperationException(
				                                    $"An attempt was made to load a type with the fully qualified name of '{name}', but no type could be located with that name.");
			}

			return result;
		}

		public static Func<T, string> ToFormatter<T>(this IConverter<T> @this) => @this.Format;

		public static Func<string, T> ToParser<T>(this IConverter<T> @this) => @this.Parse;


		sealed class Converters<TFrom, TTo> : ReferenceCache<IConverter<TFrom>, IConverter<TTo>>
		{
			public static Converters<TFrom, TTo> Default { get; } = new Converters<TFrom, TTo>();
			Converters() : base(x => x.To(CastCoercer<TFrom, TTo>.Default, CastCoercer<TTo, TFrom>.Default)) {}
		}

		public static IConverter<TTo> To<TFrom, TTo>(this IConverter<TFrom> @this, A<TTo> _)
			=> Converters<TFrom, TTo>.Default.Get(@this);

		public static IConverter<TTo> To<TFrom, TTo>(this IConverter<TFrom> @this, IParameterizedSource<TFrom, TTo> parse, IParameterizedSource<TTo, TFrom> format)
		=> new Converter<TTo>(@this.ToParser().Out(parse.ToDelegate()),
		                      @this.ToFormatter().In(format.ToDelegate()));
	}
}