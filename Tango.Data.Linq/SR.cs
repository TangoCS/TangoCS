﻿using System;
using System.Globalization;
using System.Resources;
using System.Threading;
namespace System.Data.Linq
{
	internal sealed class SR
	{
		internal const string OwningTeam = "OwningTeam";
		internal const string CannotAddChangeConflicts = "CannotAddChangeConflicts";
		internal const string CannotRemoveChangeConflicts = "CannotRemoveChangeConflicts";
		internal const string InconsistentAssociationAndKeyChange = "InconsistentAssociationAndKeyChange";
		internal const string UnableToDetermineDataContext = "UnableToDetermineDataContext";
		internal const string ArgumentTypeHasNoIdentityKey = "ArgumentTypeHasNoIdentityKey";
		internal const string CouldNotConvert = "CouldNotConvert";
		internal const string CannotRemoveUnattachedEntity = "CannotRemoveUnattachedEntity";
		internal const string ColumnMappedMoreThanOnce = "ColumnMappedMoreThanOnce";
		internal const string CouldNotAttach = "CouldNotAttach";
		internal const string CouldNotGetTableForSubtype = "CouldNotGetTableForSubtype";
		internal const string CouldNotRemoveRelationshipBecauseOneSideCannotBeNull = "CouldNotRemoveRelationshipBecauseOneSideCannotBeNull";
		internal const string EntitySetAlreadyLoaded = "EntitySetAlreadyLoaded";
		internal const string EntitySetModifiedDuringEnumeration = "EntitySetModifiedDuringEnumeration";
		internal const string ExpectedQueryableArgument = "ExpectedQueryableArgument";
		internal const string ExpectedUpdateDeleteOrChange = "ExpectedUpdateDeleteOrChange";
		internal const string KeyIsWrongSize = "KeyIsWrongSize";
		internal const string KeyValueIsWrongType = "KeyValueIsWrongType";
		internal const string IdentityChangeNotAllowed = "IdentityChangeNotAllowed";
		internal const string DbGeneratedChangeNotAllowed = "DbGeneratedChangeNotAllowed";
		internal const string ModifyDuringAddOrRemove = "ModifyDuringAddOrRemove";
		internal const string ProviderDoesNotImplementRequiredInterface = "ProviderDoesNotImplementRequiredInterface";
		internal const string ProviderTypeNull = "ProviderTypeNull";
		internal const string TypeCouldNotBeAdded = "TypeCouldNotBeAdded";
		internal const string TypeCouldNotBeRemoved = "TypeCouldNotBeRemoved";
		internal const string TypeCouldNotBeTracked = "TypeCouldNotBeTracked";
		internal const string TypeIsNotEntity = "TypeIsNotEntity";
		internal const string UnrecognizedRefreshObject = "UnrecognizedRefreshObject";
		internal const string UnhandledExpressionType = "UnhandledExpressionType";
		internal const string UnhandledBindingType = "UnhandledBindingType";
		internal const string ObjectTrackingRequired = "ObjectTrackingRequired";
		internal const string OptionsCannotBeModifiedAfterQuery = "OptionsCannotBeModifiedAfterQuery";
		internal const string DeferredLoadingRequiresObjectTracking = "DeferredLoadingRequiresObjectTracking";
		internal const string SubqueryDoesNotSupportOperator = "SubqueryDoesNotSupportOperator";
		internal const string SubqueryNotSupportedOn = "SubqueryNotSupportedOn";
		internal const string SubqueryNotSupportedOnType = "SubqueryNotSupportedOnType";
		internal const string SubqueryNotAllowedAfterFreeze = "SubqueryNotAllowedAfterFreeze";
		internal const string IncludeNotAllowedAfterFreeze = "IncludeNotAllowedAfterFreeze";
		internal const string LoadOptionsChangeNotAllowedAfterQuery = "LoadOptionsChangeNotAllowedAfterQuery";
		internal const string IncludeCycleNotAllowed = "IncludeCycleNotAllowed";
		internal const string SubqueryMustBeSequence = "SubqueryMustBeSequence";
		internal const string RefreshOfDeletedObject = "RefreshOfDeletedObject";
		internal const string RefreshOfNewObject = "RefreshOfNewObject";
		internal const string CannotChangeInheritanceType = "CannotChangeInheritanceType";
		internal const string DataContextCannotBeUsedAfterDispose = "DataContextCannotBeUsedAfterDispose";
		internal const string TypeIsNotMarkedAsTable = "TypeIsNotMarkedAsTable";
		internal const string NonEntityAssociationMapping = "NonEntityAssociationMapping";
		internal const string CannotPerformCUDOnReadOnlyTable = "CannotPerformCUDOnReadOnlyTable";
		internal const string InsertCallbackComment = "InsertCallbackComment";
		internal const string UpdateCallbackComment = "UpdateCallbackComment";
		internal const string DeleteCallbackComment = "DeleteCallbackComment";
		internal const string RowNotFoundOrChanged = "RowNotFoundOrChanged";
		internal const string UpdatesFailedMessage = "UpdatesFailedMessage";
		internal const string CycleDetected = "CycleDetected";
		internal const string CantAddAlreadyExistingItem = "CantAddAlreadyExistingItem";
		internal const string CantAddAlreadyExistingKey = "CantAddAlreadyExistingKey";
		internal const string DatabaseGeneratedAlreadyExistingKey = "DatabaseGeneratedAlreadyExistingKey";
		internal const string InsertAutoSyncFailure = "InsertAutoSyncFailure";
		internal const string EntitySetDataBindingWithAbstractBaseClass = "EntitySetDataBindingWithAbstractBaseClass";
		internal const string EntitySetDataBindingWithNonPublicDefaultConstructor = "EntitySetDataBindingWithNonPublicDefaultConstructor";
		internal const string InvalidLoadOptionsLoadMemberSpecification = "InvalidLoadOptionsLoadMemberSpecification";
		internal const string EntityIsTheWrongType = "EntityIsTheWrongType";
		internal const string OriginalEntityIsWrongType = "OriginalEntityIsWrongType";
		internal const string CannotAttachAlreadyExistingEntity = "CannotAttachAlreadyExistingEntity";
		internal const string CannotAttachAsModifiedWithoutOriginalState = "CannotAttachAsModifiedWithoutOriginalState";
		internal const string CannotPerformOperationDuringSubmitChanges = "CannotPerformOperationDuringSubmitChanges";
		internal const string CannotPerformOperationOutsideSubmitChanges = "CannotPerformOperationOutsideSubmitChanges";
		internal const string CannotPerformOperationForUntrackedObject = "CannotPerformOperationForUntrackedObject";
		internal const string CannotAttachAddNonNewEntities = "CannotAttachAddNonNewEntities";
		internal const string QueryWasCompiledForDifferentMappingSource = "QueryWasCompiledForDifferentMappingSource";
		internal const string InvalidFieldInfo = "InvalidFieldInfo";
		internal const string CouldNotCreateAccessorToProperty = "CouldNotCreateAccessorToProperty";
		internal const string UnableToAssignValueToReadonlyProperty = "UnableToAssignValueToReadonlyProperty";
		internal const string LinkAlreadyLoaded = "LinkAlreadyLoaded";
		internal const string EntityRefAlreadyLoaded = "EntityRefAlreadyLoaded";
		internal const string NoDiscriminatorFound = "NoDiscriminatorFound";
		internal const string InheritanceTypeDoesNotDeriveFromRoot = "InheritanceTypeDoesNotDeriveFromRoot";
		internal const string AbstractClassAssignInheritanceDiscriminator = "AbstractClassAssignInheritanceDiscriminator";
		internal const string CannotGetInheritanceDefaultFromNonInheritanceClass = "CannotGetInheritanceDefaultFromNonInheritanceClass";
		internal const string InheritanceCodeMayNotBeNull = "InheritanceCodeMayNotBeNull";
		internal const string InheritanceTypeHasMultipleDiscriminators = "InheritanceTypeHasMultipleDiscriminators";
		internal const string InheritanceCodeUsedForMultipleTypes = "InheritanceCodeUsedForMultipleTypes";
		internal const string InheritanceTypeHasMultipleDefaults = "InheritanceTypeHasMultipleDefaults";
		internal const string InheritanceHierarchyDoesNotDefineDefault = "InheritanceHierarchyDoesNotDefineDefault";
		internal const string InheritanceSubTypeIsAlsoRoot = "InheritanceSubTypeIsAlsoRoot";
		internal const string NonInheritanceClassHasDiscriminator = "NonInheritanceClassHasDiscriminator";
		internal const string MemberMappedMoreThanOnce = "MemberMappedMoreThanOnce";
		internal const string BadStorageProperty = "BadStorageProperty";
		internal const string IncorrectAutoSyncSpecification = "IncorrectAutoSyncSpecification";
		internal const string UnhandledDeferredStorageType = "UnhandledDeferredStorageType";
		internal const string BadKeyMember = "BadKeyMember";
		internal const string ProviderTypeNotFound = "ProviderTypeNotFound";
		internal const string MethodCannotBeFound = "MethodCannotBeFound";
		internal const string UnableToResolveRootForType = "UnableToResolveRootForType";
		internal const string MappingForTableUndefined = "MappingForTableUndefined";
		internal const string CouldNotFindTypeFromMapping = "CouldNotFindTypeFromMapping";
		internal const string TwoMembersMarkedAsPrimaryKeyAndDBGenerated = "TwoMembersMarkedAsPrimaryKeyAndDBGenerated";
		internal const string TwoMembersMarkedAsRowVersion = "TwoMembersMarkedAsRowVersion";
		internal const string TwoMembersMarkedAsInheritanceDiscriminator = "TwoMembersMarkedAsInheritanceDiscriminator";
		internal const string CouldNotFindRuntimeTypeForMapping = "CouldNotFindRuntimeTypeForMapping";
		internal const string UnexpectedNull = "UnexpectedNull";
		internal const string CouldNotFindElementTypeInModel = "CouldNotFindElementTypeInModel";
		internal const string BadFunctionTypeInMethodMapping = "BadFunctionTypeInMethodMapping";
		internal const string IncorrectNumberOfParametersMappedForMethod = "IncorrectNumberOfParametersMappedForMethod";
		internal const string CouldNotFindRequiredAttribute = "CouldNotFindRequiredAttribute";
		internal const string InvalidDeleteOnNullSpecification = "InvalidDeleteOnNullSpecification";
		internal const string MappedMemberHadNoCorrespondingMemberInType = "MappedMemberHadNoCorrespondingMemberInType";
		internal const string UnrecognizedAttribute = "UnrecognizedAttribute";
		internal const string UnrecognizedElement = "UnrecognizedElement";
		internal const string TooManyResultTypesDeclaredForFunction = "TooManyResultTypesDeclaredForFunction";
		internal const string NoResultTypesDeclaredForFunction = "NoResultTypesDeclaredForFunction";
		internal const string UnexpectedElement = "UnexpectedElement";
		internal const string ExpectedEmptyElement = "ExpectedEmptyElement";
		internal const string DatabaseNodeNotFound = "DatabaseNodeNotFound";
		internal const string DiscriminatorClrTypeNotSupported = "DiscriminatorClrTypeNotSupported";
		internal const string IdentityClrTypeNotSupported = "IdentityClrTypeNotSupported";
		internal const string PrimaryKeyInSubTypeNotSupported = "PrimaryKeyInSubTypeNotSupported";
		internal const string MismatchedThisKeyOtherKey = "MismatchedThisKeyOtherKey";
		internal const string InvalidUseOfGenericMethodAsMappedFunction = "InvalidUseOfGenericMethodAsMappedFunction";
		internal const string MappingOfInterfacesMemberIsNotSupported = "MappingOfInterfacesMemberIsNotSupported";
		internal const string UnmappedClassMember = "UnmappedClassMember";
		internal const string VbLikeDoesNotSupportMultipleCharacterRanges = "VbLikeDoesNotSupportMultipleCharacterRanges";
		internal const string VbLikeUnclosedBracket = "VbLikeUnclosedBracket";
		internal const string UnrecognizedProviderMode = "UnrecognizedProviderMode";
		internal const string CompiledQueryCannotReturnType = "CompiledQueryCannotReturnType";
		internal const string ArgumentEmpty = "ArgumentEmpty";
		internal const string ProviderCannotBeUsedAfterDispose = "ProviderCannotBeUsedAfterDispose";
		internal const string ArgumentTypeMismatch = "ArgumentTypeMismatch";
		internal const string ContextNotInitialized = "ContextNotInitialized";
		internal const string CouldNotDetermineSqlType = "CouldNotDetermineSqlType";
		internal const string CouldNotDetermineDbGeneratedSqlType = "CouldNotDetermineDbGeneratedSqlType";
		internal const string CouldNotDetermineCatalogName = "CouldNotDetermineCatalogName";
		internal const string CreateDatabaseFailedBecauseOfClassWithNoMembers = "CreateDatabaseFailedBecauseOfClassWithNoMembers";
		internal const string CreateDatabaseFailedBecauseOfContextWithNoTables = "CreateDatabaseFailedBecauseOfContextWithNoTables";
		internal const string CreateDatabaseFailedBecauseSqlCEDatabaseAlreadyExists = "CreateDatabaseFailedBecauseSqlCEDatabaseAlreadyExists";
		internal const string DistributedTransactionsAreNotAllowed = "DistributedTransactionsAreNotAllowed";
		internal const string InvalidConnectionArgument = "InvalidConnectionArgument";
		internal const string CannotEnumerateResultsMoreThanOnce = "CannotEnumerateResultsMoreThanOnce";
		internal const string IifReturnTypesMustBeEqual = "IifReturnTypesMustBeEqual";
		internal const string MethodNotMappedToStoredProcedure = "MethodNotMappedToStoredProcedure";
		internal const string ResultTypeNotMappedToFunction = "ResultTypeNotMappedToFunction";
		internal const string ToStringOnlySupportedForPrimitiveTypes = "ToStringOnlySupportedForPrimitiveTypes";
		internal const string TransactionDoesNotMatchConnection = "TransactionDoesNotMatchConnection";
		internal const string UnexpectedTypeCode = "UnexpectedTypeCode";
		internal const string UnsupportedDateTimeConstructorForm = "UnsupportedDateTimeConstructorForm";
		internal const string UnsupportedDateTimeOffsetConstructorForm = "UnsupportedDateTimeOffsetConstructorForm";
		internal const string UnsupportedStringConstructorForm = "UnsupportedStringConstructorForm";
		internal const string UnsupportedTimeSpanConstructorForm = "UnsupportedTimeSpanConstructorForm";
		internal const string UnsupportedTypeConstructorForm = "UnsupportedTypeConstructorForm";
		internal const string WrongNumberOfValuesInCollectionArgument = "WrongNumberOfValuesInCollectionArgument";
		internal const string LogGeneralInfoMessage = "LogGeneralInfoMessage";
		internal const string LogAttemptingToDeleteDatabase = "LogAttemptingToDeleteDatabase";
		internal const string LogStoredProcedureExecution = "LogStoredProcedureExecution";
		internal const string MemberCannotBeTranslated = "MemberCannotBeTranslated";
		internal const string NonConstantExpressionsNotSupportedFor = "NonConstantExpressionsNotSupportedFor";
		internal const string MathRoundNotSupported = "MathRoundNotSupported";
		internal const string SqlMethodOnlyForSql = "SqlMethodOnlyForSql";
		internal const string NonConstantExpressionsNotSupportedForRounding = "NonConstantExpressionsNotSupportedForRounding";
		internal const string CompiledQueryAgainstMultipleShapesNotSupported = "CompiledQueryAgainstMultipleShapesNotSupported";
		internal const string LenOfTextOrNTextNotSupported = "LenOfTextOrNTextNotSupported";
		internal const string TextNTextAndImageCannotOccurInDistinct = "TextNTextAndImageCannotOccurInDistinct";
		internal const string TextNTextAndImageCannotOccurInUnion = "TextNTextAndImageCannotOccurInUnion";
		internal const string MaxSizeNotSupported = "MaxSizeNotSupported";
		internal const string IndexOfWithStringComparisonArgNotSupported = "IndexOfWithStringComparisonArgNotSupported";
		internal const string LastIndexOfWithStringComparisonArgNotSupported = "LastIndexOfWithStringComparisonArgNotSupported";
		internal const string ConvertToCharFromBoolNotSupported = "ConvertToCharFromBoolNotSupported";
		internal const string ConvertToDateTimeOnlyForDateTimeOrString = "ConvertToDateTimeOnlyForDateTimeOrString";
		internal const string CannotTranslateExpressionToSql = "CannotTranslateExpressionToSql";
		internal const string SkipIsValidOnlyOverOrderedQueries = "SkipIsValidOnlyOverOrderedQueries";
		internal const string SkipRequiresSingleTableQueryWithPKs = "SkipRequiresSingleTableQueryWithPKs";
		internal const string NoMethodInTypeMatchingArguments = "NoMethodInTypeMatchingArguments";
		internal const string CannotConvertToEntityRef = "CannotConvertToEntityRef";
		internal const string ExpressionNotDeferredQuerySource = "ExpressionNotDeferredQuerySource";
		internal const string DeferredMemberWrongType = "DeferredMemberWrongType";
		internal const string ArgumentWrongType = "ArgumentWrongType";
		internal const string ArgumentWrongValue = "ArgumentWrongValue";
		internal const string BadProjectionInSelect = "BadProjectionInSelect";
		internal const string InvalidReturnFromSproc = "InvalidReturnFromSproc";
		internal const string WrongDataContext = "WrongDataContext";
		internal const string BinaryOperatorNotRecognized = "BinaryOperatorNotRecognized";
		internal const string CannotAggregateType = "CannotAggregateType";
		internal const string CannotCompareItemsAssociatedWithDifferentTable = "CannotCompareItemsAssociatedWithDifferentTable";
		internal const string CannotDeleteTypesOf = "CannotDeleteTypesOf";
		internal const string ClassLiteralsNotAllowed = "ClassLiteralsNotAllowed";
		internal const string ClientCaseShouldNotHold = "ClientCaseShouldNotHold";
		internal const string ClrBoolDoesNotAgreeWithSqlType = "ClrBoolDoesNotAgreeWithSqlType";
		internal const string ColumnCannotReferToItself = "ColumnCannotReferToItself";
		internal const string ColumnClrTypeDoesNotAgreeWithExpressionsClrType = "ColumnClrTypeDoesNotAgreeWithExpressionsClrType";
		internal const string ColumnIsDefinedInMultiplePlaces = "ColumnIsDefinedInMultiplePlaces";
		internal const string ColumnIsNotAccessibleThroughGroupBy = "ColumnIsNotAccessibleThroughGroupBy";
		internal const string ColumnIsNotAccessibleThroughDistinct = "ColumnIsNotAccessibleThroughDistinct";
		internal const string ColumnReferencedIsNotInScope = "ColumnReferencedIsNotInScope";
		internal const string ConstructedArraysNotSupported = "ConstructedArraysNotSupported";
		internal const string ParametersCannotBeSequences = "ParametersCannotBeSequences";
		internal const string CapturedValuesCannotBeSequences = "CapturedValuesCannotBeSequences";
		internal const string CouldNotAssignSequence = "CouldNotAssignSequence";
		internal const string CouldNotTranslateExpressionForReading = "CouldNotTranslateExpressionForReading";
		internal const string CouldNotGetClrType = "CouldNotGetClrType";
		internal const string CouldNotGetSqlType = "CouldNotGetSqlType";
		internal const string CouldNotHandleAliasRef = "CouldNotHandleAliasRef";
		internal const string DidNotExpectAs = "DidNotExpectAs";
		internal const string DidNotExpectTypeBinding = "DidNotExpectTypeBinding";
		internal const string DidNotExpectTypeChange = "DidNotExpectTypeChange";
		internal const string EmptyCaseNotSupported = "EmptyCaseNotSupported";
		internal const string ExpectedNoObjectType = "ExpectedNoObjectType";
		internal const string ExpectedBitFoundPredicate = "ExpectedBitFoundPredicate";
		internal const string ExpectedClrTypesToAgree = "ExpectedClrTypesToAgree";
		internal const string ExpectedPredicateFoundBit = "ExpectedPredicateFoundBit";
		internal const string InvalidGroupByExpressionType = "InvalidGroupByExpressionType";
		internal const string InvalidGroupByExpression = "InvalidGroupByExpression";
		internal const string InvalidOrderByExpression = "InvalidOrderByExpression";
		internal const string Impossible = "Impossible";
		internal const string InfiniteDescent = "InfiniteDescent";
		internal const string InvalidFormatNode = "InvalidFormatNode";
		internal const string InvalidReferenceToRemovedAliasDuringDeflation = "InvalidReferenceToRemovedAliasDuringDeflation";
		internal const string InvalidSequenceOperatorCall = "InvalidSequenceOperatorCall";
		internal const string ParameterNotInScope = "ParameterNotInScope";
		internal const string MemberAccessIllegal = "MemberAccessIllegal";
		internal const string MemberCouldNotBeTranslated = "MemberCouldNotBeTranslated";
		internal const string MemberNotPartOfProjection = "MemberNotPartOfProjection";
		internal const string MethodHasNoSupportConversionToSql = "MethodHasNoSupportConversionToSql";
		internal const string MethodFormHasNoSupportConversionToSql = "MethodFormHasNoSupportConversionToSql";
		internal const string UnableToBindUnmappedMember = "UnableToBindUnmappedMember";
		internal const string QueryOperatorNotSupported = "QueryOperatorNotSupported";
		internal const string QueryOperatorOverloadNotSupported = "QueryOperatorOverloadNotSupported";
		internal const string ReaderUsedAfterDispose = "ReaderUsedAfterDispose";
		internal const string RequiredColumnDoesNotExist = "RequiredColumnDoesNotExist";
		internal const string SimpleCaseShouldNotHold = "SimpleCaseShouldNotHold";
		internal const string TypeBinaryOperatorNotRecognized = "TypeBinaryOperatorNotRecognized";
		internal const string UnexpectedNode = "UnexpectedNode";
		internal const string UnexpectedFloatingColumn = "UnexpectedFloatingColumn";
		internal const string UnexpectedSharedExpression = "UnexpectedSharedExpression";
		internal const string UnexpectedSharedExpressionReference = "UnexpectedSharedExpressionReference";
		internal const string UnhandledStringTypeComparison = "UnhandledStringTypeComparison";
		internal const string UnhandledMemberAccess = "UnhandledMemberAccess";
		internal const string UnmappedDataMember = "UnmappedDataMember";
		internal const string UnrecognizedExpressionNode = "UnrecognizedExpressionNode";
		internal const string ValueHasNoLiteralInSql = "ValueHasNoLiteralInSql";
		internal const string UnionIncompatibleConstruction = "UnionIncompatibleConstruction";
		internal const string UnionDifferentMembers = "UnionDifferentMembers";
		internal const string UnionDifferentMemberOrder = "UnionDifferentMemberOrder";
		internal const string UnionOfIncompatibleDynamicTypes = "UnionOfIncompatibleDynamicTypes";
		internal const string UnionWithHierarchy = "UnionWithHierarchy";
		internal const string IntersectNotSupportedForHierarchicalTypes = "IntersectNotSupportedForHierarchicalTypes";
		internal const string ExceptNotSupportedForHierarchicalTypes = "ExceptNotSupportedForHierarchicalTypes";
		internal const string NonCountAggregateFunctionsAreNotValidOnProjections = "NonCountAggregateFunctionsAreNotValidOnProjections";
		internal const string GroupingNotSupportedAsOrderCriterion = "GroupingNotSupportedAsOrderCriterion";
		internal const string SourceExpressionAnnotation = "SourceExpressionAnnotation";
		internal const string SelectManyDoesNotSupportStrings = "SelectManyDoesNotSupportStrings";
		internal const string SequenceOperatorsNotSupportedForType = "SequenceOperatorsNotSupportedForType";
		internal const string SkipNotSupportedForSequenceTypes = "SkipNotSupportedForSequenceTypes";
		internal const string ComparisonNotSupportedForType = "ComparisonNotSupportedForType";
		internal const string QueryOnLocalCollectionNotSupported = "QueryOnLocalCollectionNotSupported";
		internal const string UnsupportedNodeType = "UnsupportedNodeType";
		internal const string TypeColumnWithUnhandledSource = "TypeColumnWithUnhandledSource";
		internal const string GeneralCollectionMaterializationNotSupported = "GeneralCollectionMaterializationNotSupported";
		internal const string TypeCannotBeOrdered = "TypeCannotBeOrdered";
		internal const string InvalidMethodExecution = "InvalidMethodExecution";
		internal const string SprocsCannotBeComposed = "SprocsCannotBeComposed";
		internal const string InsertItemMustBeConstant = "InsertItemMustBeConstant";
		internal const string UpdateItemMustBeConstant = "UpdateItemMustBeConstant";
		internal const string CouldNotConvertToPropertyOrField = "CouldNotConvertToPropertyOrField";
		internal const string BadParameterType = "BadParameterType";
		internal const string CannotAssignToMember = "CannotAssignToMember";
		internal const string MappedTypeMustHaveDefaultConstructor = "MappedTypeMustHaveDefaultConstructor";
		internal const string UnsafeStringConversion = "UnsafeStringConversion";
		internal const string CannotAssignNull = "CannotAssignNull";
		internal const string ProviderNotInstalled = "ProviderNotInstalled";
		internal const string InvalidProviderType = "InvalidProviderType";
		internal const string InvalidDbGeneratedType = "InvalidDbGeneratedType";
		internal const string DatabaseDeleteThroughContext = "DatabaseDeleteThroughContext";
		internal const string CannotMaterializeEntityType = "CannotMaterializeEntityType";

