using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using SecondLanguage;

namespace Frost.GettextMarkupExtension {

    public class SecondLanguageTranslationProvider : IGettextProvider {
        private IEnumerable<CultureInfo> _availableLanguages;
        private readonly Translator _translator;

        public string DirectoryPath { get; set; }

        public string ResourceName { get; set; }

        public SecondLanguageTranslationProvider(string directoryPath, string resourceName = null) {
            _translator = Translator.Default;

            DirectoryPath = directoryPath;
            if (resourceName != null) {
                ResourceName = resourceName;
            }

            RefreshAvailableCulutres();
            SelectedCulture = Thread.CurrentThread.CurrentUICulture;
        }

        public void RefreshAvailableCulutres() {
            DirectoryInfo executingDir;
            try {
                executingDir = new DirectoryInfo(DirectoryPath ?? ".");
            }
            catch {
                Languages = Enumerable.Empty<CultureInfo>();
                return;
            }

            List<CultureInfo> ci = new List<CultureInfo>();

            List<string> cultures;
            if (string.IsNullOrEmpty(ResourceName)) {
                cultures = executingDir.GetFiles("*.mo")
                                       .Select(f => Path.GetFileNameWithoutExtension(f.Name))
                                       .ToList();

                foreach (string culture in cultures) {
                    CultureInfo cultureInfo;
                    try {
                        cultureInfo = CultureInfo.GetCultureInfo(culture);
                    }
                    catch {
                        continue;
                    }
                    _translator.RegisterTranslationsByCulture(executingDir.Name + "/{0}.mo", cultureInfo);
                }

                cultures = executingDir.GetFiles("*.po")
                                       .Where(fi => !cultures.Contains(Path.GetFileNameWithoutExtension(fi.Name)))
                                       .Select(f => Path.GetFileNameWithoutExtension(f.Name))
                                       .ToList();

                foreach (string culture in cultures) {
                    CultureInfo cultureInfo;
                    try {
                        cultureInfo = CultureInfo.GetCultureInfo(culture);
                    }
                    catch {
                        continue;
                    }
                    _translator.RegisterTranslationsByCulture(executingDir.Name + "/{0}.po", cultureInfo);
                }
            }
            else {
                cultures = executingDir.GetDirectories()
                                       .Select(f => f.Name)
                                       .ToList();

                foreach (string culture in cultures) {
                    CultureInfo cultureInfo;
                    try {
                        cultureInfo = CultureInfo.GetCultureInfo(culture);
                    }
                    catch {
                        continue;
                    }
                    _translator.RegisterTranslationsByCulture(executingDir.Name + "/{0}/*.mo", cultureInfo);
                    _translator.RegisterTranslationsByCulture(executingDir.Name + "/{0}/*.po", cultureInfo);
                }
            }

            foreach (string culureTag in cultures) {
                CultureInfo culture;
                try {
                    culture = CultureInfo.GetCultureInfo(culureTag);
                }
                catch {
                    continue;
                }

                ci.Add(culture);
            }
            Languages = ci;
        }

        /// <summary>Translates the specified key.</summary>
        /// <param name="key">The key.</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public string Translate(string key, params object[] args) {
            return Gettext.IsInDesignMode ? key : _translator.Translate(key, args ?? new object[] { });
        }

        public string TranslateContext(string key, string context, params object[] args) {
            return Gettext.IsInDesignMode ? key : _translator.TranslateContextual(context, key, args ?? new object[] { });
        }

        public string TranslatePlural(string key, string pluralKey, long number, params object[] args) {
            return Gettext.IsInDesignMode ? key : _translator.TranslatePlural(key, pluralKey, number, args ?? new object[] { });
        }

        public string TranslateContextPlural(string key, string pluralKey, string context, long number, params object[] args) {
            return Gettext.IsInDesignMode ? key : _translator.TranslateContextualPlural(context, key, pluralKey, number, args ?? new object[] { });
        }

        /// <summary>Gets the available languages.</summary>
        /// <value>The available languages.</value>
        public IEnumerable<CultureInfo> Languages {
            get {
                if (_availableLanguages == null) {
                    RefreshAvailableCulutres();
                }
                return _availableLanguages;
            }
            private set { _availableLanguages = value; }
        }

        public CultureInfo SelectedCulture {
            set {
                Translator.SelectedCulture = value;
            }
            get { return Translator.SelectedCulture; }
        }

    }

}