using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Net;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.Xml;

namespace System.ServiceModel
{
	/// <summary>Manages security tokens for the client.</summary>
	public class ClientCredentialsSecurityTokenManager : SecurityTokenManager
	{
		/*internal class KerberosSecurityTokenProviderWrapper : CommunicationObjectSecurityTokenProvider
		{
			//private KerberosSecurityTokenProvider innerProvider;

			//private SafeFreeCredentials credentialsHandle;

			private bool ownCredentialsHandle;

			public KerberosSecurityTokenProviderWrapper(KerberosSecurityTokenProvider innerProvider, SafeFreeCredentials credentialsHandle)
			{
				this.innerProvider = innerProvider;
				this.credentialsHandle = credentialsHandle;
			}

			public override void OnOpening()
			{
				base.OnOpening();
				if (this.credentialsHandle == null)
				{
					this.credentialsHandle = SecurityUtils.GetCredentialsHandle("Kerberos", this.innerProvider.NetworkCredential, false, new string[0]);
					this.ownCredentialsHandle = true;
				}
			}

			public override void OnClose(TimeSpan timeout)
			{
				base.OnClose(timeout);
				this.FreeCredentialsHandle();
			}

			public override void OnAbort()
			{
				base.OnAbort();
				this.FreeCredentialsHandle();
			}

			private void FreeCredentialsHandle()
			{
				if (this.credentialsHandle != null)
				{
					if (this.ownCredentialsHandle)
					{
						this.credentialsHandle.Close();
					}
					this.credentialsHandle = null;
				}
			}

			internal SecurityToken GetToken(TimeSpan timeout, ChannelBinding channelbinding)
			{
				return new KerberosRequestorSecurityToken(this.innerProvider.ServicePrincipalName, this.innerProvider.TokenImpersonationLevel, this.innerProvider.NetworkCredential, SecurityUniqueId.Create().Value, this.credentialsHandle, channelbinding);
			}

			protected override SecurityToken GetTokenCore(TimeSpan timeout)
			{
				return this.GetToken(timeout, null);
			}
		}*/

		private ClientCredentials parent;

