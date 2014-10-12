﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Buzzer.DatabaseConverter.Properties {
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
    internal class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Buzzer.DatabaseConverter.Properties.Resource", typeof(Resource).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to alter table Credits add column ApplicationDate date null;
        ///alter table Credits add column ProtocolDate date null;.
        /// </summary>
        internal static string AddApplicationDateAndProtocolDateColumns {
            get {
                return ResourceManager.GetString("AddApplicationDateAndProtocolDateColumns", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to alter table Credits add column CreditState integer not null default 1;.
        /// </summary>
        internal static string AddCreditStateColumnToCreditsTable {
            get {
                return ResourceManager.GetString("AddCreditStateColumnToCreditsTable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to create table NotificationLog
        ///(
        ///	ID integer primary key autoincrement not null,
        ///	CreditID integer not null,
        ///	PersonID integer not null,
        ///	NotificationDate date not null,
        ///	Comment nvarchar(2000),
        ///	foreign key(CreditID) references Credits(ID),
        ///	foreign key(PersonID) references Persons(ID)
        ///);
        ///
        ///create view NotificationLogView as
        ///select
        ///	NL.ID as ID,
        ///	C.CreditNumber as CreditNumber,
        ///	P.Name as PersonName,
        ///	NL.CreditID as CreditID,
        ///	NL.PersonID as PersonID,
        ///	NL.NotificationDate as NotificationDate [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string AddNotificationLogTable {
            get {
                return ResourceManager.GetString("AddNotificationLogTable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to alter table Credits add column RefusalReason text null;.
        /// </summary>
        internal static string AddRefusalReasonColumnToCreditsTable {
            get {
                return ResourceManager.GetString("AddRefusalReasonColumnToCreditsTable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to alter table Credits add column RowState integer not null default 0;.
        /// </summary>
        internal static string AddRowStateColumnToCreditsTable {
            get {
                return ResourceManager.GetString("AddRowStateColumnToCreditsTable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to create table Users
        ///(
        ///	ID integer primary key autoincrement not null,
        ///	Login nvarchar(100) not null,
        ///	Password nvarchar(100) not null
        ///);
        ///
        ///insert into Users (Login, Password) values (&apos;Atai&apos;, &apos;WZRHGrsBESr8wYFZ9sx0tPURuZgG2lmzyvWpwXPKz8U=&apos;);.
        /// </summary>
        internal static string AddUsersTable {
            get {
                return ResourceManager.GetString("AddUsersTable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to drop view NotificationLogView;
        ///
        ///create view NotificationLogView as
        ///select
        ///	NL.ID as ID,
        ///	C.CreditNumber as CreditNumber,
        ///	P.Name as PersonName,
        ///	NL.CreditID as CreditID,
        ///	NL.PersonID as PersonID,
        ///	NL.NotificationDate as NotificationDate,
        ///	NL.Comment as Comment
        ///from NotificationLog as NL
        ///inner join Credits as C on C.ID = NL.CreditID
        ///inner join Persons as P on P.ID = NL.PersonID
        ///where C.RowState &lt;&gt; 1;.
        /// </summary>
        internal static string AlterNotificationLogViewToFilterDeletedCredits {
            get {
                return ResourceManager.GetString("AlterNotificationLogViewToFilterDeletedCredits", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- Recreate Credits table.
        ///create table Credits2
        ///(
        ///	ID integer primary key autoincrement not null,
        ///	CreditState integer not null default 1,
        ///	CreditNumber nvarchar(100),
        ///	CreditAmount decimal not null,
        ///	CreditIssueDate date not null,
        ///	MonthsCount integer not null,
        ///	DiscountRate decimal(18, 4) not null,
        ///	EffectiveDiscountRate decimal(18, 4),
        ///	ExchangeRate decimal(18, 4)
        ///);
        ///
        ///insert into Credits2
        ///(
        ///	ID, CreditState, CreditNumber, CreditAmount, CreditIssueDate,
        ///	MonthsCount, DiscountRate, Effect [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ChangeCreditsAndPersonsTablesToAllowNullValues {
            get {
                return ResourceManager.GetString("ChangeCreditsAndPersonsTablesToAllowNullValues", resourceCulture);
            }
        }
    }
}
