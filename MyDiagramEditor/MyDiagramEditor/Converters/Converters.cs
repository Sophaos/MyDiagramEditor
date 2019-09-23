using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MyDiagramEditor.Converters
{
    class ConvertisseurBordure : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value.ToString() == parameter.ToString()) ? "#FF58BDFA" : "#00000000";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => System.Windows.DependencyProperty.UnsetValue;
    }

    class GridLengthConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = (double)value;
            GridLength gridLength = new GridLength(val);

            return gridLength;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            GridLength val = (GridLength)value;

            return val.Value;
        }
    }
    class ConvertisseurCouleurFond : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value.ToString() == parameter.ToString()) ? "#3F58BDFA" : "#00000000";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => System.Windows.DependencyProperty.UnsetValue;
    }

    class ConvertisseurModeEdition : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch (value)
            {
                case "deplacement":
                    return 1;
                case "artefact":
                    return 1;
                case "activite":
                    return 1;
                case "role":
                    return 1;
                case "commentaire":
                    return 1;
                case "phase":
                    return 1;
                case "texte":
                    return 1;
                case "ligne":
                    return 1;
                case "classe":
                    return 1;
                case "line":
                    return 1;
                case "square":
                    return 1;
                case "dashed":
                    return 1;
                case "lineConnexion":
                    return 1;
                case "directionnalArrow":
                    return 1;
                case "bidirectionnalArrow":
                    return 1;
                case "bidirectionnalLink":
                    return 1;
                case "unidirectionnalLink":
                    return 1;
                case "aggregationLink":
                    return 1;
                case "compositionLink":
                    return 1;
                case "inheritanceLink":
                    return 1;
                case "resize":
                    return 1;
                default:
                    return 1;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => System.Windows.DependencyProperty.UnsetValue;
    }
}