		/// <summary>Gets the client credentials.</summary>
		/// <returns>The <see cref="T:System.ServiceModel.Description.ClientCredentials" />.</returns>
		public ClientCredentials ClientCredentials
		{
			get
			{
				return this.parent;
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.ClientCredentialsSecurityTokenManager" /> class. </summary>
		/// <param name="clientCredentials">The <see cref="T:System.ServiceModel.Description.ClientCredentials" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="clientCredentials" /> is null.</exception>
		public ClientCredentialsSecurityTokenManager(ClientCredentials clientCredentials)
		{
			if (clientCredentials == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("clientCredentials");
			}
			this.parent = clientCredentials;
		}

		/*private string GetServicePrincipalName(InitiatorServiceModelSecurityTokenRequirement initiatorRequirement)
		{
			EndpointAddress targetAddress = initiatorRequirement.TargetAddress;
			if (targetAddress == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.GetString("TokenRequirementDoesNotSpecifyTargetAddress", new object[]
				{
					initiatorRequirement
				}));
			}
			SecurityBindingElement securityBindingElement = initiatorRequirement.SecurityBindingElement;
			IdentityVerifier identityVerifier;
			if (securityBindingElement != null)
			{
				identityVerifier = securityBindingElement.LocalClientSettings.IdentityVerifier;
			}
			else
			{
				identityVerifier = IdentityVerifier.CreateDefault();
			}
			EndpointIdentity identity;
			identityVerifier.TryGetIdentity(targetAddress, out identity);
			return SecurityUtils.GetSpnFromIdentity(identity, targetAddress);
		}*/

		/*private SspiSecurityToken GetSpnegoClientCredential(InitiatorServiceModelSecurityTokenRequirement initiatorRequirement)
		{
			InitiatorServiceModelSecurityTokenRequirement initiatorServiceModelSecurityTokenRequirement = new InitiatorServiceModelSecurityTokenRequirement();
			initiatorServiceModelSecurityTokenRequirement.TargetAddress = initiatorRequirement.TargetAddress;
			initiatorServiceModelSecurityTokenRequirement.TokenType = ServiceModelSecurityTokenTypes.SspiCredential;
			initiatorServiceModelSecurityTokenRequirement.Via = initiatorRequirement.Via;
			initiatorServiceModelSecurityTokenRequirement.RequireCryptographicToken = false;
			initiatorServiceModelSecurityTokenRequirement.SecurityBindingElement = initiatorRequirement.SecurityBindingElement;
			initiatorServiceModelSecurityTokenRequirement.MessageSecurityVersion = initiatorRequirement.MessageSecurityVersion;
			ChannelParameterCollection value;
			if (initiatorRequirement.TryGetProperty<ChannelParameterCollection>(ServiceModelSecurityTokenRequirement.ChannelParametersCollectionProperty, out value))
			{
				initiatorServiceModelSecurityTokenRequirement.Properties[ServiceModelSecurityTokenRequirement.ChannelParametersCollectionProperty] = value;
			}
			SecurityTokenProvider securityTokenProvider = this.CreateSecurityTokenProvider(initiatorServiceModelSecurityTokenRequirement);
			SecurityUtils.OpenTokenProviderIfRequired(securityTokenProvider, TimeSpan.Zero);
			SspiSecurityToken result = (SspiSecurityToken)securityTokenProvider.GetToken(TimeSpan.Zero);
			SecurityUtils.AbortTokenProviderIfRequired(securityTokenProvider);
			return result;
		}*/

		/*private SecurityTokenProvider CreateSpnegoTokenProvider(InitiatorServiceModelSecurityTokenRequirement initiatorRequirement)
		{
			EndpointAddress targetAddress = initiatorRequirement.TargetAddress;
			if (targetAddress == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.GetString("TokenRequirementDoesNotSpecifyTargetAddress", new object[]
				{
					initiatorRequirement
				}));
			}
			SecurityBindingElement securityBindingElement = initiatorRequirement.SecurityBindingElement;
			if (securityBindingElement == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.GetString("TokenProviderRequiresSecurityBindingElement", new object[]
				{
					initiatorRequirement
				}));
			}
			SspiIssuanceChannelParameter sspiIssuanceChannelParameter = this.GetSspiIssuanceChannelParameter(initiatorRequirement);
			bool flag = sspiIssuanceChannelParameter == null || sspiIssuanceChannelParameter.GetTokenOnOpen;
			LocalClientSecuritySettings localClientSettings = securityBindingElement.LocalClientSettings;
			BindingContext property = initiatorRequirement.GetProperty<BindingContext>(ServiceModelSecurityTokenRequirement.IssuerBindingContextProperty);
			SpnegoTokenProvider spnegoTokenProvider = new SpnegoTokenProvider((sspiIssuanceChannelParameter != null) ? sspiIssuanceChannelParameter.CredentialsHandle : null, securityBindingElement);
			SspiSecurityToken spnegoClientCredential = this.GetSpnegoClientCredential(initiatorRequirement);
			spnegoTokenProvider.ClientCredential = spnegoClientCredential.NetworkCredential;
			spnegoTokenProvider.IssuerAddress = initiatorRequirement.IssuerAddress;
			spnegoTokenProvider.AllowedImpersonationLevel = this.parent.Windows.AllowedImpersonationLevel;
			spnegoTokenProvider.AllowNtlm = spnegoClientCredential.AllowNtlm;
			spnegoTokenProvider.IdentityVerifier = localClientSettings.IdentityVerifier;
			spnegoTokenProvider.SecurityAlgorithmSuite = initiatorRequirement.SecurityAlgorithmSuite;
			spnegoTokenProvider.AuthenticateServer = !initiatorRequirement.Properties.ContainsKey(ServiceModelSecurityTokenRequirement.SupportingTokenAttachmentModeProperty);
			spnegoTokenProvider.NegotiateTokenOnOpen = flag;
			spnegoTokenProvider.CacheServiceTokens = (flag || localClientSettings.CacheCookies);
			spnegoTokenProvider.IssuerBindingContext = property;
			spnegoTokenProvider.MaxServiceTokenCachingTime = localClientSettings.MaxCookieCachingTime;
			spnegoTokenProvider.ServiceTokenValidityThresholdPercentage = localClientSettings.CookieRenewalThresholdPercentage;
			spnegoTokenProvider.StandardsManager = SecurityUtils.CreateSecurityStandardsManager(initiatorRequirement, this);
			spnegoTokenProvider.TargetAddress = targetAddress;
			spnegoTokenProvider.Via = initiatorRequirement.GetPropertyOrDefault<Uri>(ServiceModelSecurityTokenRequirement.ViaProperty, null);
			spnegoTokenProvider.ApplicationProtectionRequirements = ((property != null) ? property.BindingParameters.Find<ChannelProtectionRequirements>() : null);
			spnegoTokenProvider.InteractiveNegoExLogonEnabled = this.ClientCredentials.SupportInteractive;
			return spnegoTokenProvider;
		}*/

		/*private SecurityTokenProvider CreateTlsnegoClientX509TokenProvider(InitiatorServiceModelSecurityTokenRequirement initiatorRequirement)
		{
			InitiatorServiceModelSecurityTokenRequirement initiatorServiceModelSecurityTokenRequirement = new InitiatorServiceModelSecurityTokenRequirement();
			initiatorServiceModelSecurityTokenRequirement.TokenType = SecurityTokenTypes.X509Certificate;
			initiatorServiceModelSecurityTokenRequirement.TargetAddress = initiatorRequirement.TargetAddress;
			initiatorServiceModelSecurityTokenRequirement.SecurityBindingElement = initiatorRequirement.SecurityBindingElement;
			initiatorServiceModelSecurityTokenRequirement.SecurityAlgorithmSuite = initiatorRequirement.SecurityAlgorithmSuite;
			initiatorServiceModelSecurityTokenRequirement.RequireCryptographicToken = true;
			initiatorServiceModelSecurityTokenRequirement.MessageSecurityVersion = initiatorRequirement.MessageSecurityVersion;
			initiatorServiceModelSecurityTokenRequirement.KeyUsage = SecurityKeyUsage.Signature;
			initiatorServiceModelSecurityTokenRequirement.KeyType = SecurityKeyType.AsymmetricKey;
			initiatorServiceModelSecurityTokenRequirement.Properties[ServiceModelSecurityTokenRequirement.MessageDirectionProperty] = MessageDirection.Output;
			ChannelParameterCollection value;
			if (initiatorRequirement.TryGetProperty<ChannelParameterCollection>(ServiceModelSecurityTokenRequirement.ChannelParametersCollectionProperty, out value))
			{
				initiatorServiceModelSecurityTokenRequirement.Properties[ServiceModelSecurityTokenRequirement.ChannelParametersCollectionProperty] = value;
			}
			return this.CreateSecurityTokenProvider(initiatorServiceModelSecurityTokenRequirement);
		}*/

		/*private SecurityTokenAuthenticator CreateTlsnegoServerX509TokenAuthenticator(InitiatorServiceModelSecurityTokenRequirement initiatorRequirement)
		{
			InitiatorServiceModelSecurityTokenRequirement initiatorServiceModelSecurityTokenRequirement = new InitiatorServiceModelSecurityTokenRequirement();
			initiatorServiceModelSecurityTokenRequirement.TokenType = SecurityTokenTypes.X509Certificate;
			initiatorServiceModelSecurityTokenRequirement.RequireCryptographicToken = true;
			initiatorServiceModelSecurityTokenRequirement.SecurityBindingElement = initiatorRequirement.SecurityBindingElement;
			initiatorServiceModelSecurityTokenRequirement.MessageSecurityVersion = initiatorRequirement.MessageSecurityVersion;
			initiatorServiceModelSecurityTokenRequirement.KeyUsage = SecurityKeyUsage.Exchange;
			initiatorServiceModelSecurityTokenRequirement.KeyType = SecurityKeyType.AsymmetricKey;
			initiatorServiceModelSecurityTokenRequirement.Properties[ServiceModelSecurityTokenRequirement.MessageDirectionProperty] = MessageDirection.Input;
			ChannelParameterCollection value;
			if (initiatorRequirement.TryGetProperty<ChannelParameterCollection>(ServiceModelSecurityTokenRequirement.ChannelParametersCollectionProperty, out value))
			{
				initiatorServiceModelSecurityTokenRequirement.Properties[ServiceModelSecurityTokenRequirement.ChannelParametersCollectionProperty] = value;
			}
			SecurityTokenResolver securityTokenResolver;
			return this.CreateSecurityTokenAuthenticator(initiatorServiceModelSecurityTokenRequirement, out securityTokenResolver);
		}*/

		/*private SspiIssuanceChannelParameter GetSspiIssuanceChannelParameter(SecurityTokenRequirement initiatorRequirement)
		{
			ChannelParameterCollection channelParameterCollection;
			if (initiatorRequirement.TryGetProperty<ChannelParameterCollection>(ServiceModelSecurityTokenRequirement.ChannelParametersCollectionProperty, out channelParameterCollection) && channelParameterCollection != null)
			{
				for (int i = 0; i < channelParameterCollection.Count; i++)
				{
					if (channelParameterCollection[i] is SspiIssuanceChannelParameter)
					{
						return (SspiIssuanceChannelParameter)channelParameterCollection[i];
					}
				}
			}
			return null;
		}*/

		/*private SecurityTokenProvider CreateTlsnegoTokenProvider(InitiatorServiceModelSecurityTokenRequirement initiatorRequirement, bool requireClientCertificate)
		{
			EndpointAddress targetAddress = initiatorRequirement.TargetAddress;
			if (targetAddress == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.GetString("TokenRequirementDoesNotSpecifyTargetAddress", new object[]
				{
					initiatorRequirement
				}));
			}
			SecurityBindingElement securityBindingElement = initiatorRequirement.SecurityBindingElement;
			if (securityBindingElement == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.GetString("TokenProviderRequiresSecurityBindingElement", new object[]
				{
					initiatorRequirement
				}));
			}
			SspiIssuanceChannelParameter sspiIssuanceChannelParameter = this.GetSspiIssuanceChannelParameter(initiatorRequirement);
			bool flag = sspiIssuanceChannelParameter != null && sspiIssuanceChannelParameter.GetTokenOnOpen;
			LocalClientSecuritySettings localClientSettings = securityBindingElement.LocalClientSettings;
			BindingContext property = initiatorRequirement.GetProperty<BindingContext>(ServiceModelSecurityTokenRequirement.IssuerBindingContextProperty);
			TlsnegoTokenProvider tlsnegoTokenProvider = new TlsnegoTokenProvider();
			tlsnegoTokenProvider.IssuerAddress = initiatorRequirement.IssuerAddress;
			tlsnegoTokenProvider.NegotiateTokenOnOpen = flag;
			tlsnegoTokenProvider.CacheServiceTokens = (flag || localClientSettings.CacheCookies);
			if (requireClientCertificate)
			{
				tlsnegoTokenProvider.ClientTokenProvider = this.CreateTlsnegoClientX509TokenProvider(initiatorRequirement);
			}
			tlsnegoTokenProvider.IssuerBindingContext = property;
			tlsnegoTokenProvider.ApplicationProtectionRequirements = ((property != null) ? property.BindingParameters.Find<ChannelProtectionRequirements>() : null);
			tlsnegoTokenProvider.MaxServiceTokenCachingTime = localClientSettings.MaxCookieCachingTime;
			tlsnegoTokenProvider.SecurityAlgorithmSuite = initiatorRequirement.SecurityAlgorithmSuite;
			tlsnegoTokenProvider.ServerTokenAuthenticator = this.CreateTlsnegoServerX509TokenAuthenticator(initiatorRequirement);
			tlsnegoTokenProvider.ServiceTokenValidityThresholdPercentage = localClientSettings.CookieRenewalThresholdPercentage;
			tlsnegoTokenProvider.StandardsManager = SecurityUtils.CreateSecurityStandardsManager(initiatorRequirement, this);
			tlsnegoTokenProvider.TargetAddress = initiatorRequirement.TargetAddress;
			tlsnegoTokenProvider.Via = initiatorRequirement.GetPropertyOrDefault<Uri>(ServiceModelSecurityTokenRequirement.ViaProperty, null);
			return tlsnegoTokenProvider;
		}*/

		/*private SecurityTokenProvider CreateSecureConversationSecurityTokenProvider(InitiatorServiceModelSecurityTokenRequirement initiatorRequirement)
		{
			EndpointAddress targetAddress = initiatorRequirement.TargetAddress;
			if (targetAddress == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.GetString("TokenRequirementDoesNotSpecifyTargetAddress", new object[]
				{
					initiatorRequirement
				}));
			}
			SecurityBindingElement securityBindingElement = initiatorRequirement.SecurityBindingElement;
			if (securityBindingElement == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.GetString("TokenProviderRequiresSecurityBindingElement", new object[]
				{
					initiatorRequirement
				}));
			}
			LocalClientSecuritySettings localClientSettings = securityBindingElement.LocalClientSettings;
			BindingContext property = initiatorRequirement.GetProperty<BindingContext>(ServiceModelSecurityTokenRequirement.IssuerBindingContextProperty);
			ChannelParameterCollection propertyOrDefault = initiatorRequirement.GetPropertyOrDefault<ChannelParameterCollection>(ServiceModelSecurityTokenRequirement.ChannelParametersCollectionProperty, null);
			bool supportSecurityContextCancellation = initiatorRequirement.SupportSecurityContextCancellation;
			if (supportSecurityContextCancellation)
			{
				SecuritySessionSecurityTokenProvider securitySessionSecurityTokenProvider = new SecuritySessionSecurityTokenProvider(this.GetCredentialsHandle(initiatorRequirement));
				securitySessionSecurityTokenProvider.BootstrapSecurityBindingElement = SecurityUtils.GetIssuerSecurityBindingElement(initiatorRequirement);
				securitySessionSecurityTokenProvider.IssuedSecurityTokenParameters = initiatorRequirement.GetProperty<SecurityTokenParameters>(ServiceModelSecurityTokenRequirement.IssuedSecurityTokenParametersProperty);
				securitySessionSecurityTokenProvider.IssuerBindingContext = property;
				securitySessionSecurityTokenProvider.KeyEntropyMode = securityBindingElement.KeyEntropyMode;
				securitySessionSecurityTokenProvider.SecurityAlgorithmSuite = initiatorRequirement.SecurityAlgorithmSuite;
				securitySessionSecurityTokenProvider.StandardsManager = SecurityUtils.CreateSecurityStandardsManager(initiatorRequirement, this);
				securitySessionSecurityTokenProvider.TargetAddress = targetAddress;
				securitySessionSecurityTokenProvider.Via = initiatorRequirement.GetPropertyOrDefault<Uri>(ServiceModelSecurityTokenRequirement.ViaProperty, null);
				Uri privacyNoticeUri;
				if (initiatorRequirement.TryGetProperty<Uri>(ServiceModelSecurityTokenRequirement.PrivacyNoticeUriProperty, out privacyNoticeUri))
				{
					securitySessionSecurityTokenProvider.PrivacyNoticeUri = privacyNoticeUri;
				}
				int privacyNoticeVersion;
				if (initiatorRequirement.TryGetProperty<int>(ServiceModelSecurityTokenRequirement.PrivacyNoticeVersionProperty, out privacyNoticeVersion))
				{
					securitySessionSecurityTokenProvider.PrivacyNoticeVersion = privacyNoticeVersion;
				}
				EndpointAddress localAddress;
				if (initiatorRequirement.TryGetProperty<EndpointAddress>(ServiceModelSecurityTokenRequirement.DuplexClientLocalAddressProperty, out localAddress))
				{
					securitySessionSecurityTokenProvider.LocalAddress = localAddress;
				}
				securitySessionSecurityTokenProvider.ChannelParameters = propertyOrDefault;
				securitySessionSecurityTokenProvider.WebHeaders = initiatorRequirement.WebHeaders;
				return securitySessionSecurityTokenProvider;
			}
			AcceleratedTokenProvider acceleratedTokenProvider = new AcceleratedTokenProvider(this.GetCredentialsHandle(initiatorRequirement));
			acceleratedTokenProvider.IssuerAddress = initiatorRequirement.IssuerAddress;
			acceleratedTokenProvider.BootstrapSecurityBindingElement = SecurityUtils.GetIssuerSecurityBindingElement(initiatorRequirement);
			acceleratedTokenProvider.CacheServiceTokens = localClientSettings.CacheCookies;
			acceleratedTokenProvider.IssuerBindingContext = property;
			acceleratedTokenProvider.KeyEntropyMode = securityBindingElement.KeyEntropyMode;
			acceleratedTokenProvider.MaxServiceTokenCachingTime = localClientSettings.MaxCookieCachingTime;
			acceleratedTokenProvider.SecurityAlgorithmSuite = initiatorRequirement.SecurityAlgorithmSuite;
			acceleratedTokenProvider.ServiceTokenValidityThresholdPercentage = localClientSettings.CookieRenewalThresholdPercentage;
			acceleratedTokenProvider.StandardsManager = SecurityUtils.CreateSecurityStandardsManager(initiatorRequirement, this);
			acceleratedTokenProvider.TargetAddress = targetAddress;
			acceleratedTokenProvider.Via = initiatorRequirement.GetPropertyOrDefault<Uri>(ServiceModelSecurityTokenRequirement.ViaProperty, null);
			Uri privacyNoticeUri2;
			if (initiatorRequirement.TryGetProperty<Uri>(ServiceModelSecurityTokenRequirement.PrivacyNoticeUriProperty, out privacyNoticeUri2))
			{
				acceleratedTokenProvider.PrivacyNoticeUri = privacyNoticeUri2;
			}
			acceleratedTokenProvider.ChannelParameters = propertyOrDefault;
			int privacyNoticeVersion2;
			if (initiatorRequirement.TryGetProperty<int>(ServiceModelSecurityTokenRequirement.PrivacyNoticeVersionProperty, out privacyNoticeVersion2))
			{
				acceleratedTokenProvider.PrivacyNoticeVersion = privacyNoticeVersion2;
			}
			return acceleratedTokenProvider;
		}*/

		/*private SecurityTokenProvider CreateServerX509TokenProvider(EndpointAddress targetAddress)
		{
			X509Certificate2 x509Certificate = null;
			if (targetAddress != null)
			{
				this.parent.ServiceCertificate.ScopedCertificates.TryGetValue(targetAddress.Uri, out x509Certificate);
			}
			if (x509Certificate == null)
			{
				x509Certificate = this.parent.ServiceCertificate.DefaultCertificate;
			}
			if (x509Certificate == null && targetAddress.Identity != null && targetAddress.Identity.GetType() == typeof(X509CertificateEndpointIdentity))
			{
				x509Certificate = ((X509CertificateEndpointIdentity)targetAddress.Identity).Certificates[0];
			}
			if (x509Certificate != null)
			{
				return new X509SecurityTokenProvider(x509Certificate);
			}
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.GetString("ServiceCertificateNotProvidedOnClientCredentials", new object[]
			{
				targetAddress.Uri
			})));
		}*/

		/*private X509SecurityTokenAuthenticator CreateServerX509TokenAuthenticator()
		{
			return new X509SecurityTokenAuthenticator(this.parent.ServiceCertificate.Authentication.GetCertificateValidator(), false);
		}

		private X509SecurityTokenAuthenticator CreateServerSslX509TokenAuthenticator()
		{
			if (this.parent.ServiceCertificate.SslCertificateAuthentication != null)
			{
				return new X509SecurityTokenAuthenticator(this.parent.ServiceCertificate.SslCertificateAuthentication.GetCertificateValidator(), false);
			}
			return this.CreateServerX509TokenAuthenticator();
		}*/

		private bool IsDigestAuthenticationScheme(SecurityTokenRequirement requirement)
		{
			if (!requirement.Properties.ContainsKey(ServiceModelSecurityTokenRequirement.HttpAuthenticationSchemeProperty))
			{
				return false;
			}
			AuthenticationSchemes authenticationSchemes = (AuthenticationSchemes)requirement.Properties[ServiceModelSecurityTokenRequirement.HttpAuthenticationSchemeProperty];
			if (!authenticationSchemes.IsSingleton())
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(Res.GetString(Res.S("HttpRequiresSingleAuthScheme"), new object[] { authenticationSchemes }));
			}
			return authenticationSchemes == AuthenticationSchemes.Digest;
		}

		/// <summary>Gets a value that indicates whether the specified token requirement is an issued security token requirement.</summary>
		/// <returns>true if the specified token requirement is an issued security token requirement; otherwise, false. The default is false.</returns>
		/// <param name="requirement">The <see cref="T:System.IdentityModel.Selectors.SecurityTokenRequirement" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="requirement" /> is null.</exception>
		/*protected internal bool IsIssuedSecurityTokenRequirement(SecurityTokenRequirement requirement)
		{
			return requirement != null && requirement.Properties.ContainsKey(ServiceModelSecurityTokenRequirement.IssuerAddressProperty) && !(requirement.TokenType == ServiceModelSecurityTokenTypes.AnonymousSslnego) && !(requirement.TokenType == ServiceModelSecurityTokenTypes.MutualSslnego) && !(requirement.TokenType == ServiceModelSecurityTokenTypes.SecureConversation) && !(requirement.TokenType == ServiceModelSecurityTokenTypes.Spnego);
		}

		private void CopyIssuerChannelBehaviorsAndAddSecurityCredentials(IssuedSecurityTokenProvider federationTokenProvider, KeyedByTypeCollection<IEndpointBehavior> issuerChannelBehaviors, EndpointAddress issuerAddress)
		{
			if (issuerChannelBehaviors != null)
			{
				foreach (IEndpointBehavior current in issuerChannelBehaviors)
				{
					if (current is SecurityCredentialsManager)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.GetString("IssuerChannelBehaviorsCannotContainSecurityCredentialsManager", new object[]
						{
							issuerAddress,
							typeof(SecurityCredentialsManager)
						})));
					}
					federationTokenProvider.IssuerChannelBehaviors.Add(current);
				}
			}
			federationTokenProvider.IssuerChannelBehaviors.Add(this.parent);
		}*/

		/*private SecurityKeyEntropyMode GetIssuerBindingKeyEntropyModeOrDefault(Binding issuerBinding)
		{
			BindingElementCollection bindingElementCollection = issuerBinding.CreateBindingElements();
			SecurityBindingElement securityBindingElement = bindingElementCollection.Find<SecurityBindingElement>();
			if (securityBindingElement != null)
			{
				return securityBindingElement.KeyEntropyMode;
			}
			return this.parent.IssuedToken.DefaultKeyEntropyMode;
		}

		private void GetIssuerBindingSecurityVersion(Binding issuerBinding, MessageSecurityVersion issuedTokenParametersDefaultMessageSecurityVersion, SecurityBindingElement outerSecurityBindingElement, out MessageSecurityVersion messageSecurityVersion, out SecurityTokenSerializer tokenSerializer)
		{
			messageSecurityVersion = null;
			if (issuerBinding != null)
			{
				BindingElementCollection bindingElementCollection = issuerBinding.CreateBindingElements();
				SecurityBindingElement securityBindingElement = bindingElementCollection.Find<SecurityBindingElement>();
				if (securityBindingElement != null)
				{
					messageSecurityVersion = securityBindingElement.MessageSecurityVersion;
				}
			}
			if (messageSecurityVersion == null)
			{
				if (issuedTokenParametersDefaultMessageSecurityVersion != null)
				{
					messageSecurityVersion = issuedTokenParametersDefaultMessageSecurityVersion;
				}
				else if (outerSecurityBindingElement != null)
				{
					messageSecurityVersion = outerSecurityBindingElement.MessageSecurityVersion;
				}
			}
			if (messageSecurityVersion == null)
			{
				messageSecurityVersion = MessageSecurityVersion.Default;
			}
			tokenSerializer = this.CreateSecurityTokenSerializer(messageSecurityVersion.SecurityTokenVersion);
		}*/

		/*private IssuedSecurityTokenProvider CreateIssuedSecurityTokenProvider(InitiatorServiceModelSecurityTokenRequirement initiatorRequirement, FederatedClientCredentialsParameters actAsOnBehalfOfParameters)
		{
			if (initiatorRequirement.TargetAddress == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.GetString("TokenRequirementDoesNotSpecifyTargetAddress", new object[]
				{
					initiatorRequirement
				}));
			}
			SecurityBindingElement securityBindingElement = initiatorRequirement.SecurityBindingElement;
			if (securityBindingElement == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.GetString("TokenProviderRequiresSecurityBindingElement", new object[]
				{
					initiatorRequirement
				}));
			}
			EndpointAddress endpointAddress = initiatorRequirement.IssuerAddress;
			Binding binding = initiatorRequirement.IssuerBinding;
			bool flag = endpointAddress == null || endpointAddress.Equals(EndpointAddress.AnonymousAddress);
			if (flag)
			{
				endpointAddress = this.parent.IssuedToken.LocalIssuerAddress;
				binding = this.parent.IssuedToken.LocalIssuerBinding;
			}
			if (endpointAddress == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.GetString("StsAddressNotSet", new object[]
				{
					initiatorRequirement.TargetAddress
				})));
			}
			if (binding == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.GetString("StsBindingNotSet", new object[]
				{
					endpointAddress
				})));
			}
			Uri uri = endpointAddress.Uri;
			KeyedByTypeCollection<IEndpointBehavior> localIssuerChannelBehaviors;
			if (!this.parent.IssuedToken.IssuerChannelBehaviors.TryGetValue(endpointAddress.Uri, out localIssuerChannelBehaviors) & flag)
			{
				localIssuerChannelBehaviors = this.parent.IssuedToken.LocalIssuerChannelBehaviors;
			}
			IssuedSecurityTokenProvider issuedSecurityTokenProvider = new IssuedSecurityTokenProvider(this.GetCredentialsHandle(initiatorRequirement));
			issuedSecurityTokenProvider.TokenHandlerCollectionManager = this.parent.SecurityTokenHandlerCollectionManager;
			issuedSecurityTokenProvider.TargetAddress = initiatorRequirement.TargetAddress;
			this.CopyIssuerChannelBehaviorsAndAddSecurityCredentials(issuedSecurityTokenProvider, localIssuerChannelBehaviors, endpointAddress);
			issuedSecurityTokenProvider.CacheIssuedTokens = this.parent.IssuedToken.CacheIssuedTokens;
			issuedSecurityTokenProvider.IdentityVerifier = securityBindingElement.LocalClientSettings.IdentityVerifier;
			issuedSecurityTokenProvider.IssuerAddress = endpointAddress;
			issuedSecurityTokenProvider.IssuerBinding = binding;
			issuedSecurityTokenProvider.KeyEntropyMode = this.GetIssuerBindingKeyEntropyModeOrDefault(binding);
			issuedSecurityTokenProvider.MaxIssuedTokenCachingTime = this.parent.IssuedToken.MaxIssuedTokenCachingTime;
			issuedSecurityTokenProvider.SecurityAlgorithmSuite = initiatorRequirement.SecurityAlgorithmSuite;
			IssuedSecurityTokenParameters property = initiatorRequirement.GetProperty<IssuedSecurityTokenParameters>(ServiceModelSecurityTokenRequirement.IssuedSecurityTokenParametersProperty);
			MessageSecurityVersion messageSecurityVersion;
			SecurityTokenSerializer securityTokenSerializer;
			this.GetIssuerBindingSecurityVersion(binding, property.DefaultMessageSecurityVersion, initiatorRequirement.SecurityBindingElement, out messageSecurityVersion, out securityTokenSerializer);
			issuedSecurityTokenProvider.MessageSecurityVersion = messageSecurityVersion;
			issuedSecurityTokenProvider.SecurityTokenSerializer = securityTokenSerializer;
			issuedSecurityTokenProvider.IssuedTokenRenewalThresholdPercentage = this.parent.IssuedToken.IssuedTokenRenewalThresholdPercentage;
			IEnumerable<XmlElement> enumerable = property.CreateRequestParameters(messageSecurityVersion, securityTokenSerializer);
			if (enumerable != null)
			{
				foreach (XmlElement current in enumerable)
				{
					issuedSecurityTokenProvider.TokenRequestParameters.Add(current);
				}
			}
			ChannelParameterCollection channelParameters;
			if (initiatorRequirement.TryGetProperty<ChannelParameterCollection>(ServiceModelSecurityTokenRequirement.ChannelParametersCollectionProperty, out channelParameters))
			{
				issuedSecurityTokenProvider.ChannelParameters = channelParameters;
			}
			issuedSecurityTokenProvider.SetupActAsOnBehalfOfParameters(actAsOnBehalfOfParameters);
			return issuedSecurityTokenProvider;
		}*/

		/// <summary>Creates a security token provider.</summary>
		/// <returns>The <see cref="T:System.IdentityModel.Selectors.SecurityTokenProvider" />.</returns>
		/// <param name="tokenRequirement">The <see cref="T:System.IdentityModel.Selectors.SecurityTokenRequirement" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="tokenRequirement" /> is null.</exception>
		public override SecurityTokenProvider CreateSecurityTokenProvider(SecurityTokenRequirement tokenRequirement)
		{
			return this.CreateSecurityTokenProvider(tokenRequirement, false);
		}

		internal SecurityTokenProvider CreateSecurityTokenProvider(SecurityTokenRequirement tokenRequirement, bool disableInfoCard)
		{
			if (tokenRequirement == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokenRequirement");
			}
			SecurityTokenProvider securityTokenProvider = null;
			//if (disableInfoCard || !this.CardSpaceTryCreateSecurityTokenProviderStub(tokenRequirement, this, out securityTokenProvider))
			{
				/*if (tokenRequirement is RecipientServiceModelSecurityTokenRequirement && tokenRequirement.TokenType == SecurityTokenTypes.X509Certificate && tokenRequirement.KeyUsage == SecurityKeyUsage.Exchange)
				{
					if (this.parent.ClientCertificate.Certificate == null)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.GetString("ClientCertificateNotProvidedOnClientCredentials")));
					}
					securityTokenProvider = new X509SecurityTokenProvider(this.parent.ClientCertificate.Certificate);
				}
				else*/ if (tokenRequirement is InitiatorServiceModelSecurityTokenRequirement)
				{
					InitiatorServiceModelSecurityTokenRequirement initiatorServiceModelSecurityTokenRequirement = tokenRequirement as InitiatorServiceModelSecurityTokenRequirement;
					string tokenType = initiatorServiceModelSecurityTokenRequirement.TokenType;
					/*if (this.IsIssuedSecurityTokenRequirement(initiatorServiceModelSecurityTokenRequirement))
					{
						FederatedClientCredentialsParameters federatedClientCredentialsParameters = this.FindFederatedChannelParameters(tokenRequirement);
						if (federatedClientCredentialsParameters != null && federatedClientCredentialsParameters.IssuedSecurityToken != null)
						{
							return new SimpleSecurityTokenProvider(federatedClientCredentialsParameters.IssuedSecurityToken, tokenRequirement);
						}
						securityTokenProvider = this.CreateIssuedSecurityTokenProvider(initiatorServiceModelSecurityTokenRequirement, federatedClientCredentialsParameters);
					}
					else if (tokenType == SecurityTokenTypes.X509Certificate)
					{
						if (initiatorServiceModelSecurityTokenRequirement.Properties.ContainsKey(SecurityTokenRequirement.KeyUsageProperty) && initiatorServiceModelSecurityTokenRequirement.KeyUsage == SecurityKeyUsage.Exchange)
						{
							securityTokenProvider = this.CreateServerX509TokenProvider(initiatorServiceModelSecurityTokenRequirement.TargetAddress);
						}
						else
						{
							if (this.parent.ClientCertificate.Certificate == null)
							{
								throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.GetString("ClientCertificateNotProvidedOnClientCredentials")));
							}
							securityTokenProvider = new X509SecurityTokenProvider(this.parent.ClientCertificate.Certificate);
						}
					}
					else if (tokenType == SecurityTokenTypes.Kerberos)
					{
						string servicePrincipalName = this.GetServicePrincipalName(initiatorServiceModelSecurityTokenRequirement);
						securityTokenProvider = new ClientCredentialsSecurityTokenManager.KerberosSecurityTokenProviderWrapper(new KerberosSecurityTokenProvider(servicePrincipalName, this.parent.Windows.AllowedImpersonationLevel, SecurityUtils.GetNetworkCredentialOrDefault(this.parent.Windows.ClientCredential)), this.GetCredentialsHandle(initiatorServiceModelSecurityTokenRequirement));
					}
					else*/ if (tokenType == SecurityTokenTypes.UserName)
					{
						if (this.parent.UserName.UserName == null)
						{
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(Res.GetString(Res.S("UserNamePasswordNotProvidedOnClientCredentials"))));
						}
						securityTokenProvider = new UserNameSecurityTokenProvider(this.parent.UserName.UserName, this.parent.UserName.Password);
					}
					/*else if (tokenType == ServiceModelSecurityTokenTypes.SspiCredential)
					{
						if (this.IsDigestAuthenticationScheme(initiatorServiceModelSecurityTokenRequirement))
						{
							securityTokenProvider = new SspiSecurityTokenProvider(SecurityUtils.GetNetworkCredentialOrDefault(this.parent.HttpDigest.ClientCredential), true, this.parent.HttpDigest.AllowedImpersonationLevel);
						}
						else
						{
							securityTokenProvider = new SspiSecurityTokenProvider(SecurityUtils.GetNetworkCredentialOrDefault(this.parent.Windows.ClientCredential), this.parent.Windows.AllowNtlm, this.parent.Windows.AllowedImpersonationLevel);
						}
					}
					else if (tokenType == ServiceModelSecurityTokenTypes.Spnego)
					{
						securityTokenProvider = this.CreateSpnegoTokenProvider(initiatorServiceModelSecurityTokenRequirement);
					}
					else if (tokenType == ServiceModelSecurityTokenTypes.MutualSslnego)
					{
						securityTokenProvider = this.CreateTlsnegoTokenProvider(initiatorServiceModelSecurityTokenRequirement, true);
					}
					else if (tokenType == ServiceModelSecurityTokenTypes.AnonymousSslnego)
					{
						securityTokenProvider = this.CreateTlsnegoTokenProvider(initiatorServiceModelSecurityTokenRequirement, false);
					}
					else if (tokenType == ServiceModelSecurityTokenTypes.SecureConversation)
					{
						securityTokenProvider = this.CreateSecureConversationSecurityTokenProvider(initiatorServiceModelSecurityTokenRequirement);
					}*/
				}
			}
			if (securityTokenProvider == null && !tokenRequirement.IsOptionalToken)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(Res.GetString(Res.S("SecurityTokenManagerCannotCreateProviderForRequirement"), new object[]	{ tokenRequirement })));
			}
			return securityTokenProvider;
		}

		/*private bool CardSpaceTryCreateSecurityTokenProviderStub(SecurityTokenRequirement tokenRequirement, ClientCredentialsSecurityTokenManager clientCredentialsTokenManager, out SecurityTokenProvider provider)
		{
			return InfoCardHelper.TryCreateSecurityTokenProvider(tokenRequirement, clientCredentialsTokenManager, out provider);
		}*/

		/// <summary>Creates a security token serializer.</summary>
		/// <returns>The <see cref="T:System.IdentityModel.Selectors.SecurityTokenSerializer" />.</returns>
		/// <param name="version">The <see cref="T:System.ServiceModel.Security.SecurityVersion" /> of the security token.</param>
		/*protected SecurityTokenSerializer CreateSecurityTokenSerializer(SecurityVersion version)
		{
			if (version == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("version"));
			}
			return this.CreateSecurityTokenSerializer(MessageSecurityTokenVersion.GetSecurityTokenVersion(version, true));
		}*/

		/// <summary>Creates a security token serializer.</summary>
		/// <returns>The <see cref="T:System.IdentityModel.Selectors.SecurityTokenSerializer" />.</returns>
		/// <param name="version">The <see cref="T:System.IdentityModel.Selectors.SecurityTokenVersion" /> of the security token.</param>
		/*public override SecurityTokenSerializer CreateSecurityTokenSerializer(SecurityTokenVersion version)
		{
			if (version == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("version");
			}
			if (this.parent != null && this.parent.UseIdentityConfiguration)
			{
				return this.WrapTokenHandlersAsSecurityTokenSerializer(version);
			}
			MessageSecurityTokenVersion messageSecurityTokenVersion = version as MessageSecurityTokenVersion;
			if (messageSecurityTokenVersion != null)
			{
				return new WSSecurityTokenSerializer(messageSecurityTokenVersion.SecurityVersion, messageSecurityTokenVersion.TrustVersion, messageSecurityTokenVersion.SecureConversationVersion, messageSecurityTokenVersion.EmitBspRequiredAttributes, null, null, null);
			}
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.GetString("SecurityTokenManagerCannotCreateSerializerForVersion", new object[]
			{
				version
			})));
		}*/

		/*private SecurityTokenSerializer WrapTokenHandlersAsSecurityTokenSerializer(SecurityTokenVersion version)
		{
			TrustVersion trustVersion = TrustVersion.WSTrust13;
			SecureConversationVersion secureConversationVersion = SecureConversationVersion.WSSecureConversation13;
			SecurityVersion securityVersion = SecurityVersion.WSSecurity11;
			foreach (string current in version.GetSecuritySpecifications())
			{
				if (StringComparer.Ordinal.Equals(current, "http://schemas.xmlsoap.org/ws/2005/02/trust"))
				{
					trustVersion = TrustVersion.WSTrustFeb2005;
				}
				else if (StringComparer.Ordinal.Equals(current, "http://docs.oasis-open.org/ws-sx/ws-trust/200512"))
				{
					trustVersion = TrustVersion.WSTrust13;
				}
				else if (StringComparer.Ordinal.Equals(current, "http://schemas.xmlsoap.org/ws/2005/02/sc"))
				{
					secureConversationVersion = SecureConversationVersion.WSSecureConversationFeb2005;
				}
				else if (StringComparer.Ordinal.Equals(current, "http://docs.oasis-open.org/ws-sx/ws-secureconversation/200512"))
				{
					secureConversationVersion = SecureConversationVersion.WSSecureConversation13;
				}
			}
			securityVersion = FederatedSecurityTokenManager.GetSecurityVersion(version);
			SecurityTokenHandlerCollectionManager securityTokenHandlerCollectionManager = this.parent.SecurityTokenHandlerCollectionManager;
			return new WsSecurityTokenSerializerAdapter(securityTokenHandlerCollectionManager[""], securityVersion, trustVersion, secureConversationVersion, false, null, null, null);
		}*/

		/// <summary>Creates a security token authenticator.</summary>
		/// <returns>The <see cref="T:System.IdentityModel.Selectors.SecurityTokenAuthenticator" />.</returns>
		/// <param name="tokenRequirement">The <see cref="T:System.IdentityModel.Selectors.SecurityTokenRequirement" />.</param>
		/// <param name="outOfBandTokenResolver">When this method returns, contains a <see cref="T:System.IdentityModel.Selectors.SecurityTokenResolver" />. This parameter is passed uninitialized. </param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="tokenRequirement" /> is null.</exception>
		/*public override SecurityTokenAuthenticator CreateSecurityTokenAuthenticator(SecurityTokenRequirement tokenRequirement, out SecurityTokenResolver outOfBandTokenResolver)
		{
			if (tokenRequirement == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokenRequirement");
			}
			outOfBandTokenResolver = null;
			SecurityTokenAuthenticator securityTokenAuthenticator = null;
			InitiatorServiceModelSecurityTokenRequirement initiatorServiceModelSecurityTokenRequirement = tokenRequirement as InitiatorServiceModelSecurityTokenRequirement;
			if (initiatorServiceModelSecurityTokenRequirement != null)
			{
				string tokenType = initiatorServiceModelSecurityTokenRequirement.TokenType;
				if (this.IsIssuedSecurityTokenRequirement(initiatorServiceModelSecurityTokenRequirement))
				{
					return new GenericXmlSecurityTokenAuthenticator();
				}
				if (tokenType == SecurityTokenTypes.X509Certificate)
				{
					if (initiatorServiceModelSecurityTokenRequirement.IsOutOfBandToken)
					{
						securityTokenAuthenticator = new X509SecurityTokenAuthenticator(X509CertificateValidator.None);
					}
					else if (initiatorServiceModelSecurityTokenRequirement.PreferSslCertificateAuthenticator)
					{
						securityTokenAuthenticator = this.CreateServerSslX509TokenAuthenticator();
					}
					else
					{
						securityTokenAuthenticator = this.CreateServerX509TokenAuthenticator();
					}
				}
				else if (tokenType == SecurityTokenTypes.Rsa)
				{
					securityTokenAuthenticator = new RsaSecurityTokenAuthenticator();
				}
				else if (tokenType == SecurityTokenTypes.Kerberos)
				{
					securityTokenAuthenticator = new KerberosRequestorSecurityTokenAuthenticator();
				}
				else if (tokenType == ServiceModelSecurityTokenTypes.SecureConversation || tokenType == ServiceModelSecurityTokenTypes.MutualSslnego || tokenType == ServiceModelSecurityTokenTypes.AnonymousSslnego || tokenType == ServiceModelSecurityTokenTypes.Spnego)
				{
					securityTokenAuthenticator = new GenericXmlSecurityTokenAuthenticator();
				}
			}
			else if (tokenRequirement is RecipientServiceModelSecurityTokenRequirement && tokenRequirement.TokenType == SecurityTokenTypes.X509Certificate)
			{
				securityTokenAuthenticator = this.CreateServerX509TokenAuthenticator();
			}
			if (securityTokenAuthenticator == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.GetString("SecurityTokenManagerCannotCreateAuthenticatorForRequirement", new object[]
				{
					tokenRequirement
				})));
			}
			return securityTokenAuthenticator;
		}*/

		/*private SafeFreeCredentials GetCredentialsHandle(InitiatorServiceModelSecurityTokenRequirement initiatorRequirement)
		{
			SspiIssuanceChannelParameter sspiIssuanceChannelParameter = this.GetSspiIssuanceChannelParameter(initiatorRequirement);
			if (sspiIssuanceChannelParameter == null)
			{
				return null;
			}
			return sspiIssuanceChannelParameter.CredentialsHandle;
		}*/

		/*internal FederatedClientCredentialsParameters FindFederatedChannelParameters(SecurityTokenRequirement tokenRequirement)
		{
			FederatedClientCredentialsParameters federatedClientCredentialsParameters = null;
			ChannelParameterCollection channelParameterCollection = null;
			if (tokenRequirement.TryGetProperty<ChannelParameterCollection>(ServiceModelSecurityTokenRequirement.ChannelParametersCollectionProperty, out channelParameterCollection) && channelParameterCollection != null)
			{
				foreach (object current in channelParameterCollection)
				{
					federatedClientCredentialsParameters = (current as FederatedClientCredentialsParameters);
					if (federatedClientCredentialsParameters != null)
					{
						break;
					}
				}
			}
			return federatedClientCredentialsParameters;
		}*/
	}
}
