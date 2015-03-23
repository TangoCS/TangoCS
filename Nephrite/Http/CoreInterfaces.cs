// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nephrite.Http
{
	public interface IApplicationBuilder
	{
		IServiceProvider ApplicationServices { get; set; }

		IServerInformation Server { get; set; }

		IDictionary<string, object> Properties { get; set; }

		IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware);

		IApplicationBuilder New();

		RequestDelegate Build();
	}

	// TODO: [AssemblyNeutral]
	public interface IServerInformation
	{
		string Name { get; }
	}

	public delegate Task RequestDelegate(AbstractHttpContext context);

	public interface ISessionCollection : IEnumerable<KeyValuePair<string, byte[]>>
	{
		byte[] this[string key] { get; set; }

		bool TryGetValue(string key, out byte[] value);

		void Set(string key, ArraySegment<byte> value);

		void Remove(string key);

		void Clear();
	}

	/// <summary>
	/// Contains the parsed form values.
	/// </summary>
	public interface IFormCollection : IReadableStringCollection
	{
		IFormFileCollection Files { get; }
	}

	public interface IFormFileCollection : IList<IFormFile>
	{
		IFormFile this[string name] { get; }

		IFormFile GetFile(string name);

		IList<IFormFile> GetFiles(string name);
	}

	public interface IFormFile
	{
		string ContentType { get; }

		string ContentDisposition { get; }

		IHeaderDictionary Headers { get; }

		long Length { get; }

		Stream OpenReadStream();
	}

	/// <summary>
	/// Represents request and response headers
	/// </summary>
	public interface IHeaderDictionary : IReadableStringCollection, IDictionary<string, string[]>
	{
		/// <summary>
		/// Get or sets the associated value from the collection as a single string.
		/// </summary>
		/// <param name="key">The header name.</param>
		/// <returns>the associated value from the collection as a single string or null if the key is not present.</returns>
		new string this[string key] { get; set; }

		// This property is duplicated to resolve an ambiguity between IReadableStringCollection.Count and IDictionary<string, string[]>.Count
		/// <summary>
		/// Gets the number of elements contained in the collection.
		/// </summary>
		new int Count { get; }

		// This property is duplicated to resolve an ambiguity between IReadableStringCollection.Keys and IDictionary<string, string[]>.Keys
		/// <summary>
		/// Gets a collection containing the keys.
		/// </summary>
		new ICollection<string> Keys { get; }

		/// <summary>
		/// Get the associated values from the collection separated into individual values.
		/// Quoted values will not be split, and the quotes will be removed.
		/// </summary>
		/// <param name="key">The header name.</param>
		/// <returns>the associated values from the collection separated into individual values, or null if the key is not present.</returns>
		IList<string> GetCommaSeparatedValues(string key);

		/// <summary>
		/// Add a new value. Appends to the header if already present
		/// </summary>
		/// <param name="key">The header name.</param>
		/// <param name="value">The header value.</param>
		void Append(string key, string value);

		/// <summary>
		/// Add new values. Each item remains a separate array entry.
		/// </summary>
		/// <param name="key">The header name.</param>
		/// <param name="values">The header values.</param>
		void AppendValues(string key, params string[] values);

		/// <summary>
		/// Quotes any values containing comas, and then coma joins all of the values with any existing values.
		/// </summary>
		/// <param name="key">The header name.</param>
		/// <param name="values">The header values.</param>
		void AppendCommaSeparatedValues(string key, params string[] values);

		/// <summary>
		/// Sets a specific header value.
		/// </summary>
		/// <param name="key">The header name.</param>
		/// <param name="value">The header value.</param>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Set", Justification = "Re-evaluate later.")]
		void Set(string key, string value);

		/// <summary>
		/// Sets the specified header values without modification.
		/// </summary>
		/// <param name="key">The header name.</param>
		/// <param name="values">The header values.</param>
		void SetValues(string key, params string[] values);

		/// <summary>
		/// Quotes any values containing comas, and then coma joins all of the values.
		/// </summary>
		/// <param name="key">The header name.</param>
		/// <param name="values">The header values.</param>
		void SetCommaSeparatedValues(string key, params string[] values);
	}

	/// <summary>
	/// A wrapper for the response Set-Cookie header
	/// </summary>
	public interface IResponseCookies
	{
		/// <summary>
		/// Add a new cookie and value
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		void Append(string key, string value);

		/// <summary>
		/// Add a new cookie
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="options"></param>
		void Append(string key, string value, CookieOptions options);

		/// <summary>
		/// Sets an expired cookie
		/// </summary>
		/// <param name="key"></param>
		void Delete(string key);

		/// <summary>
		/// Sets an expired cookie
		/// </summary>
		/// <param name="key"></param>
		/// <param name="options"></param>
		void Delete(string key, CookieOptions options);
	}


	public interface IReadableStringCollection : IEnumerable<KeyValuePair<string, string[]>>
	{
		/// <summary>
		/// Get the associated value from the collection.  Multiple values will be merged.
		/// Returns null if the key is not present.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		string this[string key] { get; }

		/// <summary>
		/// Gets the number of elements contained in the collection.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Gets a collection containing the keys.
		/// </summary>
		ICollection<string> Keys { get; }

		/// <summary>
		/// Determines whether the collection contains an element with the specified key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		bool ContainsKey(string key);

		/// <summary>
		/// Get the associated value from the collection.  Multiple values will be merged.
		/// Returns null if the key is not present.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		string Get(string key);

		/// <summary>
		/// Get the associated values from the collection in their original format.
		/// Returns null if the key is not present.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		IList<string> GetValues(string key);
	}

}
