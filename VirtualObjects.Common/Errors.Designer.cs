﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18051
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VirtualObjects {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Errors {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Errors() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("VirtualObjects.Errors", typeof(Errors).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Init method on Mapper not called..
        /// </summary>
        public static string Internal_MapperNotInitialized {
            get {
                return ResourceManager.GetString("Internal_MapperNotInitialized", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Wrong method called for {NodeType}..
        /// </summary>
        public static string Internal_WrongMethodCall {
            get {
                return ResourceManager.GetString("Internal_WrongMethodCall", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Provider for type {Name} not yet implemented..
        /// </summary>
        public static string Mapping_EntityTypeNotSupported {
            get {
                return ResourceManager.GetString("Mapping_EntityTypeNotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type {OutputType} is not yet supported..
        /// </summary>
        public static string Mapping_OutputTypeNotSupported {
            get {
                return ResourceManager.GetString("Mapping_OutputTypeNotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to get the ForeignKey for field {Name} in type {PropertyType.Name}..
        /// </summary>
        public static string Mapping_UnableToGetForeignKey {
            get {
                return ResourceManager.GetString("Mapping_UnableToGetForeignKey", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Its not possible to get the value of field [{FieldName}]..
        /// </summary>
        public static string Mapping_UnableToGetValue {
            get {
                return ResourceManager.GetString("Mapping_UnableToGetValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Its not possible to set the field [{FieldName}] with the value [{Value}].
        /// </summary>
        public static string Mapping_UnableToSetValue {
            get {
                return ResourceManager.GetString("Mapping_UnableToSetValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to find the proper Entity Info for {ElementTypeName}..
        /// </summary>
        public static string Query_EntityInfoNotFound {
            get {
                return ResourceManager.GetString("Query_EntityInfoNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The source of the query was not set..
        /// </summary>
        public static string Query_SourceNotSet {
            get {
                return ResourceManager.GetString("Query_SourceNotSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {NodeType} in predicate is not yet supported..
        /// </summary>
        public static string SQL_ExpressionTypeNotSupported {
            get {
                return ResourceManager.GetString("SQL_ExpressionTypeNotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {NodeType} is not yet supported..
        /// </summary>
        public static string SQL_UnableToFormatNode {
            get {
                return ResourceManager.GetString("SQL_UnableToFormatNode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The datetime member [{Name}] is not yet supported..
        /// </summary>
        public static string Translation_Datetime_Member_NotSupported {
            get {
                return ResourceManager.GetString("Translation_Datetime_Member_NotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The member {Member.Name} is not yet supported..
        /// </summary>
        public static string Translation_DateTimeMemberNotSupported {
            get {
                return ResourceManager.GetString("Translation_DateTimeMemberNotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Predicates with multiple member access on both sides is not yet supported..
        /// </summary>
        public static string Translation_ManyMembersAccess_On_BothSides_NotSupported {
            get {
                return ResourceManager.GetString("Translation_ManyMembersAccess_On_BothSides_NotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The method {Name} without arguments is not supported..
        /// </summary>
        public static string Translation_Method_NoArgs_NotSupported {
            get {
                return ResourceManager.GetString("Translation_Method_NoArgs_NotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [{MethodName}] Method translation not yet supported..
        /// </summary>
        public static string Translation_MethodCall_NotSupported {
            get {
                return ResourceManager.GetString("Translation_MethodCall_NotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The method [{Method.Name}] used in a query is not yet supported..
        /// </summary>
        public static string Translation_MethodNotSupported {
            get {
                return ResourceManager.GetString("Translation_MethodNotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The string member [{Name}] is not yet supported..
        /// </summary>
        public static string Translation_String_MemberAccess_NotSupported {
            get {
                return ResourceManager.GetString("Translation_String_MemberAccess_NotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to extract lambda from expression..
        /// </summary>
        public static string Translation_UnableToExtractLambda {
            get {
                return ResourceManager.GetString("Translation_UnableToExtractLambda", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to extract the queryable from expression..
        /// </summary>
        public static string Translation_UnableToExtractQueryable {
            get {
                return ResourceManager.GetString("Translation_UnableToExtractQueryable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to get the type from {NodeType} because it&apos;s not yet supported..
        /// </summary>
        public static string UnableToGetType {
            get {
                return ResourceManager.GetString("UnableToGetType", resourceCulture);
            }
        }
    }
}
