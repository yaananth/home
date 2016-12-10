﻿// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
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

using System.IO;
using System.Xml;
using ExtendedXmlSerialization.Elements;
using ExtendedXmlSerialization.Extensibility.Write;

namespace ExtendedXmlSerialization.ProcessModel.Write
{
    public class WritingFactory : IWritingFactory
    {
        private readonly ISerializationToolsFactoryHost _services;
        private readonly INamespaceLocator _locator;
        private readonly INamespaces _namespaces;

        public WritingFactory(
            ISerializationToolsFactoryHost services,
            INamespaceLocator locator,
            INamespaces namespaces
        )
        {
            _locator = locator;
            _namespaces = namespaces;
            _services = services;
        }

        public IWriting Get(Stream parameter)
        {
            var context = _services.New();
            var settings = new XmlWriterSettings {NamespaceHandling = NamespaceHandling.OmitDuplicates, Indent = true};
            var xmlWriter = XmlWriter.Create(parameter, settings);
            var serializer = new EncryptedObjectSerializer(new EncryptionSpecification(_services, context), _services);
            var writer = new Writer(serializer, _locator, new NamespaceEmitter(xmlWriter, _namespaces), xmlWriter);
            var extensions = new ExtensionRegistry();
            foreach (var extension in _services.Extensions)
            {
                extension.Accept(extensions);
            }
            
            var result = new Writing(writer, context, _locator, extensions
                                     /*services:*/, _services, context, settings, writer, xmlWriter);
            return result;
        }
    }
}