		private static SR loader;
		private ResourceManager resources;
		private static CultureInfo Culture
		{
			get
			{
				return null;
			}
		}
		public static ResourceManager Resources
		{
			get
			{
				return SR.GetLoader().resources;
			}
		}
		internal SR()
		{
			this.resources = new ResourceManager("Tango.Data.Linq.Properties.Main", base.GetType().Assembly);
		}
		private static SR GetLoader()
		{
			if (SR.loader == null)
			{
				SR value = new SR();
				Interlocked.CompareExchange<SR>(ref SR.loader, value, null);
			}
			return SR.loader;
		}
		public static string GetString(string name, params object[] args)
		{
			SR sR = SR.GetLoader();
			if (sR == null)
			{
				return null;
			}
			string @string = sR.resources.GetString(name, SR.Culture);
			if (args != null && args.Length > 0)
			{
				for (int i = 0; i < args.Length; i++)
				{
					string text = args[i] as string;
					if (text != null && text.Length > 1024)
					{
						args[i] = text.Substring(0, 1021) + "...";
					}
				}
				return string.Format(CultureInfo.CurrentCulture, @string, args);
			}
			return @string;
		}
		public static string GetString(string name)
		{
			SR sR = SR.GetLoader();
			if (sR == null)
			{
				return null;
			}
			return sR.resources.GetString(name, SR.Culture);
		}
		public static object GetObject(string name)
		{
			SR sR = SR.GetLoader();
			if (sR == null)
			{
				return null;
			}
			return sR.resources.GetObject(name, SR.Culture);
		}
	}
}