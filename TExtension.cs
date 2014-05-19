using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

[assembly:XmlnsDefinition("http://www.frostmediamanager.com/xaml/translation", "Frost.GettextMarkupExtension")]
[assembly:XmlnsPrefix("http://www.frostmediamanager.com/xaml/translation", "l")]
namespace Frost.GettextMarkupExtension {

    /// <summary>Represents a Gettext translation extension</summary>
    public class TExtension : MarkupExtension {

        /// <summary>Initializes a new instance of a class derived from <see cref="T:System.Windows.Markup.MarkupExtension"/>. </summary>
        public TExtension() {
            
        }

        /// <summary>Initializes a new instance of a class derived from <see cref="T:System.Windows.Markup.MarkupExtension"/>. </summary>
        public TExtension(string key) {
            Key = key;
        }

        /// <summary>Initializes a new instance of a class derived from <see cref="T:System.Windows.Markup.MarkupExtension"/>. </summary>
        public TExtension(string key, object[] args) {
            Key = key;
            Arguments = args;
        }

        /// <summary>Initializes a new instance of a class derived from <see cref="T:System.Windows.Markup.MarkupExtension"/>. </summary>
        public TExtension(string key, string prefix, string postfix, string context) {
            Key = key;
            Prefix = prefix;
            Postfix = postfix;
            Context = context;
        }

        /// <summary>Initializes a new instance of a class derived from <see cref="T:System.Windows.Markup.MarkupExtension"/>. </summary>
        public TExtension(string key, string prefix, string postfix, string context, object[] args) {
            Key = key;
            Prefix = prefix;
            Postfix = postfix;
            Context = context;
            Arguments = args;
        }

        [ConstructorArgument("key")]
        public string Key { get; set; }

        [ConstructorArgument("prefix")]
        public string Prefix { get; set; }

        [ConstructorArgument("postfix")]
        public string Postfix { get; set; }

        [ConstructorArgument("context")]
        public string Context { get; set; }

        [ConstructorArgument("args")]
        public object[] Arguments { get; set; }

        /// <summary>When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.</summary>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        public override object ProvideValue(IServiceProvider serviceProvider) {
            if (Key == null) {
                throw new InvalidOperationException("Translation key must be provided");
            }

            IProvideValueTarget ipvt = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));

            if (!(ipvt.TargetProperty is DependencyProperty)) {
                return TranslationData.ProvideValue(Key, Context, Arguments, Prefix, Postfix);
            }

            Binding binding = new Binding("Value") {
                Source = new TranslationData(Key, Context, Arguments, Prefix, Postfix)
            };

            return binding.ProvideValue(serviceProvider);
        }
    }

}
