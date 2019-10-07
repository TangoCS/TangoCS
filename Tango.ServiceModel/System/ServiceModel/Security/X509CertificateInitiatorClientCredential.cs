using System;
using System.Security.Cryptography.X509Certificates;

namespace System.ServiceModel.Security
{
	/// <summary>Defines a certificate used by a client to identify itself.</summary>
	public sealed class X509CertificateInitiatorClientCredential
	{
		//internal const StoreLocation DefaultStoreLocation = StoreLocation.CurrentUser;

		//internal const StoreName DefaultStoreName = StoreName.My;

		//internal const X509FindType DefaultFindType = X509FindType.FindBySubjectDistinguishedName;

		private X509Certificate2 certificate;

		private bool isReadOnly;

		/// <summary>Gets or sets the certificate to use to represent the service when communicating back to the client.</summary>
		/// <returns>The <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" /> to use to represent the service when communicating back to the client.</returns>
		/// <exception cref="T:System.InvalidOperationException">A set method was used when the credential is read-only.</exception>
		public X509Certificate2 Certificate
		{
			get
			{
				return this.certificate;
			}
			set
			{
				//this.ThrowIfImmutable();
				this.certificate = value;
			}
		}

		internal X509CertificateInitiatorClientCredential()
		{
		}

		internal X509CertificateInitiatorClientCredential(X509CertificateInitiatorClientCredential other)
		{
			this.certificate = other.certificate;
			this.isReadOnly = other.isReadOnly;
		}

		/// <summary>Allows you to specify the certificate to use to represent the service by specifying the subject distinguished name.</summary>
		/// <param name="subjectName">Subject distinguished name.</param>
		/// <param name="storeLocation">The location of the certificate store the service uses to obtain the service certificate.</param>
		/// <param name="storeName">Specifies the name of the X.509 certificate store to open.</param>
		//public void SetCertificate(string subjectName, StoreLocation storeLocation, StoreName storeName)
		//{
		//	if (subjectName == null)
		//	{
		//		throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("subjectName");
		//	}
		//	this.SetCertificate(storeLocation, storeName, X509FindType.FindBySubjectDistinguishedName, subjectName);
		//}

		/// <summary>Allows you to specify the certificate to use to represent the client by specifying query parameters such as <paramref name="storeLocation" />, <paramref name="storeName" />, <paramref name="findType" /> and <paramref name="findValue" />.</summary>
		/// <param name="storeLocation">The location of the certificate store the client uses to obtain the client certificate.</param>
		/// <param name="storeName">Specifies the name of the X.509 certificate store to open.</param>
		/// <param name="findType">Defines the type of X.509 search to be executed.</param>
		/// <param name="findValue">The value to search for in the X.509 certificate store.</param>
		//public void SetCertificate(StoreLocation storeLocation, StoreName storeName, X509FindType findType, object findValue)
		//{
		//	if (findValue == null)
		//	{
		//		throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("findValue");
		//	}
		//	this.ThrowIfImmutable();
		//	this.certificate = SecurityUtils.GetCertificateFromStore(storeName, storeLocation, findType, findValue, null);
		//}

		internal void MakeReadOnly()
		{
			this.isReadOnly = true;
		}

		//private void ThrowIfImmutable()
		//{
		//	if (this.isReadOnly)
		//	{
		//		throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.GetString("ObjectIsReadOnly")));
		//	}
		//}
	}
}
