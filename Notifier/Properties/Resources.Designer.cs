﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Notifier.Properties {
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
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Notifier.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Ошибка.
        /// </summary>
        internal static string Error {
            get {
                return ResourceManager.GetString("Error", resourceCulture);
            }
        }
        
        internal static System.Drawing.Bitmap RefreshIcon {
            get {
                object obj = ResourceManager.GetObject("RefreshIcon", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Возникла ошибка при обновлении списка оповещения. См. логи..
        /// </summary>
        internal static string RefreshNotificationListError {
            get {
                return ResourceManager.GetString("RefreshNotificationListError", resourceCulture);
            }
        }
        
        internal static System.Drawing.Bitmap SaveIcon {
            get {
                object obj = ResourceManager.GetObject("SaveIcon", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Возникла ошибка при сохранении номеров телефонов. См. логи..
        /// </summary>
        internal static string SavePhoneNumbersError {
            get {
                return ResourceManager.GetString("SavePhoneNumbersError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Возникла ошибка при попытке отправить СМС. См. логи..
        /// </summary>
        internal static string SendSmsError {
            get {
                return ResourceManager.GetString("SendSmsError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Возникла ошибка при попытке отобразить форму Оповещение. См. логи..
        /// </summary>
        internal static string ShowNotificationFormError {
            get {
                return ResourceManager.GetString("ShowNotificationFormError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Указан неизвестный мобильный провайдер..
        /// </summary>
        internal static string UnknownMobileProvider {
            get {
                return ResourceManager.GetString("UnknownMobileProvider", resourceCulture);
            }
        }
    }
}
