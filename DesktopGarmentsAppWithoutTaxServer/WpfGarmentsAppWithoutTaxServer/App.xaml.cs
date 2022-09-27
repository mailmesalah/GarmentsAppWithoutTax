using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace WpfServerApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            EventManager.RegisterClassHandler(typeof(TextBox), TextBox.KeyDownEvent, new KeyEventHandler(Control_KeyDown));
            EventManager.RegisterClassHandler(typeof(ComboBox), ComboBox.KeyDownEvent, new KeyEventHandler(Control_KeyDown));
            EventManager.RegisterClassHandler(typeof(DatePicker), DatePicker.KeyDownEvent, new KeyEventHandler(Control_KeyDown));
        }

        private void Control_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FocusNavigationDirection direction = FocusNavigationDirection.Next;
                TraversalRequest request = new TraversalRequest(direction);
                UIElement element = Keyboard.FocusedElement as UIElement;
                if (element != null)
                {
                    //element.MoveFocus(request);
                    if (element.MoveFocus(request))
                        e.Handled = true;
                }
            }
        }
    }
}
