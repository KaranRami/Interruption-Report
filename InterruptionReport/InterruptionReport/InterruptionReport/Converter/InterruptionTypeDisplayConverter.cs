using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace InterruptionReport.Converter
{
    public class InterruptionTypeDisplayTpyeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Application.Current.Resources["FontAwesomeRegularFree"];
            else if (value.ToString().ToLower() == parameter.ToString().ToLower())
                return Application.Current.Resources["FontAwesomeSolidFree"];
            else
                return Application.Current.Resources["FontAwesomeRegularFree"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InterruptionTypeDisplayTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "circle";
            else if (value.ToString().ToLower() == parameter.ToString().ToLower())
                return "dot-circle";
            else
                return "circle";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InterruptionTypeDisplayTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Label.TextColorProperty.DefaultValue;
            else if (value.ToString().ToLower() == parameter.ToString().ToLower())
                return App.Current.Resources["PrimaryColor"];
            else
                return Label.TextColorProperty.DefaultValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
