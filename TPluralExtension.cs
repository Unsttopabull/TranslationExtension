using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Frost.GettextMarkupExtension {

    public class TPluralExtension : TExtension {

        [ConstructorArgument("pluralKey")]
        public string PluralKey { get; set; }

        [ConstructorArgument("number")]
        public long Number { get; set; }

        public TPluralExtension() {
            
        }

        /// <summary>Initializes a new instance of a class derived from <see cref="T:System.Windows.Markup.MarkupExtension"/>. </summary>
        public TPluralExtension(string key, string pluralKey, long number) : base(key) {
            PluralKey = pluralKey;
            Number = number;
            Arguments = new object[] { number };
        }

        /// <summary>Initializes a new instance of a class derived from <see cref="T:System.Windows.Markup.MarkupExtension"/>. </summary>
        public TPluralExtension(string key, string pluralKey, long number, object param) : base(key) {
            PluralKey = pluralKey;
            Number = number;
            Arguments = new[] { param };
        }

        /// <summary>Initializes a new instance of a class derived from <see cref="T:System.Windows.Markup.MarkupExtension"/>. </summary>
        public TPluralExtension(string key, string pluralKey, long number, string prefix, string postfix, string context, string[] args) : base(key, prefix, postfix, context, args) {
            Number = number;
            PluralKey = pluralKey;
        }

        /// <summary>When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.</summary>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        public override object ProvideValue(IServiceProvider serviceProvider) {
             if (Key == null || PluralKey == null) {
                throw new InvalidOperationException("Translation key and plural key must be provided");
            }

            IProvideValueTarget ipvt = (IProvideValueTarget) serviceProvider.GetService(typeof(IProvideValueTarget));

            if (ipvt.TargetProperty is DependencyProperty) {
                Binding binding = new Binding("Value") {
                    Source = new TranslationData(Key, PluralKey, Number, Context, Arguments, Prefix, Postfix)
                };

                return binding.ProvideValue(serviceProvider);
            }

            return TranslationData.ProvideValue(Key, PluralKey, Number, Context, Arguments, Prefix, Postfix);
        }
    }

}