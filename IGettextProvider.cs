using System.Collections.Generic;
using System.Globalization;

namespace Frost.GettextMarkupExtension {
    public interface IGettextProvider {

        /// <summary>Translates the specified key.</summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        string Translate(string key, params object[] args);

        string TranslateContext(string key, string context, params object[] args);

        string TranslatePlural(string key, string pluralKey, long number, params object[] args);

        string TranslateContextPlural(string key, string pluralKey, string context, long number, params object[] args);

        /// <summary>Gets the available languages.</summary>
        /// <value>The available languages.</value>
        IEnumerable<CultureInfo> Languages { get; }

        CultureInfo SelectedCulture { set; }

        string DirectoryPath { get; set; }

        string ResourceName { get; set; }
    }
}
