//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Cache
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial interface IQueryCache
	{

		/// <summary>
		/// Clear the cache.
		/// </summary>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		Task ClearAsync(CancellationToken cancellationToken);

		// Since 5.2
		[Obsolete("Have the query cache implement IBatchableQueryCache, and use IBatchableQueryCache.Put")]
		Task<bool> PutAsync(QueryKey key, ICacheAssembler[] returnTypes, IList result, bool isNaturalKeyLookup, ISessionImplementor session, CancellationToken cancellationToken);

		// Since 5.2
		[Obsolete("Have the query cache implement IBatchableQueryCache, and use IBatchableQueryCache.Get")]
		Task<IList> GetAsync(QueryKey key, ICacheAssembler[] returnTypes, bool isNaturalKeyLookup, ISet<string> spaces, ISessionImplementor session, CancellationToken cancellationToken);
	}

	public partial interface IBatchableQueryCache : IQueryCache
	{
		/// <summary>
		/// Get query results from the cache.
		/// </summary>
		/// <param name="key">The query key.</param>
		/// <param name="queryParameters">The query parameters.</param>
		/// <param name="returnTypes">The query result row types.</param>
		/// <param name="spaces">The query spaces.</param>
		/// <param name="session">The session for which the query is executed.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <returns>The query results, if cached.</returns>
		Task<IList> GetAsync(
			QueryKey key, QueryParameters queryParameters, ICacheAssembler[] returnTypes, ISet<string> spaces,
			ISessionImplementor session, CancellationToken cancellationToken);

		/// <summary>
		/// Put query results in the cache.
		/// </summary>
		/// <param name="key">The query key.</param>
		/// <param name="queryParameters">The query parameters.</param>
		/// <param name="returnTypes">The query result row types.</param>
		/// <param name="result">The query result.</param>
		/// <param name="session">The session for which the query was executed.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <returns><see langword="true" /> if the result has been cached, <see langword="false" />
		/// otherwise.</returns>
		Task<bool> PutAsync(
			QueryKey key, QueryParameters queryParameters, ICacheAssembler[] returnTypes, IList result,
			ISessionImplementor session, CancellationToken cancellationToken);

		/// <summary>
		/// Retrieve multiple query results from the cache.
		/// </summary>
		/// <param name="keys">The query keys.</param>
		/// <param name="queryParameters">The array of query parameters matching <paramref name="keys"/>.</param>
		/// <param name="returnTypes">The array of query result row types matching <paramref name="keys"/>.</param>
		/// <param name="spaces">The array of query spaces matching <paramref name="keys"/>.</param>
		/// <param name="session">The session for which the queries are executed.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <returns>The cached query results, matching each key of <paramref name="keys"/> respectively. For each
		/// missed key, it will contain a <see langword="null" />.</returns>
		Task<IList[]> GetManyAsync(
			QueryKey[] keys, QueryParameters[] queryParameters, ICacheAssembler[][] returnTypes,
			ISet<string>[] spaces, ISessionImplementor session, CancellationToken cancellationToken);

		/// <summary>
		/// Attempt to cache objects, after loading them from the database.
		/// </summary>
		/// <param name="keys">The query keys.</param>
		/// <param name="queryParameters">The array of query parameters matching <paramref name="keys"/>.</param>
		/// <param name="returnTypes">The array of query result row types matching <paramref name="keys"/>.</param>
		/// <param name="results">The array of query results matching <paramref name="keys"/>.</param>
		/// <param name="session">The session for which the queries were executed.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <returns>An array of boolean indicating if each query was successfully cached.</returns>
		/// <exception cref="CacheException"></exception>
		Task<bool[]> PutManyAsync(
			QueryKey[] keys, QueryParameters[] queryParameters, ICacheAssembler[][] returnTypes, IList[] results,
			ISessionImplementor session, CancellationToken cancellationToken);
	}

	internal static partial class QueryCacheExtensions
	{

		/// <summary>
		/// Get query results from the cache.
		/// </summary>
		/// <param name="queryCache">The cache.</param>
		/// <param name="key">The query key.</param>
		/// <param name="queryParameters">The query parameters.</param>
		/// <param name="returnTypes">The query result row types.</param>
		/// <param name="spaces">The query spaces.</param>
		/// <param name="session">The session for which the query is executed.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <returns>The query results, if cached.</returns>
		public static async Task<IList> GetAsync(
			this IQueryCache queryCache,
			QueryKey key,
			QueryParameters queryParameters,
			ICacheAssembler[] returnTypes,
			ISet<string> spaces,
			ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (queryCache is IBatchableQueryCache batchableQueryCache)
			{
				return await (batchableQueryCache.GetAsync(
					key,
					queryParameters,
					returnTypes,
					spaces,
					session, cancellationToken)).ConfigureAwait(false);
			}

			if (!_hasWarnForObsoleteQueryCache)
			{
				_hasWarnForObsoleteQueryCache = true;
				Log.Warn("{0} is obsolete, it should implement {1}", queryCache, nameof(IBatchableQueryCache));
			}

			var persistenceContext = session.PersistenceContext;

			var defaultReadOnlyOrig = persistenceContext.DefaultReadOnly;

			if (queryParameters.IsReadOnlyInitialized)
				persistenceContext.DefaultReadOnly = queryParameters.ReadOnly;
			else
				queryParameters.ReadOnly = persistenceContext.DefaultReadOnly;

			try
			{
#pragma warning disable 618
				return await (queryCache.GetAsync(
#pragma warning restore 618
					key,
					returnTypes,
					queryParameters.NaturalKeyLookup,
					spaces,
					session, cancellationToken)).ConfigureAwait(false);
			}
			finally
			{
				persistenceContext.DefaultReadOnly = defaultReadOnlyOrig;
			}
		}

		/// <summary>
		/// Put query results in the cache.
		/// </summary>
		/// <param name="queryCache">The cache.</param>
		/// <param name="key">The query key.</param>
		/// <param name="queryParameters">The query parameters.</param>
		/// <param name="returnTypes">The query result row types.</param>
		/// <param name="result">The query result.</param>
		/// <param name="session">The session for which the query was executed.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <returns><see langword="true" /> if the result has been cached, <see langword="false" />
		/// otherwise.</returns>
		public static Task<bool> PutAsync(
			this IQueryCache queryCache,
			QueryKey key,
			QueryParameters queryParameters,
			ICacheAssembler[] returnTypes,
			IList result,
			ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<bool>(cancellationToken);
			}
			try
			{
				if (queryCache is IBatchableQueryCache batchableQueryCache)
				{
					return batchableQueryCache.PutAsync(
					key, queryParameters,
					returnTypes,
					result, session, cancellationToken);
				}

#pragma warning disable 618
				return queryCache.PutAsync(
#pragma warning restore 618
				key,
				returnTypes,
				result,
				queryParameters.NaturalKeyLookup,
				session, cancellationToken);
			}
			catch (Exception ex)
			{
				return Task.FromException<bool>(ex);
			}
		}

		/// <summary>
		/// Retrieve multiple query results from the cache.
		/// </summary>
		/// <param name="queryCache">The cache.</param>
		/// <param name="keys">The query keys.</param>
		/// <param name="queryParameters">The array of query parameters matching <paramref name="keys"/>.</param>
		/// <param name="returnTypes">The array of query result row types matching <paramref name="keys"/>.</param>
		/// <param name="spaces">The array of query spaces matching <paramref name="keys"/>.</param>
		/// <param name="session">The session for which the queries are executed.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <returns>The cached query results, matching each key of <paramref name="keys"/> respectively. For each
		/// missed key, it will contain a <see langword="null" />.</returns>
		public static async Task<IList[]> GetManyAsync(
			this IQueryCache queryCache,
			QueryKey[] keys,
			QueryParameters[] queryParameters,
			ICacheAssembler[][] returnTypes,
			ISet<string>[] spaces,
			ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (queryCache is IBatchableQueryCache batchableQueryCache)
			{
				return await (batchableQueryCache.GetManyAsync(
					keys,
					queryParameters,
					returnTypes,
					spaces,
					session, cancellationToken)).ConfigureAwait(false);
			}

			var results = new IList[keys.Length];
			for (var i = 0; i < keys.Length; i++)
			{
				results[i] = await (queryCache.GetAsync(keys[i], queryParameters[i], returnTypes[i], spaces[i], session, cancellationToken)).ConfigureAwait(false);
			}

			return results;
		}

		/// <summary>
		/// Attempt to cache objects, after loading them from the database.
		/// </summary>
		/// <param name="queryCache">The cache.</param>
		/// <param name="keys">The query keys.</param>
		/// <param name="queryParameters">The array of query parameters matching <paramref name="keys"/>.</param>
		/// <param name="returnTypes">The array of query result row types matching <paramref name="keys"/>.</param>
		/// <param name="results">The array of query results matching <paramref name="keys"/>.</param>
		/// <param name="session">The session for which the queries were executed.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <returns>An array of boolean indicating if each query was successfully cached.</returns>
		/// <exception cref="CacheException"></exception>
		public static async Task<bool[]> PutManyAsync(
			this IQueryCache queryCache,
			QueryKey[] keys,
			QueryParameters[] queryParameters,
			ICacheAssembler[][] returnTypes,
			IList[] results,
			ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (queryCache is IBatchableQueryCache batchableQueryCache)
			{
				return await (batchableQueryCache.PutManyAsync(
					keys,
					queryParameters,
					returnTypes,
					results,
					session, cancellationToken)).ConfigureAwait(false);
			}

			var puts = new bool[keys.Length];
			for (var i = 0; i < keys.Length; i++)
			{
				puts[i] = await (queryCache.PutAsync(keys[i], queryParameters[i], returnTypes[i], results[i], session, cancellationToken)).ConfigureAwait(false);
			}

			return puts;
		}
	}
}
