// MIT License
// 
// Copyright (c) 2016 Wojciech Nag�rski
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

using System.Reflection;
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ContentModel.Collections
{
	abstract class CollectionContentOptionBase : ContentOptionBase
	{
		readonly static AllSpecification<TypeInfo> Specification =
			new AllSpecification<TypeInfo>(IsActivatedTypeSpecification.Default, IsCollectionTypeSpecification.Default);

		readonly static CollectionItemTypeLocator Locator = CollectionItemTypeLocator.Default;

		readonly ISerialization _serialization;
		readonly ICollectionItemTypeLocator _locator;

		protected CollectionContentOptionBase(ISerialization serialization) : this(Specification, serialization) {}

		protected CollectionContentOptionBase(ISpecification<TypeInfo> specification, ISerialization serialization)
			: this(specification, serialization, Locator) {}

		protected CollectionContentOptionBase(ISpecification<TypeInfo> specification, ISerialization serialization,
		                                      ICollectionItemTypeLocator locator) : base(specification)
		{
			_serialization = serialization;
			_locator = locator;
		}

		public sealed override ISerializer Get(TypeInfo parameter)
		{
			var itemType = _locator.Get(parameter);
			var result = Create(_serialization.Get(itemType), parameter, itemType);
			return result;
		}

		protected abstract ISerializer Create(ISerializer item, TypeInfo classification, TypeInfo itemType);
	}
}