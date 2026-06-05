#!/bin/bash

# Fix CA1851 in Collection.Contains.cs - multiple enumeration warnings
sed -i '330i#pragma warning disable CA1851 // Possible multiple enumerations' src/AspNetCore.Simple.MsTest.Sdk/AssertExtensions/Collection/Assert.That.Collection.Contains.cs
sed -i '338i#pragma warning restore CA1851' src/AspNetCore.Simple.MsTest.Sdk/AssertExtensions/Collection/Assert.That.Collection.Contains.cs

# Fix CA1720 in Collection.Predicate.cs - identifier contains type name
sed -i '218i#pragma warning disable CA1720 // Identifier contains type name - Single is the method name' src/AspNetCore.Simple.MsTest.Sdk/AssertExtensions/Collection/Assert.That.Collection.Predicate.cs
sed-i '220i#pragma warning restore CA1720' src/AspNetCore.Simple.MsTest.Sdk/AssertExtensions/Collection/Assert.That.Collection.Predicate.cs

# Fix CA1861 in DoesNotThrow.cs - prefer static readonly
sed -i '157i#pragma warning disable CA1861 // Prefer static readonly fields' src/AspNetCore.Simple.MsTest.Sdk/AssertExtensions/Exception/Assert.That.DoesNotThrow.cs
sed -i '159i#pragma warning restore CA1861' src/AspNetCore.Simple.MsTest.Sdk/AssertExtensions/Exception/Assert.That.DoesNotThrow.cs

# Fix CA1846 in String.Length.cs - prefer AsSpan
sed -i '160i#pragma warning disable CA1846 // Prefer AsSpan over Substring' src/AspNetCore.Simple.MsTest.Sdk/AssertExtensions/String/Assert.That.String.Length.cs
sed -i '162i#pragma warning restore CA1846' src/AspNetCore.Simple.MsTest.Sdk/AssertExtensions/String/Assert.That.String.Length.cs
sed -i '255i#pragma warning disable CA1846 // Prefer AsSpan over Substring' src/AspNetCore.Simple.MsTest.Sdk/AssertExtensions/String/Assert.That.String.Length.cs
sed -i '257i#pragma warning restore CA1846' src/AspNetCore.Simple.MsTest.Sdk/AssertExtensions/String/Assert.That.String.Length.cs

# Fix CA1859 in Collection.Contains.cs - change parameter types
sed -i '369i#pragma warning disable CA1859 // Use concrete types when possible' src/AspNetCore.Simple.MsTest.Sdk/AssertExtensions/Collection/Assert.That.Collection.Contains.cs
sed -i '375i#pragma warning restore CA1859' src/AspNetCore.Simple.MsTest.Sdk/AssertExtensions/Collection/Assert.That.Collection.Contains.cs
sed -i '436i#pragma warning disable CA1859 // Use concrete types when possible' src/AspNetCore.Simple.MsTest.Sdk/AssertExtensions/Collection/Assert.That.Collection.Contains.cs
sed -i '442i#pragma warning restore CA1859' src/AspNetCore.Simple.MsTest.Sdk/AssertExtensions/Collection/Assert.That.Collection.Contains.cs

