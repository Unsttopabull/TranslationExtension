using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;

namespace Frost.GettextMarkupExtension {
    public class Gettext : DependencyObject {
        private static readonly Gettext Singleton = new Gettext();
        public event EventHandler LanguageChanged;

        static Gettext() {
            
        }

        private Gettext() {
        }

        #region Static members & Wrappers

        public static Gettext Instance {
            get { return Singleton; }
        }

        public static CultureInfo CurrentCulture {
            get { return Instance.CurrentLangauge; }
            set { Instance.CurrentLangauge = value; }
        }

        public static IGettextProvider CurrentTranslationProvider {
            get { return Instance.TranslationProvider; }
            set { Instance.TranslationProvider = value; }
        }

        public static bool IsInDesignMode {
            get {
                if (Instance.Dispatcher == null || Instance.Dispatcher.Thread == null || !Instance.Dispatcher.Thread.IsAlive) {
                    return false;
                }

                return !Instance.Dispatcher.CheckAccess()
                           ? (bool) Instance.Dispatcher.Invoke(new Func<bool>(() => IsInDesignMode))
                           : DesignerProperties.GetIsInDesignMode(Instance);
            }
        }

        public static string T(string key, params object[] args) {
            return Instance.Translate(key, args);
        }

        public static string Tc(string key, string context, params object[] args) {
            return Instance.TranslateContext(key, context, args);
        }

        public static string Tp(string key, string pluralKey, long number, params object[] args) {
            return Instance.TranslatePlural(key, pluralKey, number, args);
        }

        public static string Tpc(string key, string pluralKey, long number, string context, params object[] args) {
            return Instance.TranslatePluralContext(key, pluralKey, number, context, args);
        }

        #endregion


        public IGettextProvider TranslationProvider { get; set; }

        public CultureInfo CurrentLangauge {
            get { return Thread.CurrentThread.CurrentUICulture; }
            set {
                if (!Equals(value, CurrentLangauge)) {
                    Thread.CurrentThread.CurrentUICulture = value;
                    OnLanguageChanged();
                }
            }
        }

        public IEnumerable<CultureInfo> Languages {
            get {
                return TranslationProvider != null
                    ? TranslationProvider.Languages
                    : Enumerable.Empty<CultureInfo>();
            }
        }

        public string Translate(string key, params object[] args) {
            if (TranslationProvider == null) {
                return key;
            }

            return TranslationProvider.Translate(key, args) ?? key;
        }

        public string TranslateContext(string key, string context, params object[] args) {
            if (TranslationProvider == null) {
                return key;
            }

            return TranslationProvider.TranslateContext(key, context, args) ?? key;
        }

        public string TranslatePlural(string key, string pluralKey, long number, object[] args) {
            if (TranslationProvider == null) {
                return key;
            }

            return TranslationProvider.TranslatePlural(key, pluralKey, number, args) ?? key;
        }

        public string TranslatePluralContext(string key, string pluralKey, long number, string context, object[] args) {
            if (TranslationProvider == null) {
                return key;
            }

            return TranslationProvider.TranslateContextPlural(key, pluralKey, context, number, args) ?? key;
        }

        protected virtual void OnLanguageChanged() {
            if (CurrentTranslationProvider != null) {
                CurrentTranslationProvider.SelectedCulture = Thread.CurrentThread.CurrentUICulture;
            }

            if (LanguageChanged != null) {
                LanguageChanged(this, EventArgs.Empty);
            }
        }
    }

}
