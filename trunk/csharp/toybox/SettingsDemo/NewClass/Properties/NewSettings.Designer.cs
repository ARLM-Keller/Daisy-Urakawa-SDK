﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3603
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NewClass.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "8.0.0.0")]
    internal sealed partial class NewSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static NewSettings defaultInstance = ((NewSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new NewSettings())));
        
        public static NewSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("string1")]
        public string UserSettingsString1 {
            get {
                return ((string)(this["UserSettingsString1"]));
            }
            set {
                this["UserSettingsString1"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ApplicationSettings")]
        public string AppSettingsString {
            get {
                return ((string)(this["AppSettingsString"]));
            }
        }
    }
}
